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
        /// Autentica o usuário e inicia uma nova sessão.
        /// </summary>
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInInputModel input)
        {
            await _signIn.OnExecute(input);
            return NoContent();
        }

        /// <summary>
        /// Encerra a sessão do usuário autenticado.
        /// </summary>
        [Authorize]
        [HttpPost("sign-out")]
        public async Task<IActionResult> SignOut()
        {
            await _signOut.OnExecute();
            return NoContent();
        }

        /// <summary>
        /// Renova o token de acesso do usuário.
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            await _refresh.OnExecute();
            return NoContent();
        }
    }
}
