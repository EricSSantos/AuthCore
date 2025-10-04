using AuthCore.Application.Models.Input;
using AuthCore.Application.Models.Output;

namespace AuthCore.Application.UseCases.AuthCase.SignIn
{
    public interface ISigIn
    {
        Task<SignInViewModel> OnExecute(SigInInputModel input);
    }
}
