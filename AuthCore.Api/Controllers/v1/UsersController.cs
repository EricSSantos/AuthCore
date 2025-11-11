using AuthCore.Application.Models;
using AuthCore.Application.Models.Requests;
using AuthCore.Application.Models.Responses;
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
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<ApiResponse<object>>> Register(
            [FromBody] AddUserRequest request,
            [FromServices] IAddUser addUser)
        {
            await addUser.OnExecute(request);

            return Created(string.Empty, ApiResponse<object>.Success(
                null!,
                "Cadastro realizado com sucesso! Enviamos um e-mail com o código de confirmação da conta.",
                HttpStatusCode.Created
            ));
        }

        /// <summary>
        /// Retorna os dados do usuário autenticado.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetCurrent(
            [FromServices] IGetCurrentUser getCurrentUser)
        {
            var user = await getCurrentUser.OnExecute();

            return Ok(ApiResponse<UserResponse>.Success(
                user,
                "Usuário obtido com sucesso.",
                HttpStatusCode.OK
            ));
        }

        /// <summary>
        /// Atualiza a senha do usuário autenticado.
        /// </summary>
        [Authorize]
        [HttpPatch("me/change-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResponse<object>>> UpdatePassword(
            [FromBody] ChangePasswordRequest request,
            [FromServices] IChangePassword changePassword)
        {
            await changePassword.OnExecute(request);

            return Ok(ApiResponse<object>.Success(
                null!,
                "Senha alterada com sucesso.",
                HttpStatusCode.OK
            ));
        }

        /// <summary>
        /// Confirma o endereço de e-mail de um usuário pelo código de verificação.
        /// </summary>
        [HttpPatch("{email}/confirm")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResponse<object>>> ConfirmEmail(
            [FromRoute] string email,
            [FromBody] ConfirmEmailRequest request,
            [FromServices] IConfirmEmail confirmEmail)
        {
            request.Email = email;

            await confirmEmail.OnExecute(request);

            return Ok(ApiResponse<object>.Success(
                null!,
                "E-mail confirmado com sucesso.",
                HttpStatusCode.OK
            ));
        }

        /// <summary>
        /// Redefine a senha de um usuário usando o código de verificação recebido por e-mail.
        /// </summary>
        [HttpPatch("{email}/reset-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword(
            [FromRoute] string email,
            [FromBody] ResetPasswordRequest request,
            [FromServices] IResetPassword resetPassword)
        {
            request.Email = email;

            await resetPassword.OnExecute(request);

            return Ok(ApiResponse<object>.Success(
                null!,
                "Senha redefinida com sucesso.",
                HttpStatusCode.OK
            ));
        }
    }
}
