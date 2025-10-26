using AuthCore.Application.Models;
using AuthCore.Application.Models.Input;
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
        /// Faz login e inicia uma nova sessão.
        /// </summary>
        [HttpPost("sign-in")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<Response<object>>> SignIn(
            [FromBody] SignInInputModel input,
            [FromServices] ISignIn signIn)
        {
            await signIn.OnExecute(input);

            return Ok(Response<object>.Success(
                null!,
                "Login realizado com sucesso.",
                HttpStatusCode.NoContent
            ));
        }

        /// <summary>
        /// Faz logout e encerra a sessão atual.
        /// </summary>
        [Authorize]
        [HttpPost("sign-out")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<Response<object>>> SignOut(
            [FromServices] ISignOut signOut)
        {
            await signOut.OnExecute();

            return Ok(Response<object>.Success(
                null!,
                "Logout realizado com sucesso.",
                HttpStatusCode.NoContent
            ));
        }

        /// <summary>
        /// Atualiza o token de acesso do usuário.
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<Response<object>>> RefreshToken(
            [FromServices] IRefresh refresh)
        {
            await refresh.OnExecute();

            return Ok(Response<object>.Success(
                null!,
                "Token atualizado com sucesso.",
                HttpStatusCode.NoContent
            ));
        }
    }
}
