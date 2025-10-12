using AuthCore.Application.Models.Output;

namespace AuthCore.Application.UseCases.SessionCase.Interfaces
{
    public interface IGetUserSessions
    {
        Task<IEnumerable<SessionViewModel>> OnExecute();
    }
}
