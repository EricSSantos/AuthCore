using System.Security.Cryptography;

namespace AuthCore.Domain.Aggregates.EmailAggregate.Payloads
{
    /// <summary>
    /// Payload para e-mails de recuperação de senha,
    /// contendo o código numérico que será validado pelo usuário.
    /// </summary>
    public sealed class ForgotPasswordPayload : EmailPayload
    {
        private const int CODE_MIN = 100_000;
        private const int CODE_MAX = 1_000_000;

        public int Code { get; init; }

        public ForgotPasswordPayload()
        {
            Code = GenerateCode();
        }

        private static int GenerateCode()
        {
            return RandomNumberGenerator.GetInt32(CODE_MIN, CODE_MAX);
        }
    }
}
