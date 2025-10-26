using AuthCore.Application.Models;
using AuthCore.Application.Models.Input;
using AuthCore.Application.Models.Output;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthCore.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/users")]
    public sealed class UsersController : ControllerBase
    {
        /// <summary>
        /// Registra um novo usuário.
        /// </summary>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult<Response<object>>> Add(
            [FromBody] AddUserInputModel input,
            [FromServices] IAddUser addUser)
        {
            await addUser.OnExecute(input);

            return Ok(Response<object>.Success(
                null!,
                "Usuário criado com sucesso.",
                HttpStatusCode.Created
            ));
        }

        /// <summary>
        /// Retorna os dados do usuário atual.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(Response<UserViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Response<UserViewModel>>> Me(
            [FromServices] IGetCurrentUser getCurrentUser)
        {
            var user = await getCurrentUser.OnExecute();

            return Ok(Response<UserViewModel>.Success(
                user,
                "Usuário obtido com sucesso.",
                HttpStatusCode.OK
            ));
        }

        /// <summary>
        /// Altera a senha do usuário atual.
        /// </summary>
        [Authorize]
        [HttpPatch("me/change-password")]
        [ProducesResponseType(typeof(Response<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Response<object>>> ChangePassword(
            [FromBody] ChangePasswordInputModel input,
            [FromServices] IChangePassword changePassword)
        {
            await changePassword.OnExecute(input);

            return Ok(Response<object>.Success(
                null!,
                "Senha alterada com sucesso.",
                HttpStatusCode.OK
            ));
        }

        /// <summary>
        /// Envia um código de recuperação para o e-mail informado.
        /// </summary>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(Response<object>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<Response<object>>> ForgotPassword(
            [FromBody] ForgotPasswordInputModel input,
            [FromServices] IForgotPassword forgotPassword)
        {
            await forgotPassword.OnExecute(input);

            return Accepted(Response<object>.Success(
                null!,
                "Se o e-mail existir, enviaremos instruções de recuperação.",
                HttpStatusCode.Accepted
            ));
        }

        /// <summary>
        /// Redefine a senha usando o código recebido.
        /// </summary>
        [HttpPatch("reset-password")]
        [ProducesResponseType(typeof(Response<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Response<object>>> ResetPassword(
            [FromBody] ResetPasswordInputModel input,
            [FromServices] IResetPassword resetPassword)
        {
            await resetPassword.OnExecute(input);

            return Ok(Response<object>.Success(
                null!,
                "Senha alterada com sucesso.",
                HttpStatusCode.OK
            ));
        }
    }
}
