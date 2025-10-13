using AuthCore.Application.Models.Output;

namespace AuthCore.Application.UseCases.SessionCase.Interfaces
{
    public interface IGetSessions
    {
        Task<IEnumerable<SessionViewModel>> OnExecute();
    }
}
