using AuthCore.Application.Models.Input;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    public interface IResetPassword
    {
        Task OnExecute(ResetPasswordInputModel input);
    }
}
