using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VASAPI_Azure.Infrastructure;
using VASAPI_Azure.Models;
using VASAPI_Azure.Services;

namespace VASAPI_Azure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginUserController : ControllerBase
    {
        private readonly IJWTAuthManager _jwtAuthManager;
        private readonly ILoginUserService _loginUserService;

        public LoginUserController(IJWTAuthManager jwtAuthManager, ILoginUserService loginUserService)
        {
            _jwtAuthManager = jwtAuthManager;
            _loginUserService = loginUserService;
        }

        // POST: api/LoginUser
        [HttpPost("Login")]
        public ActionResult<JWTAuthResult> Login([FromBody] LoginUser user)
        {
            var loginUser = _loginUserService.GetLoginUser(user.Username, user.Password);

            if (loginUser == null)
            {
                return Unauthorized();
            }

            var jwtAuthResult = _jwtAuthManager.GenerateTokens(loginUser);

            return Ok(jwtAuthResult);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<JWTAuthResult>> RefreshToken([FromBody] string refreshToken)
        {
            var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var jwtResult = _jwtAuthManager.RefreshTokens(accessToken, refreshToken);
            return Ok(jwtResult);
        }
    }
}
