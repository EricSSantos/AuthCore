using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCore.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController : Controller
    {
        private readonly ISignIn _signIn;
        private readonly ISignOut _signOut;
        private readonly IRefresh _refresh;

        public AuthController(
            ISignIn signIn,
            ISignOut signOut,
            IRefresh refresh)
        {
            _signIn = signIn;
            _signOut = signOut;
            _refresh = refresh;
        }

        /// <summary>
        /// Autentica e inicia uma nova sessão.
        /// </summary>
        [HttpPost("sign-in")]
        public async Task<ActionResult> SignIn(SignInInputModel input)
        {
            await _signIn.OnExecute(input);
            return NoContent();
        }

        /// <summary>
        /// Encerra a sessão.
        /// </summary>
        [Authorize]
        [HttpPost("sign-out")]
        public async Task<ActionResult> SignOut()
        {
            await _signOut.OnExecute();
            return NoContent();
        }

        /// <summary>
        /// Renova o token de acesso.
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken()
        {
            await _refresh.OnExecute();
            return NoContent();
        }
    }
}
