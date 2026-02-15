using System.Linq;
using AuthCore.Domain.Core.Settings;

namespace AuthCore.Domain.Aggregates.Sessions.Policies
{
    /// <summary>Representa a política padrão de sessões.</summary>
    public sealed class DefaultSessionPolicy : ISessionPolicy
    {
        private readonly SecuritySettings _settings;

        /// <summary>Operação para criar instância de política.</summary>
        /// <param name="settings">Configurações de segurança.</param>
        public DefaultSessionPolicy(SecuritySettings settings)
        {
            _settings = settings;
        }

        /// <summary>Operação para selecionar sessões a revogar.</summary>
        /// <param name="sessions">Coleção de sessões do usuário.</param>
        public IReadOnlyCollection<Session> SelectSessionsToRevoke(IReadOnlyCollection<Session> sessions)
        {
            var maxSessions = _settings.Session.MaxSessionsPerUser;
            if (maxSessions < 1)
                return Array.Empty<Session>();

            if (sessions is null || sessions.Count < maxSessions)
                return Array.Empty<Session>();

            var sessionsToRevoke = sessions
                .OrderBy(session => session.CreatedAt)
                .Take(sessions.Count - maxSessions + 1)
                .ToList();

            return sessionsToRevoke;
        }
    }
}
