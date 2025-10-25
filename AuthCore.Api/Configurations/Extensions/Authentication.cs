using AuthCore.Domain.Commons.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace AuthCore.Api.Configurations.Extensions
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Registra e configura a autenticação JWT no container de serviços da aplicação.
        /// </summary>
        /// <param name="builder">Instância do <see cref="WebApplicationBuilder"/> utilizada para configurar os serviços.</param>
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

        #region Private Methods

        /// <summary>
        /// Obtém e importa a chave pública ECDSA a partir do caminho físico ou da variável de ambiente configurada.
        /// </summary>
        /// <param name="securitySettings">Configurações de segurança carregadas do arquivo <c>appsettings.json</c>.</param>
        /// <returns>Instância de <see cref="ECDsaSecurityKey"/> contendo a chave pública para validação de tokens.</returns>
        /// <exception cref="FileNotFoundException">Lançada quando a chave pública não é encontrada.</exception>
        private static ECDsaSecurityKey GetPublicKey(SecuritySettings securitySettings)
        {
            var keyPath = securitySettings.Keys.Asymmetric.PublicKeyPath
                ?? throw new FileNotFoundException("Caminho ou variável de chave pública não definido.");

            // Tenta ler a chave pública da variável de ambiente ou do arquivo físico
            var publicKeyPem = Environment.GetEnvironmentVariable(keyPath)
                ?? (File.Exists(keyPath) ? File.ReadAllText(keyPath) : null)
                ?? throw new FileNotFoundException("Chave pública não encontrada. Nenhuma variável de ambiente ou arquivo físico foi localizado.");

            var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(publicKeyPem);

            return new ECDsaSecurityKey(ecdsa);
        }

        /// <summary>
        /// Configura o esquema de autenticação JWT, definindo o algoritmo de assinatura,
        /// os validadores de emissor, audiência e chave, e a leitura do token via cookie.
        /// </summary>
        /// <param name="services">Coleção de serviços da aplicação.</param>
        /// <param name="settings">Configurações de segurança extraídas do <see cref="SecuritySettings"/>.</param>
        /// <param name="publicKey">Chave pública utilizada para validação dos tokens.</param>
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
                    // Permite que o token seja lido a partir de cookies
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
