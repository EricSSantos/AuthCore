namespace AuthCore.Domain.Commons.Interfaces.Security
{
    public interface IBCrypt
    {
        /// <summary>
        /// Gera um hash seguro a partir de um texto simples.
        /// </summary>
        string Hash(string plainText);

        /// <summary>
        /// Verifica se o texto simples corresponde ao hash armazenado.
        /// </summary>
        bool isValid(string plainText, string hash);
    }
}
