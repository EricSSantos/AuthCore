using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Add;
using Microsoft.AspNetCore.Mvc;

namespace AuthCore.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : Controller
    {
        private readonly IAddUser _addUser;

        public UsersController(IAddUser addUser)
        {
            _addUser = addUser;
        }

        [HttpPost()]
        public async Task<IActionResult> Add([FromBody] AddUserInputModel input)
        {
            await _addUser.OnExecute(input);
            return Ok(new { Message = "Usuário criado com sucesso." });
        }
    }
}
