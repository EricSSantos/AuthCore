using AuthCore.Application.Services.Interfaces;
using AuthCore.Application.UseCases.SessionCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Commons.Exceptions;

namespace AuthCore.Application.UseCases.SessionCase
{
    public sealed class RevokeSessions : IRevokeSession
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IOwnership _ownership;

        public RevokeSessions(
            ISessionRepository sessionRepository,
            IOwnership ownership)
        {
            _sessionRepository = sessionRepository;
            _ownership = ownership;
        }

        public async Task OnExecute(List<Guid> ids)
        {
            if (ids is null || ids.Count == 0)
                throw new DomainException("Nenhuma sessão foi informada para revogação.");

            var sessions = await ValidateSessions(ids);

            var deleteTasks = sessions.Select(s => _sessionRepository.DeleteById(s.Id));
            await Task.WhenAll(deleteTasks);
        }

        #region Private Methods

        private async Task<List<Session>> ValidateSessions(IEnumerable<Guid> ids)
        {
            var tasks = ids.Select(id => _sessionRepository.GetBySessionId(id));
            var results = await Task.WhenAll(tasks);

            var sessions = results
                .Where(s => s != null)
                .ToList()!;

            if (sessions.Count <= 0)
                throw new NotFoundException("Nenhuma sessão válida foi encontrada para os identificadores informados.");

            // Garante que todas as sessões informadas pertencem ao usuário autenticado, 
            // evitando que ele revogue sessões de outros usuários.
            _ownership.EnsureAll(sessions.Select(s => s.UserId));

            return sessions!;
        }

        #endregion
    }
}
