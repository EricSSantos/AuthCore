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
    public sealed class SessionsController : ControllerBase
    {
        /// <summary>
        /// Lista outras sessões ativas do usuário.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Response<IEnumerable<SessionViewModel>>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Response<IEnumerable<SessionViewModel>>>> GetAll(
            [FromServices] IGetSessions getUserSessions)
        {
            var sessions = await getUserSessions.OnExecute();

            return Ok(Response<IEnumerable<SessionViewModel>>.Success(
                sessions,
                "Sessões recuperadas com sucesso.",
                HttpStatusCode.OK
            ));
        }

        /// <summary>
        /// Encerra todas as outras sessões e mantém apenas a atual.
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(Response<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Response<object>>> Revoke(
            [FromServices] IRevokeSession revokeSessions)
        {
            await revokeSessions.OnExecute();

            return Ok(Response<object>.Success(
                null!,
                "Sessões revogadas com sucesso.",
                HttpStatusCode.OK
            ));
        }
    }
}
