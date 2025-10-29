using AuthCore.Application.Models.Responses;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    public interface IGetCurrentUser
    {
        Task<UserResponse> OnExecute();
    }
}
