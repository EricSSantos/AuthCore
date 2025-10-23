using AuthCore.Domain.Aggregates.EmailAggregate.Payloads;

namespace AuthCore.Domain.Aggregates.EmailAggregate.Strategies
{
    /// <summary>
    /// E-mail para recuperação de senha,
    /// contendo um código temporário de 6 digitos.
    /// </summary>
    public sealed class ForgotPasswordEmail : Email
    {
        internal ForgotPasswordEmail(string to, string fullName, ForgotPasswordPayload payload)
            : base(to, fullName, EmailType.ForgotPassword, payload)
        { }

        internal static ForgotPasswordEmail Create(string to, string fullName)
        {
            return new ForgotPasswordEmail(to, fullName, new ForgotPasswordPayload());
        }
    }
}
