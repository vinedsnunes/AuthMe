using AuthMe.Application.DTOs.Request;
using AuthMe.Application.DTOs.Response;

namespace AuthMe.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<UserSignUpResponse> SignUp(UserSignUpRequest request);
        Task<UserSignInResponse> SignIn(UserSignInRequest request);
        Task SignOut(string userId);
        Task<UserSignInResponse> SignInWithoutPassword(string userId);
    }
}
