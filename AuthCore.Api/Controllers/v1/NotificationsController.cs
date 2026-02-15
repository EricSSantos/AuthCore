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
        [HttpPost("{email}/confirmation")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<ApiResponse<object>>> SendConfirmationEmail(
            [FromRoute] string email,
            [FromServices] ISendNotification sendNotification)
        {
            var input = new SendNotificationRequest
            {
                Email = email,
                Type = NotificationType.ConfirmEmail
            };

            await sendNotification.OnExecuteAsync(input);

            return Accepted(ApiResponse<object>.Success(
                null!,
                "Se o e-mail informado for válido, reenviamos o código de confirmação.",
                HttpStatusCode.Accepted
            ));
        }

        /// <summary>Operação para enviar o e-mail de recuperação de senha.</summary>
        [HttpPost("{email}/forgot-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<ApiResponse<object>>> SendPasswordRecoveryEmail(
            [FromRoute] string email,
            [FromServices] ISendNotification sendNotification)
        {
            var input = new SendNotificationRequest
            {
                Email = email,
                Type = NotificationType.ForgotPassword
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
