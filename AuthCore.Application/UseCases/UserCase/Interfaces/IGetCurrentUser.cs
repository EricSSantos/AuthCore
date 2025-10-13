using AuthCore.Application.Models.Output;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    public interface IGetCurrentUser
    {
        /// <summary>
        /// Executa a operação de obtenção do usuário atual.
        /// </summary>
        Task<UserViewModel> OnExecute();
    }
}
