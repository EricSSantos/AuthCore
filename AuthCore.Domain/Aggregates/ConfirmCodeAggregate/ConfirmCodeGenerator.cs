using System.Security.Cryptography;

namespace AuthCore.Domain.Aggregates.ConfirmCodeAggregate
{
    /// <summary>
    /// Gera códigos numéricos seguros para confirmações.
    /// </summary>
    internal static class ConfirmCodeGenerator
    {
        #region Constants

        private const int CODE_MIN = 100_000;
        private const int CODE_MAX = 1_000_000;

        #endregion

        /// <summary>
        /// Gera um código seguro de 6 dígitos.
        /// </summary>
        /// <returns>Código numérico gerado.</returns>
        internal static int Generate()
        {
            return RandomNumberGenerator.GetInt32(CODE_MIN, CODE_MAX);
        }
    }
}
