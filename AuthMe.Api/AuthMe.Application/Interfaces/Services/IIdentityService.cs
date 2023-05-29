using AuthMe.Application.DTOs.Request;
using AuthMe.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthMe.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<UserSignUpResponse> SignUp(UserSignUpRequest request);
    }
}
