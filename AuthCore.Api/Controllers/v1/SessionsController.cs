using AuthCore.Application.Models;
using AuthCore.Application.Models.Responses;
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
        /// Retorna todas as sessões ativas do usuário.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Response<IEnumerable<SessionResponse>>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Response<IEnumerable<SessionResponse>>>> GetAll(
            [FromServices] IGetSessions getSessions)
        {
            var sessions = await getSessions.OnExecute();

            return Ok(Response<IEnumerable<SessionResponse>>.Success(
                sessions,
                "Sessões recuperadas com sucesso.",
                HttpStatusCode.OK
            ));
        }

        /// <summary>
        /// Revoga todas as sessões do usuário, mantendo apenas a atual.
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
