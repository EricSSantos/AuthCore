namespace AuthCore.Application.UseCases.Auth.Interfaces
{
    /// <summary>Define operação para renovar o token de acesso.</summary>
    public interface IRefresh
    {
        /// <summary>Operação para renovar o token do usuário.</summary>
        Task OnExecuteAsync();
    }
}
