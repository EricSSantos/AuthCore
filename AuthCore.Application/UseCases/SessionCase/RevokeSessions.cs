using AuthCore.Application.UseCases.SessionCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Security;

namespace AuthCore.Application.UseCases.SessionCase
{
    public sealed class RevokeSessions : IRevokeSession
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IAccessToken _accessToken;

        public RevokeSessions(
            ISessionRepository sessionRepository,
            IAccessToken accessToken)
        {
            _sessionRepository = sessionRepository;
            _accessToken = accessToken;
        }

        public async Task OnExecute(List<Guid> Ids)
        {
            if (!Ids.Any())
                throw new DomainException("Nenhuma sessão foi informada para revogação.");

            var tasks = new List<Task>();

            foreach (var id in Ids)
            {
                tasks.Add(_sessionRepository.DeleteById(id));
            }

            await Task.WhenAll(tasks);
        }
    }
}
