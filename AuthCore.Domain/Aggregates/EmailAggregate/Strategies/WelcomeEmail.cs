namespace AuthCore.Domain.Aggregates.EmailAggregate.Strategies
{
    /// <summary>
    /// Representa um e-mail de boas-vindas enviado a novos usuários.
    /// </summary>
    public sealed class WelcomeEmail : Email
    {
        private WelcomeEmail(string to, string fullName)
            : base(to, fullName, EmailType.Welcome)
        { }

        /// <summary>
        /// Cria uma nova instância de <see cref="WelcomeEmail"/> garantindo os dados essenciais.
        /// </summary>
        public static WelcomeEmail Create(string to, string fullName)
        {
            return new WelcomeEmail(to, fullName);
        }
    }
}
