namespace AuthCore.Application.UseCases.SessionCase.Interfaces
{
    /// <summary>
    /// Define operação para revogar a sessão atual.
    /// </summary>
    public interface IRevokeSession
    {
        /// <summary>
        /// Revoga a sessão do usuário.
        /// </summary>
        Task OnExecuteAsync();
    }
}
