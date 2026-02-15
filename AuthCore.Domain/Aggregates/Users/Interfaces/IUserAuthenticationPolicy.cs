namespace AuthCore.Domain.Aggregates.Users.Interfaces
{
    /// <summary>Define operações de política de autenticação de usuário.</summary>
    public interface IUserAuthenticationPolicy
    {
        /// <summary>Operação para validar autenticação do usuário.</summary>
        /// <param name="user">Usuário a validar.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        void EnsureCanAuthenticate(User user, DateTime utcNow);

        /// <summary>Operação para registrar falha de autenticação.</summary>
        /// <param name="loginAttempts">Tentativas atuais.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        LoginAttempts RegisterFailure(LoginAttempts loginAttempts, DateTime utcNow);
    }
}
