using AuthCore.Application.Models;
using AuthCore.Application.Models.Output;
using AuthCore.Application.UseCases.SessionCase.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthCore.Api.Controllers.v1
{
    [Authorize]
    [ApiController]
    [Route("api/v1/sessions")]
    public sealed class SessionsController : Controller
    {
        private readonly IGetSessions _getUserSessions;
        private readonly IRevokeSession _revokeSessions;

        public SessionsController(
            IGetSessions getUserSessions,
            IRevokeSession revokeSessions)
        {
            _getUserSessions = getUserSessions;
            _revokeSessions = revokeSessions;
        }

        /// <summary>
        /// Retorna todas as sessões ativas.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<SessionViewModel>>>> GetAll()
        {
            var sessions = await _getUserSessions.OnExecute();

            var response = Response<IEnumerable<SessionViewModel>>.Success(
                sessions,
                "Sessões recuperadas com sucesso.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }

        /// <summary>
        /// Revoga uma ou mais sessões.
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult<Response<object>>> Revoke([FromBody] List<Guid> ids)
        {
            await _revokeSessions.OnExecute(ids);

            var response = Response<object>.Success(
                null!,
                "Sessões revogadas com sucesso.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }
    }
}
