using AuthCore.Application.Models.Input;

namespace AuthCore.Application.UseCases.AuthCase.Interfaces
{
    public interface ISignIn
    {
        Task OnExecute(SignInInputModel input);
    }
}
