using Parameters.Presentation;
using Parameters.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using PHCAPI.Host.Middleware;
using PHCAPI.Host.Filters;
using Audit.Presentation;
using Hangfire;
using Hangfire.SqlServer;
using Auth.Infrastructure;
using Auth.Infrastructure.Data;
using Auth.Infrastructure.Extensions;
using Auth.Presentation;
using Shared.Kernel.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Auth.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.File("logs/PHCAPI-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    builder.Services.AddProblemDetails();

    // ✅ Hangfire (Background Jobs para Audit)
    builder.Services.AddHangfire(config =>
    {
        config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
        config.UseSimpleAssemblyNameTypeSerializer();
        config.UseRecommendedSerializerSettings();
        
        // ❌ Desabilitar logs verbose do Hangfire
        
        config.UseSqlServerStorage(
            builder.Configuration.GetConnectionString("DBconnect"),
            new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
                PrepareSchemaIfNecessary = true,
                SchemaName = "PHCHANGFIRE"
            });
    });

    // ✅ Configurar política de retry para jobs
    GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
    {
        Attempts = 3, // Tentar 3 vezes
        DelaysInSeconds = new[] { 10, 30, 60 }, // Esperar 10s, 30s, 60s entre tentativas
        LogEvents = false, // ❌ Desabilitado para reduzir logs
        OnAttemptsExceeded = AttemptsExceededAction.Delete // Deletar após 3 falhas
    });


    // ✅ Hangfire Server com configurações defensivas
    builder.Services.AddHangfireServer(options =>
    {
        options.WorkerCount = 5; // Limite de workers
        options.ServerTimeout = TimeSpan.FromMinutes(5);
        options.ShutdownTimeout = TimeSpan.FromMinutes(1);       
    });

    // ✅ Authentication & Authorization Module
    builder.Services.AddAuthPresentation(builder.Configuration);
    
    // Configure JWT Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = false;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? 
                    throw new InvalidOperationException("JWT:Secret not configured")))
        };
    });

    // Configure Authorization Policies
    builder.Services.AddAuthorization(options =>
    {
        // Policy for internal users only (e.g., Audit module)
        options.AddPolicy(AppPolicies.InternalOnly, policy =>
            policy.RequireRole(AppRoles.Administrator, AppRoles.InternalUser, AppRoles.AuditViewer));

        // Policy for administrators only
        options.AddPolicy(AppPolicies.AdminOnly, policy =>
            policy.RequireRole(AppRoles.Administrator));

        // Policy for API access (any authenticated user with API role)
        options.AddPolicy(AppPolicies.ApiAccess, policy =>
            policy.RequireRole(AppRoles.Administrator, AppRoles.ApiUser, AppRoles.InternalUser));

        // Policy for any authenticated user
        options.AddPolicy(AppPolicies.Authenticated, policy =>
            policy.RequireAuthenticatedUser());
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
        // ✅ Audit logging agora é feito via ResponseLoggingMiddleware
    }).AddApplicationPart(typeof(Parameters.Presentation.REST.Controllers.ParametersController).Assembly)
        .AddApplicationPart(typeof(Audit.Presentation.REST.Controllers.AuditController).Assembly)
        .AddApplicationPart(typeof(Auth.Presentation.Controllers.AuthenticateController).Assembly);


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

    // Auth module XML
    var authXml = "Auth.Presentation.xml";
    var authXmlPath = Path.Combine(AppContext.BaseDirectory, authXml);
    if (File.Exists(authXmlPath))
    {
        c.IncludeXmlComments(authXmlPath);
    }

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
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

// ✅ Initialize Auth Database (migrations + seeding)
await app.UseAuthDatabaseAsync();

// ✅ Exception Handler Middleware (DEVE vir primeiro!)
app.UseExceptionHandler();

// ✅ Correlation ID Middleware (para rastreabilidade)
app.UseMiddleware<CorrelationIdMiddleware>();

// ✅ Response Logging Middleware (captura TODAS as respostas incluindo erros)
app.UseMiddleware<ResponseLoggingMiddleware>();



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

    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Failed to initialize Hangfire Dashboard - continuing without it");
    }

    // ✅ Database First Approach - Tabelas já existem no banco
    // Não precisa de migrations automáticas
}

// ❌ Desabilitado para reduzir logs
// app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

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
   // Log.CloseAndFlush();
}
