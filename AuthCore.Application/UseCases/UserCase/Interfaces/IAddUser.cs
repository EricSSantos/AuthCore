using AuthCore.Application.Models.Input;
using AuthCore.Application.Models.Output;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    public interface IAddUser
    {
        Task<UserViewModel> OnExecute(AddUserInputModel input);
    }
}
