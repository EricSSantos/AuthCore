using AuthCore.Application.Services.Interfaces;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Security.Jwt;

namespace AuthCore.Application.Services
{
    public sealed class Ownership : IOwnership
    {
        private readonly IAccessToken _accessToken;

        public Ownership(IAccessToken accessToken)
        {
            _accessToken = accessToken;
        }

        public void Ensure(Guid entityUserId)
        {
            var currentUserId = _accessToken.Sub;

            if (entityUserId != currentUserId)
                throw new ForbiddenException("Você não tem permissão para acessar este recurso.");
        }

        public void EnsureAll(IEnumerable<Guid> entityUserIds)
        {
            var currentUserId = _accessToken.Sub;

            if (entityUserIds is null || !entityUserIds.Any())
                return;

            var invalids = entityUserIds.Where(id => id != currentUserId).ToList();

            if (invalids.Any())
                throw new ForbiddenException("Um ou mais recursos não pertencem ao usuário autenticado.");
        }
    }
}
