using System.Text;
using System.Threading.RateLimiting;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Auth;
using Gdn.Web.Api.Vs.Features.CreditNotes.Reports;
using Gdn.Web.Api.Vs.Features.CreditNotes.Xml;
using Gdn.Web.Api.Vs.Features.Interventions.Reports;
using Gdn.Web.Api.Vs.Features.Invoices.Reports;
using Gdn.Web.Api.Vs.Features.Invoices.Xml;
using Gdn.Web.Api.Vs.Features.Payments.Reports;
using Gdn.Web.Api.Vs.Features.Quotes.Reports;
using Gdn.Web.Api.Vs.Features.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace Gdn.Web.Api.Vs;

public static class DependencyInjection
{
    public static IServiceCollection AddReports(this IServiceCollection services)
    {
        services.AddScoped<InvoiceReportGenerator>();
        services.AddScoped<CreditNoteReportGenerator>();
        services.AddScoped<InterventionReportGenerator>();
        services.AddScoped<ReceiptReportGenerator>();
        services.AddScoped<QuoteReportGenerator>();
        return services;
    }

    public static IServiceCollection AddFatturaElettronica(this IServiceCollection services)
    {
        services.AddScoped<InvoiceXmlGenerator>();
        services.AddScoped<InvoiceXmlFileNameGenerator>();
        services.AddScoped<CreditNoteXmlGenerator>();
        services.AddScoped<CreditNoteXmlFileNameGenerator>();
        return services;
    }

    /// <summary>
    /// Registers everything the authentication layer needs: password hashing, token issuing, the
    /// settings store, bearer validation and the authorization policies.
    /// </summary>
    /// <remarks>
    /// Every endpoint is protected by the fallback policy, so a new feature slice is authenticated by
    /// default and has to opt out explicitly rather than remember to opt in.
    /// </remarks>
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, JwtSettings jwtSettings)
    {
        if (Encoding.UTF8.GetByteCount(jwtSettings.SigningKey ?? string.Empty) < 32)
        {
            throw new InvalidOperationException(
                "JwtSettings:SigningKey non configurata o troppo corta: servono almeno 32 byte. " +
                "Impostala negli user secrets in locale e nelle application settings in produzione.");
        }

        services.AddMemoryCache();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<TokenService>();
        services.AddScoped<AuthenticationSeeder>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // Keep the short claim names the token was issued with instead of expanding them
                    // into the legacy URI based ones.
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey!)),
                        ValidateLifetime = true,

                        // The five minute default would make a fifteen minute token unpredictable.
                        ClockSkew = TimeSpan.FromSeconds(30),

                        NameClaimType = TokenService.EmailClaimType,
                        RoleClaimType = TokenService.RoleClaimType
                    };
                });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            options.AddPolicy(Policies.AdminOnly, policy => policy.RequireRole(UserRole.Admin));
        });

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Partitioned by caller address, so one client hammering the sign in endpoint cannot
            // lock everybody else out of it. The account lockout is a separate, per account defence.
            options.AddPolicy(Policies.AuthenticationRateLimit, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });

        return services;
    }
}
