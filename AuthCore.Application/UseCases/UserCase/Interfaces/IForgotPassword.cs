using AuthCore.Application.Models.Input;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    public interface IForgotPassword
    {
        Task OnExecute(ForgotPasswordInputModel input);
    }
}
