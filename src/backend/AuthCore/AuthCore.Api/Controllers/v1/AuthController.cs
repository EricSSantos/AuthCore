using AuthCore.Application.Models;
using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthCore.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController : ControllerBase
    {
        /// <summary>Operação para autenticar o usuário e iniciar sessão.</summary>
        [HttpPost("sign-in")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResponse<object>>> SignIn(
            [FromBody] SignInRequest request,
            [FromServices] ISignIn signIn)
        {
            await signIn.OnExecuteAsync(request);

            return Ok(ApiResponse<object>.Success(
                data: null!,
                title: "Login realizado com sucesso.",
                statusCode: HttpStatusCode.OK
            ));
        }

        /// <summary>Operação para encerrar a sessão atual do usuário.</summary>
        [Authorize]
        [HttpPost("sign-out")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResponse<object>>> SignOut(
            [FromServices] ISignOut signOut)
        {
            await signOut.OnExecuteAsync();

            return Ok(ApiResponse<object>.Success(
                data: null!,
                title: "Logout realizado com sucesso.",
                statusCode: HttpStatusCode.OK
            ));
        }

        /// <summary>Operação para renovar o token de acesso do usuário.</summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResponse<object>>> RefreshToken(
            [FromServices] IRefresh refresh)
        {
            await refresh.OnExecuteAsync();

            return Ok(ApiResponse<object>.Success(
                data: null!,
                title: "Sessão atualizada com sucesso.",
                statusCode: HttpStatusCode.OK
            ));
        }
    }
}
