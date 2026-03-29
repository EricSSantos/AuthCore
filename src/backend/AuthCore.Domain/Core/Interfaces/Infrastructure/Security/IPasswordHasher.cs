namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações para gerar e validar hashes de senha.</summary>
    public interface IPasswordHasher
    {
        /// <summary>Operação para gerar hash da senha informada.</summary>
        /// <param name="plainText">Senha em texto puro.</param>
        string Hash(string plainText);

        /// <summary>Operação para validar senha informada com o hash armazenado.</summary>
        /// <param name="plainText">Senha em texto puro.</param>
        /// <param name="passwordHash">Hash da senha armazenada.</param>
        bool IsValid(string plainText, string passwordHash);
    }
}