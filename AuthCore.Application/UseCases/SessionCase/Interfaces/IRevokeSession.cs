namespace AuthCore.Application.UseCases.SessionCase.Interfaces
{
    public interface IRevokeSession
    {
        Task OnExecute(List<Guid> Ids);
    }
}
