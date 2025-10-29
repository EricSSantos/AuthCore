using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    public interface IChangePassword
    {
        Task OnExecute(ChangePasswordRequest request);
    }
}
