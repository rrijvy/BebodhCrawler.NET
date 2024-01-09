using Core.Data;
using Core.Models;

namespace Core.IServices
{
    public interface IAuthService
    {
        Task<bool> RegisterUser(RegisterUserRequestModel user);
        Task<SignInResponseModel> SignIn(SignInRequestModel request);
    }
}
