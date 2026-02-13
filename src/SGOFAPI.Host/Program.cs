using Parameters.Presentation;
using Parameters.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using PHCAPI.Host.Middleware;
using PHCAPI.Host.Filters;
using Audit.Presentation;
using Hangfire;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/PHCAPI-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    // ✅ Exception Handling (ResponseDTO-based)
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // ✅ Hangfire (Background Jobs para Audit)
    builder.Services.AddHangfire(config =>
    {
        config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
        config.UseSimpleAssemblyNameTypeSerializer();
        config.UseRecommendedSerializerSettings();
        config.UseSqlServerStorage(
            builder.Configuration.GetConnectionString("DBconnect"),
            new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
                PrepareSchemaIfNecessary = true, // Criar tabelas se não existirem
                SchemaName = "PHCHANGFIRE" // Schema separado para Hangfire
            });
    });

    // ✅ Configurar política de retry para jobs
    GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
    {
        Attempts = 3, // Tentar 3 vezes
        DelaysInSeconds = new[] { 10, 30, 60 }, // Esperar 10s, 30s, 60s entre tentativas
        LogEvents = true,
        OnAttemptsExceeded = AttemptsExceededAction.Delete // Deletar após 3 falhas
    });


    // ✅ Hangfire Server com configurações defensivas
    builder.Services.AddHangfireServer(options =>
    {
        options.WorkerCount = 5; // Limite de workers
        options.ServerTimeout = TimeSpan.FromMinutes(5);
        options.ShutdownTimeout = TimeSpan.FromMinutes(1);

        // ✅ Nome único do servidor (facilita identificação no dashboard)
       
    });

   

    builder.Services.AddParametersPresentation(
        builder.Configuration,
        enableRest: true,
        enableGraphQL: false
    );

    builder.Services.AddAuditPresentation(
        builder.Configuration,
        enableRest: true
    );

    // Add Controllers and API Explorer
    var mvcBuilder = builder.Services.AddControllers(options =>
    {
    
        options.Filters.Add<AuditLoggingFilter>();
    }).AddApplicationPart(typeof(Parameters.Presentation.REST.Controllers.ParametersController).Assembly)
        .AddApplicationPart(typeof(Audit.Presentation.REST.Controllers.AuditController).Assembly);

    Log.Information("✅ AuditLoggingFilter registered globally");

    builder.Services.AddEndpointsApiExplorer();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error during service configuration: {Message}", ex.Message);
    if (ex.InnerException != null)
        Log.Fatal(ex.InnerException, "Inner exception: {Message}", ex.InnerException.Message);
    throw;
}

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "PHCAPI",
        Version = "v1",
        Description = "PHC API",
        Contact = new()
        {
            Name = "2Business Team",
            Email = "suporte.mz@2business-si.com"
        }
    });

    // Include XML comments from all modules
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Parameters module XML
    var parametersXml = "Parameters.Presentation.xml";
    var parametersXmlPath = Path.Combine(AppContext.BaseDirectory, parametersXml);
    if (File.Exists(parametersXmlPath))
    {
        c.IncludeXmlComments(parametersXmlPath);
    }

    // Audit module XML
    var auditXml = "Audit.Presentation.xml";
    var auditXmlPath = Path.Combine(AppContext.BaseDirectory, auditXml);
    if (File.Exists(auditXmlPath))
    {
        c.IncludeXmlComments(auditXmlPath);
    }
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

WebApplication app;

try
{
    app = builder.Build();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to build application: {Message}", ex.Message);

    if (ex is AggregateException aggEx)
    {
        foreach (var innerEx in aggEx.InnerExceptions)
        {
            Log.Fatal(innerEx, "Inner exception: {Message}", innerEx.Message);
        }
    }

    Log.CloseAndFlush();
    throw;
}

// ✅ Exception Handler Middleware (DEVE vir primeiro!)
app.UseExceptionHandler();

// ✅ Correlation ID Middleware (para rastreabilidade)
app.UseMiddleware<CorrelationIdMiddleware>();

// ✅ NOVO: Usando AuditLoggingFilter (ActionFilter) em vez de Middleware
// Registrado em AddControllers() - MUITO mais estável!



// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PHCAPI v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });

    // ✅ Hangfire Dashboard com configuração defensiva
    try
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireAuthorizationFilter() },
            StatsPollingInterval = 5000, // 5 segundos
            DisplayStorageConnectionString = false, // Não mostrar connection string
            DashboardTitle = "PHCAPI - Background Jobs"
        });

        Log.Information("Hangfire Dashboard available at /hangfire");
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Failed to initialize Hangfire Dashboard - continuing without it");
    }

    // ✅ Database First Approach - Tabelas já existem no banco
    // Não precisa de migrations automáticas
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

try
{
   
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
