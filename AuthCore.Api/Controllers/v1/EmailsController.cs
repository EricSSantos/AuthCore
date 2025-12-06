using AuthCore.Application.Models;
using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.EmailCase.Interface;
using AuthCore.Domain.Aggregates.MessagingAggregate;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/v1/emails")]
public sealed class EmailsController : ControllerBase
{
    /// <summary>
    /// Envia o e-mail de confirmação de conta.
    /// </summary>
    [HttpPost("{email}/confirmation")]
    [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Accepted)]
    public async Task<ActionResult<ApiResponse<object>>> SendConfirmationEmail(
        [FromRoute] string email,
        [FromServices] ISendEmail sendEmail)
    {
        var input = new SendEmailRequest
        {
            Email = email,
            Type = MessagingType.ConfirmEmail
        };

        await sendEmail.OnExecuteAsync(input);

        return Accepted(ApiResponse<object>.Success(
            null!,
            "Se o e-mail informado for válido, reenviamos o código de confirmação.",
            HttpStatusCode.Accepted
        ));
    }

    /// <summary>
    /// Envia o e-mail de recuperação de senha.
    /// </summary>
    [HttpPost("{email}/forgot-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Accepted)]
    public async Task<ActionResult<ApiResponse<object>>> SendPasswordRecoveryEmail(
        [FromRoute] string email,
        [FromServices] ISendEmail sendEmail)
    {
        var input = new SendEmailRequest
        {
            Email = email,
            Type = MessagingType.ForgotPassword
        };

        await sendEmail.OnExecuteAsync(input);

        return Accepted(ApiResponse<object>.Success(
            null!,
            "Se o e-mail informado for válido, reenviamos o código de recuperação.",
            HttpStatusCode.Accepted
        ));
    }
}
