namespace AuthCore.Application.UseCases.AuthCase.Interfaces
{
    /// <summary>
    /// Define operação para encerrar a sessão do usuário.
    /// </summary>
    public interface ISignOut
    {
        /// <summary>
        /// Realiza o logout do usuário.
        /// </summary>
        Task OnExecuteAsync();
    }
}
