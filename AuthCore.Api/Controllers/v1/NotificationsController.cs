using AuthCore.Application.Models;
using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.Notifications.Interfaces;
using AuthCore.Domain.Aggregates.Notifications;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthCore.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/notifications")]
    public sealed class NotificationsController : ControllerBase
    {
        /// <summary>Operação para enviar o e-mail de confirmação de conta.</summary>
        /// <remarks>Retorna 202 mesmo quando o e-mail não existe para evitar enumeração.</remarks>
        [HttpPost("confirmation")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> SendConfirmationEmail(
            [FromBody] EmailRequest request,
            [FromServices] ISendNotification sendNotification)
        {
            var input = new SendNotificationRequest
            {
                Email = request.Email,
                Type = NotificationType.ConfirmEmail,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            await sendNotification.OnExecuteAsync(input);

            return Accepted(ApiResponse<object>.Success(
                null!,
                "Se o e-mail informado for válido, reenviamos o código de confirmação.",
                HttpStatusCode.Accepted
            ));
        }

        /// <summary>Operação para enviar o e-mail de recuperação de senha.</summary>
        /// <remarks>Retorna 202 mesmo quando o e-mail não existe para evitar enumeração.</remarks>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> SendPasswordRecoveryEmail(
            [FromBody] EmailRequest request,
            [FromServices] ISendNotification sendNotification)
        {
            var input = new SendNotificationRequest
            {
                Email = request.Email,
                Type = NotificationType.ForgotPassword,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            await sendNotification.OnExecuteAsync(input);

            return Accepted(ApiResponse<object>.Success(
                null!,
                "Se o e-mail informado for válido, reenviamos o código de recuperação.",
                HttpStatusCode.Accepted
            ));
        }
    }
}
