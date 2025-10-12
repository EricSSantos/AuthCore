using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthCore.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/users")]
    public sealed class UsersController : Controller
    {
        private readonly IAddUser _addUser;

        public UsersController(IAddUser addUser)
        {
            _addUser = addUser;
        }

        /// <summary>
        /// Cria um novo usuário no sistema
        /// </summary>
        [HttpPost()]
        public async Task<IActionResult> Add([FromBody] AddUserInputModel input)
        {
            await _addUser.OnExecute(input);
            return Ok(new { Message = "Usuário criado com sucesso." });
        }
    }
}
