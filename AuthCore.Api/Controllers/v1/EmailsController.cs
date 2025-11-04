using AuthCore.Application.Models;
using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.EmailCase.Interface;
using AuthCore.Domain.Aggregates.EmailAggregate;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/v1/emails")]
public sealed class EmailsController : ControllerBase
{
    /// <summary>
    /// Envia um novo e-mail de confirmação de conta.
    /// </summary>
    [HttpPost("{email}/confirmation")]
    [ProducesResponseType(typeof(Response<object>), (int)HttpStatusCode.Accepted)]
    public async Task<ActionResult<Response<object>>> SendConfirmationEmail(
        [FromRoute] string email,
        [FromServices] ISendEmail sendEmail)
    {
        var input = new SendEmailRequest
        {
            Email = email,
            Type = EmailType.ConfirmEmail
        };

        await sendEmail.OnExecute(input);

        return Accepted(Response<object>.Success(
            null!,
            "Se o e-mail informado for válido, reenviamos o código de confirmação.",
            HttpStatusCode.Accepted
        ));
    }

    /// <summary>
    /// Envia um novo e-mail de recuperação de senha.
    /// </summary>
    [HttpPost("{email}/forgot-password")]
    [ProducesResponseType(typeof(Response<object>), (int)HttpStatusCode.Accepted)]
    public async Task<ActionResult<Response<object>>> SendPasswordRecoveryEmail(
        [FromRoute] string email,
        [FromServices] ISendEmail sendEmail)
    {
        var input = new SendEmailRequest
        {
            Email = email,
            Type = EmailType.ForgotPassword
        };

        await sendEmail.OnExecute(input);

        return Accepted(Response<object>.Success(
            null!,
            "Se o e-mail informado for válido, reenviamos o código de recuperação.",
            HttpStatusCode.Accepted
        ));
    }
}
