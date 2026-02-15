namespace AuthCore.Application.UseCases.Sessions.Interfaces
{
    /// <summary>Define operação para revogar a sessão atual.</summary>
    public interface IRevokeSession
    {
        /// <summary>Operação para revogar a sessão do usuário.</summary>
        Task OnExecuteAsync();
    }
}
