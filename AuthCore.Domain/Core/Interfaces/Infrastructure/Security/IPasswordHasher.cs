namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações para gerar e validar hashes de senha.</summary>
    public interface IPasswordHasher
    {
        /// <summary>Operação para gerar hash de senha.</summary>
        /// <param name="plainText">Senha em texto simples.</param>
        string Hash(string plainText);

        /// <summary>Operação para verificar hash de senha.</summary>
        /// <param name="plainText">Senha digitada.</param>
        /// <param name="hash">Hash armazenado.</param>
        bool IsValid(string plainText, string hash);
    }
}
