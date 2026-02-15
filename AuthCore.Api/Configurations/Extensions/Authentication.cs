using AuthCore.Domain.Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>Configura a autenticação JWT da aplicação.</summary>
    public static class AuthenticationExtensions
    {
        /// <summary>Operação para adicionar e configura autenticação JWT.</summary>
        /// <param name="builder">Instância para configurar serviços.</param>
        public static void AddAuthentication(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;
            var settings = configuration.GetSection("Security").Get<SecuritySettings>()
                ?? throw new InvalidOperationException("As configurações de segurança não foram definidas.");
            var key = GetPublicKey(settings);
            ConfigureJwtAuthentication(services, settings, key);
        }

        #region Helpers

        /// <summary>Operação para carregar a chave pública usada na validação JWT.</summary>
        /// <param name="securitySettings">Configurações de segurança.</param>
        private static ECDsaSecurityKey GetPublicKey(SecuritySettings securitySettings)
        {
            var keyPath = securitySettings.Keys.Asymmetric.PublicKeyPath
                ?? throw new FileNotFoundException("Caminho ou variável de chave pública não definido.");

            var publicKeyPem = Environment.GetEnvironmentVariable(keyPath)
                ?? (File.Exists(keyPath) ? File.ReadAllText(keyPath) : null)
                ?? throw new FileNotFoundException("Chave pública não encontrada. Nenhuma variável de ambiente ou arquivo físico foi localizado.");

            var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(publicKeyPem);

            return new ECDsaSecurityKey(ecdsa);
        }

        /// <summary>Operação para configurar o esquema e validação JWT.</summary>
        /// <param name="services">Coleção de serviços.</param>
        /// <param name="settings">Configurações carregadas.</param>
        /// <param name="publicKey">Chave pública de validação.</param>
        private static void ConfigureJwtAuthentication(IServiceCollection services, SecuritySettings settings, ECDsaSecurityKey publicKey)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            services
                .AddAuthentication(options =>
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
                            var settings = context.HttpContext.RequestServices
                                .GetRequiredService<IOptions<SecuritySettings>>().Value;
                            context.Token = context.Request.Cookies[settings.Cookies.AccessTokenKey];
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
