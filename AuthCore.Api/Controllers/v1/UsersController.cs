using AuthCore.Application.Models;
using AuthCore.Application.Models.Requests;
using AuthCore.Application.Models.Responses;
using AuthCore.Application.UseCases.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthCore.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/users")]
    public sealed class UsersController : ControllerBase
    {
        /// <summary>Registra um novo usuário.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> Register(
            [FromBody] AddUserRequest request,
            [FromServices] IAddUser addUser)
        {
            await addUser.OnExecuteAsync(request);

            return Accepted(ApiResponse<object>.Success(
                null!,
                "Cadastro realizado com sucesso! Enviamos um e-mail com o código de confirmação da conta.",
                HttpStatusCode.Accepted
            ));
        }

        /// <summary>Retorna o usuário autenticado.</summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetCurrent(
            [FromServices] IGetCurrentUser getCurrentUser)
        {
            var user = await getCurrentUser.OnExecuteAsync();

            return Ok(ApiResponse<UserResponse>.Success(
                user,
                "Usuário obtido com sucesso.",
                HttpStatusCode.OK
            ));
        }

        /// <summary>Atualiza a senha do usuário autenticado.</summary>
        [Authorize]
        [HttpPatch("me/change-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> UpdatePassword(
            [FromBody] ChangePasswordRequest request,
            [FromServices] IChangePassword changePassword)
        {
            await changePassword.OnExecuteAsync(request);

            return Ok(ApiResponse<object>.Success(
                null!,
                "Senha alterada com sucesso.",
                HttpStatusCode.OK
            ));
        }

        /// <summary>Operação para confirmar o e-mail do usuário.</summary>
        /// <remarks>Retorna 202 mesmo quando o e-mail não existe para evitar enumeração.</remarks>
        [HttpPatch("confirm-email")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> ConfirmEmail(
            [FromBody] ConfirmEmailRequest request,
            [FromServices] IConfirmEmail confirmEmail)
        {
            await confirmEmail.OnExecuteAsync(request);

            return Accepted(ApiResponse<object>.Success(
                null!,
                "E-mail confirmado com sucesso.",
                HttpStatusCode.Accepted
            ));
        }

        /// <summary>Operação para redefinir a senha do usuário.</summary>
        /// <remarks>Retorna 202 mesmo quando o e-mail não existe para evitar enumeração.</remarks>
        [HttpPatch("reset-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword(
            [FromBody] ResetPasswordRequest request,
            [FromServices] IResetPassword resetPassword)
        {
            await resetPassword.OnExecuteAsync(request);

            return Accepted(ApiResponse<object>.Success(
                null!,
                "Senha redefinida com sucesso.",
                HttpStatusCode.Accepted
            ));
        }
    }
}
