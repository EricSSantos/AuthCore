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
    public sealed class UsersController : Controller
    {
        private readonly IAddUser _addUser;
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IForgotPassword _forgotPassword;
        private readonly IResetPassword _resetPassword;
        private readonly IChangePassword _changePassword;

        public UsersController(
            IAddUser addUser,
            IGetCurrentUser getCurrentUser,
            IForgotPassword forgotPassword,
            IResetPassword resetPassword,
            IChangePassword changePassword)
        {
            _addUser = addUser;
            _getCurrentUser = getCurrentUser;
            _forgotPassword = forgotPassword;
            _resetPassword = resetPassword;
            _changePassword = changePassword;
        }

        [HttpPost]
        public async Task<ActionResult<Response<object>>> Add([FromBody] AddUserInputModel input)
        {
            await _addUser.OnExecute(input);

            var response = Response<object>.Success(
                null!,
                "Usuário criado com sucesso.",
                HttpStatusCode.Created
            );

            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<Response<UserViewModel>>> Me()
        {
            var user = await _getCurrentUser.OnExecute();

            var response = Response<UserViewModel>.Success(
                user,
                "Usuário obtido com sucesso.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }

        [Authorize]
        [HttpPatch("me/change-password")]
        public async Task<ActionResult<Response<object>>> ChangePassword([FromBody] ChangePasswordInputModel input)
        {
            await _changePassword.OnExecute(input);

            var response = Response<object>.Success(
                null!,
                "Senha alterada com sucesso.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<Response<object>>> ForgotPassword([FromBody] ForgotPasswordInputModel input)
        {
            await _forgotPassword.OnExecute(input);

            var response = Response<object>.Success(
                null!,
                "Se o e-mail existir, enviaremos instruções de recuperação.",
                HttpStatusCode.Accepted
            );

            return Accepted(response);
        }

        [HttpPatch("reset-password")]
        public async Task<ActionResult<Response<object>>> ResetPassword([FromBody] ResetPasswordInputModel input)
        {
            await _resetPassword.OnExecute(input);

            var response = Response<object>.Success(
                null!,
                "Senha alterada com sucesso.",
                HttpStatusCode.OK
            );

            return Ok(response);
        }
    }
}
