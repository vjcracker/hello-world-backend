using System.Collections.Concurrent;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HelloWorld.Authentication.Services
{
    public class TokenService
    {
        private readonly ConcurrentDictionary<string, string> _refreshTokens;
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _refreshTokens = new();
            _config = config;
        }

        /// <summary>
        /// Generates a new JWT access token.
        /// </summary>
        public string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(1), // Access token expires in 1 minute
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates a new refresh token.
        /// </summary>
        public string GenerateRefreshToken(string username)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var refreshToken = Convert.ToBase64String(randomNumber);

                // Store the refresh token linked to the user.
                _refreshTokens[username] = refreshToken;

                return refreshToken;
            }
        }
        
        /// <summary>
        /// Validates a refresh token and returns the associated username.
        /// </summary>
        public string ValidateRefreshToken(string token)
        {
            // Simple validation: check if the token exists in our dictionary.
            var user = _refreshTokens.FirstOrDefault(x => x.Value == token).Key;
            return string.IsNullOrEmpty(user) ? null : user;
        }

        /// <summary>
        /// Invalidates a specific refresh token.
        /// </summary>
        public void InvalidateRefreshToken(string token)
        {
            var user = _refreshTokens.FirstOrDefault(x => x.Value == token).Key;
            if (!string.IsNullOrEmpty(user))
            {
                _refreshTokens.Remove(user, out _);
            }
        }
    }
}