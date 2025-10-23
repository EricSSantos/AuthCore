using AuthCore.Application.Models.Input;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    public interface IChangePassword
    {
        Task OnExecute(ChangePasswordInputModel input);
    }
}
