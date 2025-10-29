using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    public interface IConfirmEmail
    {
        Task OnExecute(ConfirmEmailRequest request);
    }
}
