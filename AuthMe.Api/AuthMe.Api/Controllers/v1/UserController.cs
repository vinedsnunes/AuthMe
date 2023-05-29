using AuthMe.Application.DTOs.Request;
using AuthMe.Application.DTOs.Response;
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

        /// <summary>
        /// Cadastro de usuário.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="request">Dados de cadastro do usuário</param>
        /// <returns></returns>
        /// <response code="200">Usuário criado com sucesso</response>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="500">Retorna erros caso ocorram</response>
        [ProducesResponseType(typeof(UserSignUpResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(UserSignUpRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _identityService.SignUp(request);
            if (result.Success)
                return Ok(result);
            //else if(result.Errors.Count()  > 0)

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Login do usuário via usuário/senha.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="request">Dados de login do usuário</param>
        /// <returns></returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="401">Erro caso usuário não esteja autorizado</response>
        /// <response code="500">Retorna erros caso ocorram</response>
        [ProducesResponseType(typeof(UserSignInResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost("sign-in")]
        public async Task<ActionResult<UserSignInResponse>> Login(UserSignInRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _identityService.SignIn(request);
            if (result.Success)
                return Ok(result);

            return Unauthorized();
        }

    }
}
