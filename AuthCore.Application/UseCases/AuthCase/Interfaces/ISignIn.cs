using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.AuthCase.Interfaces
{
    public interface ISignIn
    {
        Task OnExecute(SignInRequest request);
    }
}
