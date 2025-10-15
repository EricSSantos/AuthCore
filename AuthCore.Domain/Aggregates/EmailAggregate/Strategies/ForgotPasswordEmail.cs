using AuthCore.Domain.Aggregates.EmailAggregate.Payloads;

namespace AuthCore.Domain.Aggregates.EmailAggregate.Strategies
{
    /// <summary>
    /// Representa um e-mail para recuperação de senha, contendo
    /// um código OTP temporário para verificação do usuário.
    /// </summary>
    public sealed class ForgotPasswordEmail : Email
    {
        private ForgotPasswordEmail(string to, string fullName, ForgotPasswordPayload payload)
            : base(to, fullName, EmailType.ForgotPassword, payload)
        { }

        /// <summary>
        /// Cria uma nova instância de <see cref="ForgotPasswordEmail"/> 
        /// com um código OTP gerado automaticamente.
        /// </summary>
        public static ForgotPasswordEmail Create(string to, string fullName)
        {
            return new ForgotPasswordEmail(
                to,
                fullName,
                new ForgotPasswordPayload()
            );
        }
    }
}
