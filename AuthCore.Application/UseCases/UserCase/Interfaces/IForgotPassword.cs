using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    public interface IForgotPassword
    {
        Task OnExecute(ForgotPasswordRequest request);
    }
}
