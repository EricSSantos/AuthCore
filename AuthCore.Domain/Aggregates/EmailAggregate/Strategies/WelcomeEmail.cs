namespace AuthCore.Domain.Aggregates.EmailAggregate.Strategies
{
    /// <summary>
    /// E-mail de boas-vindas enviado a novos usuários.
    /// </summary>
    public sealed class WelcomeEmail : Email
    {
        internal WelcomeEmail(string to, string fullName)
            : base(to, fullName, EmailType.Welcome)
        { }

        internal static WelcomeEmail Create(string to, string fullName)
        {
            return new WelcomeEmail(to, fullName);
        }
    }
}
