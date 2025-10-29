using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    public interface IResetPassword
    {
        Task OnExecute(ResetPasswordRequest request);
    }
}
