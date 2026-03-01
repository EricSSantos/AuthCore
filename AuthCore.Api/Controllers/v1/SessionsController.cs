using AuthCore.Application.Models;
using AuthCore.Application.Models.Responses;
using AuthCore.Application.UseCases.Sessions.Interfaces;
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
        /// <summary>Operação para retornar as sessões ativas do usuário.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SessionResponse>>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<SessionResponse>>>> GetAll(
            [FromServices] IGetSessions getSessions)
        {
            var sessions = await getSessions.OnExecuteAsync();

            return Ok(ApiResponse<IEnumerable<SessionResponse>>.Success(
                data: sessions,
                title: "Sessões recuperadas com sucesso.",
                statusCode: HttpStatusCode.OK
            ));
        }

        /// <summary>Operação para revogar todas as sessões do usuário exceto a atual.</summary>
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Revoke(
            [FromServices] IRevokeSession revokeSessions)
        {
            await revokeSessions.OnExecuteAsync();

            return NoContent();
        }
    }
}
