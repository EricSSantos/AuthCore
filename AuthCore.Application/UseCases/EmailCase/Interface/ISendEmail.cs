using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.EmailCase.Interface
{
    public interface ISendEmail
    {
        Task OnExecute(SendEmailRequest request);
    }
}
