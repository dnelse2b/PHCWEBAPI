using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SGOFWS.Domains.Interface;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Extensions;
using SGOFWS.Jobs;
using SGOFWS.MiddleWares;
using SGOFWS.Persistence.APIs.MPDC.Helpers;
using SGOFWS.Persistence.Contexts;
using SGOFWS.Persistence.Repositories;
using SGOFWS.Services;
using SGOFWS.Services.External;

internal class Program
{
	private static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		ConsignacaoJob consignacaoJob = new ConsignacaoJob();
		ConversionExtension conversionExtension = new ConversionExtension();
		ConfigurationManager configuration2 = builder.Configuration;
		builder.Services.AddDbContext<SGOFCTX>(delegate(DbContextOptionsBuilder options)
		{
			options.UseSqlServer(configuration2.GetConnectionString("DBconnect"), delegate(SqlServerDbContextOptionsBuilder sqlServerOptions)
			{
				sqlServerOptions.CommandTimeout(120);
			});
		});
		builder.Services.AddDbContext<CFMMAINCONTEXT>(delegate(DbContextOptionsBuilder options)
		{
			options.UseSqlServer(configuration2.GetConnectionString("CFMGERALConnStr"), delegate(SqlServerDbContextOptionsBuilder sqlServerOptions)
			{
				sqlServerOptions.CommandTimeout(120);
			});
		});
		builder.Services.AddDbContext<AuthAppContext>(delegate(DbContextOptionsBuilder options)
		{
			options.UseSqlServer(configuration2.GetConnectionString("DBconnect"));
		});
		builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AuthAppContext>().AddDefaultTokenProviders();
		builder.Services.AddScoped<IConsignacaoRepository, ConsignacaoRepository>();
		builder.Services.AddScoped<IGenericRepository, GenericRepository>();
		builder.Services.AddScoped<ICFMMAinBDRepository, CFMMAinBDRepository>();
		builder.Services.AddScoped<IMPDCService, MPDCService>();
		builder.Services.AddScoped<ITFRService, TFRService>();
		builder.Services.AddScoped<IMPDCHelper, MPDCHelper>();
		builder.Services.AddScoped<ITFRService, TFRService>();
		builder.Services.AddScoped<ISyncRepository, SyncRepository>();
		builder.Services.AddScoped<IOPFService, OPFService>();
		builder.Services.AddScoped<IOPFRepository, OPFRepository>();
		builder.Services.AddScoped<ISyncService, SyncService>();
		builder.Services.AddScoped<ITrackService, TrackService>();
		builder.Services.AddScoped<IConsignmentService, ConsignmentService>();
		builder.Services.AddAuthentication(delegate(AuthenticationOptions options)
		{
			options.DefaultAuthenticateScheme = "Bearer";
			options.DefaultChallengeScheme = "Bearer";
			options.DefaultScheme = "Bearer";
		}).AddJwtBearer(delegate(JwtBearerOptions options)
		{
			options.Events = new JwtBearerEvents
			{
				OnAuthenticationFailed = (AuthenticationFailedContext context) => Task.CompletedTask,
				OnForbidden = (ForbiddenContext context) => Task.CompletedTask,
				OnChallenge = (JwtBearerChallengeContext context) => Task.CompletedTask
			};
			options.SaveToken = false;
			options.RequireHttpsMetadata = false;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidAudience = configuration2["JWT:ValidAudience"],
				ValidIssuer = configuration2["JWT:ValidIssuer"],
				ValidateLifetime = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration2["JWT:Secret"]))
			};
		});
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		builder.Services.AddControllers();
		builder.Services.AddMemoryCache();
		builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
		builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
		builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
		builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
		builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
		builder.Services.AddInMemoryRateLimiting();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddMvcCore().AddApiExplorer();
		builder.Services.AddHangfire(delegate(IGlobalConfiguration configuration)
		{
			configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("DBconnect"), new SqlServerStorageOptions
			{
				SchemaName = "SFGOFHANGFIRE"
			});
		});
		builder.Services.Configure(delegate(KestrelServerOptions options)
		{
			options.AllowSynchronousIO = true;
		});
		WebApplication app = builder.Build();
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}
		app.UseSwagger();
		app.UseSwaggerUI();
		app.UseIpRateLimiting();
		app.UseHangfireServer();
		app.UseHangfireDashboard("/Jobs");
		app.UseDefaultFiles();
		app.UseStaticFiles();
		app.UseHttpsRedirection();
		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();
		app.UseMiddleware<HttpLoggingMiddleware>(Array.Empty<object>());
		app.UseEndpoints(delegate(IEndpointRouteBuilder endpoints)
		{
			endpoints.MapGet("/", async delegate(HttpContext context)
			{
				await context.Response.WriteAsync("<h1>THE WEB SERVER IS ON!</h1>");
			});
		});
		consignacaoJob.test();
		consignacaoJob.executeJobs();
		app.MapControllers();
		app.Run();
	}
}
