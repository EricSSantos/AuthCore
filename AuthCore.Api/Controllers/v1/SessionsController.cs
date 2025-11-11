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
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SessionResponse>>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<SessionResponse>>>> GetAll(
            [FromServices] IGetSessions getSessions)
        {
            var sessions = await getSessions.OnExecute();

            return Ok(ApiResponse<IEnumerable<SessionResponse>>.Success(
                sessions,
                "Sessões recuperadas com sucesso.",
                HttpStatusCode.OK
            ));
        }

        /// <summary>
        /// Revoga todas as sessões do usuário, mantendo apenas a atual.
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResponse<object>>> Revoke(
            [FromServices] IRevokeSession revokeSessions)
        {
            await revokeSessions.OnExecute();

            return Ok(ApiResponse<object>.Success(
                null!,
                "Sessões revogadas com sucesso.",
                HttpStatusCode.OK
            ));
        }
    }
}
