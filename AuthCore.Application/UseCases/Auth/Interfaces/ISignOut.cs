namespace AuthCore.Application.UseCases.Auth.Interfaces
{
    /// <summary>Define operação para encerrar a sessão do usuário.</summary>
    public interface ISignOut
    {
        /// <summary>Operação para realizar o logout do usuário.</summary>
        Task OnExecuteAsync();
    }
}
