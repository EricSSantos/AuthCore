using AuthCore.Application.Models;
using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.EmailCase.Interface;
using AuthCore.Domain.Aggregates.EmailAggregate;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthCore.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/emails/send")]
    public sealed class EmailController : ControllerBase
    {
        /// <summary>
        /// Envia um e-mail de confirmação de conta com um novo código.
        /// </summary>
        [HttpPost("confirm-email")]
        [ProducesResponseType(typeof(Response<object>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<Response<object>>> SendConfirmEmail(
            [FromBody] EmailRequest request,
            [FromServices] ISendEmail sendEmail)
        {
            var input = new SendEmailRequest
            {
                Email = request.Email,
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
        /// Envia um e-mail de recuperação de senha com um novo código.
        /// </summary>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(Response<object>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<Response<object>>> SendForgotPassword(
            [FromBody] EmailRequest request,
            [FromServices] ISendEmail sendEmail)
        {
            var input = new SendEmailRequest
            {
                Email = request.Email,
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
}
