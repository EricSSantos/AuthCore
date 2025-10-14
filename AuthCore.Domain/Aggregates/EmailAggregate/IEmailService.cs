using AuthCore.Domain.Aggregates.EmailAggregate;

namespace AuthCore.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, EmailType type, object? data = null);
    }
}
