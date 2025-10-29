using AuthCore.Application.Models.Responses;

namespace AuthCore.Application.UseCases.SessionCase.Interfaces
{
    public interface IGetSessions
    {
        Task<IEnumerable<SessionResponse>> OnExecute();
    }
}
