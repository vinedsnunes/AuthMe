using AuthMe.Application.DTOs.Request;
using AuthMe.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthMe.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private IIdentityService _identityService;
        public UserController(IIdentityService identityService) =>
            _identityService = identityService;

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(UserSignUpRequest request)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var result = await _identityService.SignUp(request);
            if(result.Success)
                return Ok(result);
            //else if(result.Errors.Count()  > 0)

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
