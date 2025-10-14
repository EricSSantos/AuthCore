namespace AuthCore.Domain.Commons.Interfaces.Security.Hashing
{
    public interface IEntropy
    {
        /// <summary>
        /// Gera um par composto por valor bruto e seu hash correspondente.
        /// </summary>
        (string Raw, string Hash) GeneratePair(int size = 64);

        /// <summary>
        /// Gera o hash de um valor bruto.
        /// </summary>
        string Hash(string raw);

        /// <summary>
        /// Verifica se o valor bruto corresponde ao hash informado.
        /// </summary>
        bool Verify(string raw, string hash);
    }
}
