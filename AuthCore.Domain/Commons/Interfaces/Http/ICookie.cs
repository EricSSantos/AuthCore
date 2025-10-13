namespace AuthCore.Domain.Commons.Interfaces.Http
{
    public interface ICookie
    {
        /// <summary>
        /// Obtém o valor bruto do cookie de sessão.
        /// </summary>
        string Session { get; }

        /// <summary>
        /// Obtém o token de acesso armazenado no cookie.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Obtém o token de atualização armazenado no cookie.
        /// </summary>
        string RefreshToken { get; }

        /// <summary>
        /// Define os cookies de autenticação (sessão, access e refresh token).
        /// </summary>
        void SetAuthCookies(string rawSession, string accessToken, string refreshToken);

        /// <summary>
        /// Remove todos os cookies de autenticação.
        /// </summary>
        void RemoveAuthCookies();
    }
}
