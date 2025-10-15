using System.Security.Cryptography;

namespace AuthCore.Domain.Aggregates.EmailAggregate.Payloads
{
    /// <summary>
    /// Payload específico para e-mails de recuperação de senha,
    /// contendo o código OTP que será validado pelo usuário.
    /// </summary>
    public sealed class ForgotPasswordPayload : EmailPayload
    {
        private const int OTP_LENGTH = 6;

        public string Code { get; init; }

        public ForgotPasswordPayload()
        {
            Code = GenerateOtp();
        }

        /// <summary>
        /// Gera um código numérico aleatório OTP de 6 dígitos.
        /// </summary>
        private static string GenerateOtp()
        {
            int value = RandomNumberGenerator.GetInt32(0, 1_000_000);
            return value.ToString().PadLeft(OTP_LENGTH, '0');
        }
    }
}
