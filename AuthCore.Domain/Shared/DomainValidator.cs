using AuthCore.Domain.Commons.Exceptions;

namespace AuthCore.Domain.Shared
{
    /// <summary>
    /// Fornece um contexto de validação de domínio reutilizável e desacoplado.
    /// Permite acumular erros e lançar exceções padronizadas.
    /// </summary>
    public sealed class DomainValidator
    {
        private readonly List<string> _errors = new();

        /// <summary>
        /// Indica se há erros acumulados.
        /// </summary>
        public bool HasErrors
        {
            get { return _errors.Count > 0; }
        }

        /// <summary>
        /// Retorna a lista de erros acumulados.
        /// </summary>
        public IReadOnlyCollection<string> Errors
        {
            get { return _errors.AsReadOnly(); }
        }

        /// <summary>
        /// Adiciona um erro de validação à lista interna.
        /// </summary>
        public void AddError(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                _errors.Add(message);
            }
        }

        /// <summary>
        /// Lança uma exceção de requisição inválida se houver erros acumulados.
        /// </summary>
        public void ThrowIfInvalid()
        {
            if (_errors.Count > 0)
            {
                throw new BadRequestException(_errors);
            }
        }
    }
}
