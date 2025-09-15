using System.Security.Claims;
using HelloWorld.Authentication.Models;
using HelloWorld.Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserLoginService _userLoginService;
        private readonly TokenService _tokenService;

        public AuthController(UserLoginService userLoginService, TokenService tokenService)
        {
            _tokenService = tokenService;
            _userLoginService = userLoginService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin model)
        {
            if (!_userLoginService.ValidateCredentials(model.Username, model.Password))
            {
                return Unauthorized(new { message = "Invalid User name or Password" });
            }

            var accessToken = _tokenService.GenerateJwtToken(model.Username);
            var refreshToken = _tokenService.GenerateRefreshToken(model.Username);

            return Ok(new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var username = _tokenService.ValidateRefreshToken(request.RefreshToken);

            if (String.IsNullOrEmpty(username))
            {
                _tokenService.InvalidateRefreshToken(request.RefreshToken);
                return Unauthorized(new { message = "Invalid refresh token. Please login again" });
            }

            _tokenService.InvalidateRefreshToken(request.RefreshToken);
            var accessToken = _tokenService.GenerateJwtToken(username);
            var refreshToken = _tokenService.GenerateRefreshToken(username);

            return Ok(new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult ProtectedResource()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var username = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == claimsIdentity.NameClaimType)?.Value;
            return Ok(new { message = $"Hello from {username}! You have successfully accessed a protected resource" });
        }

        [HttpPost("logout")]
        public IActionResult Logout(RefreshTokenRequest request)
        {
            _tokenService.InvalidateRefreshToken(request.RefreshToken);
            return Ok(new { message = "Logged out successfully" });
        }
    }
}