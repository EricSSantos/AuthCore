using AuthCore.Domain.Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>
    /// Configura a autenticação JWT da aplicação.
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Adiciona e configura a autenticação JWT.
        /// </summary>
        /// <param name="builder">Instância usada para configurar os serviços da aplicação.</param>
        /// <exception cref="InvalidOperationException">Lançada quando as configurações de segurança não estão definidas.</exception>
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

        /// <summary>
        /// Carrega a chave pública ECDSA usada para validar os tokens JWT.
        /// </summary>
        /// <param name="securitySettings">Configurações de segurança da aplicação.</param>
        /// <returns>Chave pública para validação de tokens.</returns>
        /// <exception cref="FileNotFoundException">Lançada quando a chave pública não é encontrada.</exception>
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

        /// <summary>
        /// Configura o esquema de autenticação JWT e os parâmetros de validação.
        /// </summary>
        /// <param name="services">Coleção de serviços da aplicação.</param>
        /// <param name="settings">Configurações de segurança carregadas do appsettings.</param>
        /// <param name="publicKey">Chave pública usada para validar tokens.</param>
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
