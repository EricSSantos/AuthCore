namespace AuthCore.Domain.Shared
{
    /// <summary>
    /// Representa a classe base para todas as entidades do domínio.
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
    }
}
