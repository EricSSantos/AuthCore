namespace AuthCore.Application.UseCases.AuthCase.Interfaces
{
    /// <summary>
    /// Define operação para renovar o token de acesso.
    /// </summary>
    public interface IRefresh
    {
        /// <summary>
        /// Renova o token do usuário.
        /// </summary>
        Task OnExecuteAsync();
    }
}
