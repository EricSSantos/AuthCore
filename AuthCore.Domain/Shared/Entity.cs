namespace AuthCore.Domain.Shared
{
    /// <summary>
    /// Classe base para todas as entidades do domínio.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Identificador único da entidade.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância da entidade com um identificador único.
        /// </summary>
        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Inicializa uma nova instância da entidade com um identificador existente.
        /// Usado principalmente na reidratação a partir do banco de dados.
        /// </summary>
        protected Entity(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Cria uma nova instância de um validador de domínio.
        /// </summary>
        protected DomainValidator Validator()
        {
            return new DomainValidator();
        }
    }
}
