using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReadOrNot.Application.Interfaces;
using ReadOrNot.Application.Options;
using ReadOrNot.Infrastructure.Auth;
using ReadOrNot.Infrastructure.BotDetection;
using ReadOrNot.Infrastructure.Identity;
using ReadOrNot.Infrastructure.Persistence;
using ReadOrNot.Infrastructure.Privacy;
using ReadOrNot.Infrastructure.Services;
using ReadOrNot.Infrastructure.Time;

namespace ReadOrNot.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReadOrNotInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string sqlServerMigrationsAssembly,
        string mySqlMigrationsAssembly)
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(options => !options.SigningKey.Contains("change-this", StringComparison.OrdinalIgnoreCase), "Jwt:SigningKey must be changed from the sample value.")
            .ValidateOnStart();

        services.AddOptions<TrackingOptions>()
            .Bind(configuration.GetSection(TrackingOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<PrivacyOptions>()
            .Bind(configuration.GetSection(PrivacyOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<RetentionOptions>()
            .Bind(configuration.GetSection(RetentionOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<FrontendOptions>()
            .Bind(configuration.GetSection(FrontendOptions.SectionName))
            .ValidateOnStart();

        services.AddDbContext<ReadOrNotDbContext>((serviceProvider, options) =>
        {
            var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var connectionString = configuration.GetConnectionString(databaseOptions.ConnectionStringName)
                ?? throw new InvalidOperationException($"Connection string '{databaseOptions.ConnectionStringName}' is not configured.");

            ReadOrNotDbContextFactory.ConfigureProvider(
                options,
                databaseOptions,
                connectionString,
                databaseOptions.Provider.Equals("MySql", StringComparison.OrdinalIgnoreCase) ? mySqlMigrationsAssembly : sqlServerMigrationsAssembly);
        });

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ReadOrNotDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

        services.AddAuthorization();
        services.AddHealthChecks().AddDbContextCheck<ReadOrNotDbContext>("database");

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITrackingService, TrackingService>();
        services.AddScoped<ITokenRetentionService, OpenEventRetentionService>();
        services.AddSingleton<IAccessTokenService, JwtAccessTokenService>();
        services.AddSingleton<IBotDetector, HeuristicBotDetector>();
        services.AddSingleton<IIpPrivacyService, HmacIpPrivacyService>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<ITokenIdentifierGenerator, TokenIdentifierGenerator>();
        services.AddSingleton<ITokenPublicUrlBuilder, TokenPublicUrlBuilder>();
        services.AddSingleton<ITrackingAssetService, TrackingAssetService>();
        services.AddHostedService<OpenEventRetentionHostedService>();

        return services;
    }
}
