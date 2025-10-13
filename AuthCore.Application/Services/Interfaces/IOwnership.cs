namespace AuthCore.Application.Services.Interfaces
{
    public interface IOwnership
    {
        /// <summary>
        /// Valida se a entidade pertence ao usuário autenticado.
        /// </summary>
        void Ensure(Guid entityUserId);

        /// <summary>
        /// Valida se todos os recursos pertencem ao usuário autenticado.
        /// </summary>
        void EnsureAll(IEnumerable<Guid> entityUserIds);
    }
}
