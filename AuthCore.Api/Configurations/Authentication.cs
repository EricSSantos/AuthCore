using AuthCore.Domain.Commons.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace AuthCore.Api.Configurations
{
    public static class Authentication
    {
        public static void AddAuthentication(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            var settings = configuration.GetSection("Security").Get<SecuritySettings>();
            if (settings is null)
                throw new InvalidOperationException("As configurações de segurança não foram definidas.");

            var publicKey = LoadPublicKey(settings);

            ConfigureAuthentication(services, settings, publicKey);
        }

        private static ECDsaSecurityKey LoadPublicKey(SecuritySettings securitySettings)
        {
            var publicKeyValue = securitySettings.Keys.Asymmetric.PublicKeyPath;

            if (string.IsNullOrWhiteSpace(publicKeyValue))
                throw new FileNotFoundException("Caminho ou variável de chave pública não definido.");

            string? publicKeyPem;
            var envValue = Environment.GetEnvironmentVariable(publicKeyValue);

            if (!string.IsNullOrWhiteSpace(envValue))
                publicKeyPem = envValue;
            else if (File.Exists(publicKeyValue))
                publicKeyPem = File.ReadAllText(publicKeyValue);
            else
                throw new FileNotFoundException($"Chave pública não encontrada. Nem variável '{publicKeyValue}' nem arquivo físico existente.");

            var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(publicKeyPem);

            return new ECDsaSecurityKey(ecdsa);
        }

        private static void ConfigureAuthentication(IServiceCollection services, SecuritySettings settings, ECDsaSecurityKey publicKey)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue("access_token", out var cookieToken))
                            context.Token = cookieToken;
                        else if (!string.IsNullOrWhiteSpace(context.Request.Headers.Authorization))
                        {
                            var header = context.Request.Headers.Authorization.ToString();
                            if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                                context.Token = header["Bearer ".Length..];
                        }

                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = publicKey,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = settings.Jwt.Issuer,
                    ValidAudience = settings.Jwt.Audience,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });
        }
    }
}
