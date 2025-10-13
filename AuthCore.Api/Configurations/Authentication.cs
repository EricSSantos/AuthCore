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

            var key = GetKey(settings);

            Configure(services, settings, key);
        }

        #region Private Methods

        private static ECDsaSecurityKey GetKey(SecuritySettings securitySettings)
        {
            var keyPath = securitySettings.Keys.Asymmetric.PublicKeyPath
                ?? throw new FileNotFoundException("Caminho ou variável de chave pública não definido.");

            var publicKeyPem =
                Environment.GetEnvironmentVariable(keyPath)
                ?? (File.Exists(keyPath) ? File.ReadAllText(keyPath) : null)
                ?? throw new FileNotFoundException("Chave pública não encontrada. Nenhuma variável de ambiente ou arquivo físico foi localizado.");

            var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(publicKeyPem);

            return new ECDsaSecurityKey(ecdsa);
        }

        private static void Configure(IServiceCollection services, SecuritySettings settings, ECDsaSecurityKey publicKey)
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
                        context.Token = context.Request.Cookies["access_token"];
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

        #endregion
    }
}
