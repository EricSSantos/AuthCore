using AuthCore.Application.Models.Output;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    public interface IGetCurrentUser
    {
        Task<UserViewModel> OnExecute();
    }
}
