using AuthCore.Application.Models;
using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthCore.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController : ControllerBase
    {
        /// <summary>
        /// Autentica o usuário e inicia uma nova sessão.
        /// </summary>
        [HttpPost("sign-in")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<ApiResponse<object>>> SignIn(
            [FromBody] SignInRequest request,
            [FromServices] ISignIn signIn)
        {
            await signIn.OnExecute(request);

            return Ok(ApiResponse<object>.Success(
                null!,
                "Login realizado com sucesso.",
                HttpStatusCode.NoContent
            ));
        }

        /// <summary>
        /// Encerra a sessão do usuário autenticado.
        /// </summary>
        [Authorize]
        [HttpPost("sign-out")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<ApiResponse<object>>> SignOut(
            [FromServices] ISignOut signOut)
        {
            await signOut.OnExecute();

            return Ok(ApiResponse<object>.Success(
                null!,
                "Logout realizado com sucesso.",
                HttpStatusCode.NoContent
            ));
        }

        /// <summary>
        /// Gera um novo token de acesso para o usuário.
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<ApiResponse<object>>> RefreshToken(
            [FromServices] IRefresh refresh)
        {
            await refresh.OnExecute();

            return Ok(ApiResponse<object>.Success(
                null!,
                "Sessão atualizada com sucesso.",
                HttpStatusCode.NoContent
            ));
        }
    }
}
