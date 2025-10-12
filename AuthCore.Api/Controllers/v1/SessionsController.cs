using AuthCore.Application.UseCases.SessionCase.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCore.Api.Controllers.v1
{
    [Authorize]
    [ApiController]
    [Route("api/v1/sessions")]
    public sealed class SessionsController : Controller
    {
        private readonly IGetUserSessions _getUserSessions;

        public SessionsController(IGetUserSessions getUserSessions)
        {
            _getUserSessions = getUserSessions;
        }

        /// <summary>
        /// Retorna todas as sessões ativas de usuário autenticado
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sessions = await _getUserSessions.OnExecute();
            return Ok(sessions);
        }
    }
}
