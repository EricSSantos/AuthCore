using AuthCore.Application.Models;
using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        /// Cria um novo usuário.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] AddUserInputModel input)
        {
            await _addUser.OnExecute(input);

            var response = Response<object>.Success(
                null!,
                "Usuário criado com sucesso.",
                HttpStatusCode.Created
            );

            return StatusCode((int)HttpStatusCode.Created, response);
        }
    }
}
