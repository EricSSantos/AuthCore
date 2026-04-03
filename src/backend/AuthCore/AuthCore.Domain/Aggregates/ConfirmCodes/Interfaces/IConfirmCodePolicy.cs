namespace AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces
{
    /// <summary>Define operações de política de código de confirmação.</summary>
    public interface IConfirmCodePolicy
    {
        /// <summary>Operação para gerar código de confirmação.</summary>
        /// <param name="type">Tipo do código.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        int GenerateCode(CodeType type, DateTime utcNow);

        /// <summary>Operação para obter expiração do código.</summary>
        /// <param name="type">Tipo do código.</param>
        TimeSpan GetExpiration(CodeType type);
    }
}
