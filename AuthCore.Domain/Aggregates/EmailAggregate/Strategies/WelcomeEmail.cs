namespace AuthCore.Domain.Aggregates.EmailAggregate.Strategies
{
    /// <summary>
    /// Representa o e-mail de boas-vindas enviado a um novo usuário após o registro.
    /// </summary>
    public sealed class WelcomeEmail : Email
    {
        #region Constructors

        internal WelcomeEmail(string to, string fullName)
            : base(to, fullName, EmailType.Welcome)
        { }

        #endregion

        #region Factory

        /// <summary>
        /// Cria uma nova instância de e-mail de boas-vindas.
        /// </summary>
        internal static WelcomeEmail Create(string to, string fullName)
        {
            return new WelcomeEmail(to, fullName);
        }

        #endregion
    }
}
