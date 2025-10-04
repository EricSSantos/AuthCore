using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.AuthCase.SignIn;
using Microsoft.AspNetCore.Mvc;

namespace AuthCore.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController : Controller
    {
        private readonly ISigIn _sigIn;

        public AuthController(ISigIn sigIn)
        {
            _sigIn = sigIn;
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(SigInInputModel input)
        {
            var result = await _sigIn.OnExecute(input);
            return NoContent();
        }
    }
}
