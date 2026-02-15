namespace AuthCore.Application.UseCases.Auth.Contracts
{
    /// <summary>Define operação para renovar o token de acesso.</summary>
    public interface IRefresh
    {
        /// <summary>Operação para renovar o token do usuário.</summary>
        Task OnExecuteAsync();
    }
}
