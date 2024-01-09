using Core.Data;
using Core.IServices;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterUserRequestModel user)
        {
            var isSucceeded = await _authService.RegisterUser(user);
            return Ok(isSucceeded);
        }

        [HttpPost("SignIn")]
        public async Task<ActionResult> SignIn(SignInRequestModel request)
        {
            var result = await _authService.SignIn(request);
            return Ok(result);
        }

    }
}
