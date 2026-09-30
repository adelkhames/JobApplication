using Hangfire;
using Hangfire.SqlServer;
using JobApplication.API.Middleware;
using JobApplication.Application.Common.Interfaces;
using JobApplication.Application.Features.Jobs.Commands.CreateJob;
using JobApplication.Infrastructure.Identity;
using JobApplication.Infrastructure.Notifications;
using JobApplication.Infrastructure.Persistence;
using JobApplication.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace JobApplication.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // ── Hangfire ──────────────────────────────────────────────────────────
        builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout       = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout   = TimeSpan.FromMinutes(5),
                QueuePollInterval            = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks           = true
            }));

        builder.Services.AddHangfireServer();
        // ─────────────────────────────────────────────────────────────────────

        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        var jwtSection = builder.Configuration.GetSection("Jwt");
        var jwtKey = jwtSection["Key"]
            ?? throw new InvalidOperationException("JWT Key is not configured.");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        // CQRS/MediatR
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreateJobCommand).Assembly));

        // Infrastructure implementations
        builder.Services.AddScoped<IJobRepository, JobRepository>();
        builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();

        // Notification service
        builder.Services.AddScoped<INotificationService, NotificationService>();

        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        // ── Hangfire Dashboard ────────────────────────────────────────────────
        app.UseHangfireDashboard("/hangfire");
        // ─────────────────────────────────────────────────────────────────────

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
