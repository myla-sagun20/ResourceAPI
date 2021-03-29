using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VASAPI_Azure.Models;
using VASAPI_Azure.Services;

namespace VASAPI_Azure.Infrastructure
{
    public interface IJWTAuthManager
    {
        JWTAuthResult GenerateTokens(LoginUser user);

        JWTAuthResult RefreshTokens(string accessToken, string refreshToken);

        void RemoveRefreshToken(int userId);

        (ClaimsPrincipal, JwtSecurityToken) GetClaimsPrincipalAndToken(string token);
    }

    public class JWTAuthManager : IJWTAuthManager
    {
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly VASContext _context;
        private readonly byte[] _secretKey;
        private readonly ILoginUserService _loginUserService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<JWTAuthManager> _logger;

        public JWTAuthManager(ILogger<JWTAuthManager> logger, JwtTokenConfig jwtTokenConfig, VASContext context, 
            ILoginUserService loginUserService,
            IRefreshTokenService refreshTokenService)
        {
            _logger = logger;
            _jwtTokenConfig = jwtTokenConfig;
            _context = context;
            _secretKey = Encoding.ASCII.GetBytes(_jwtTokenConfig.Secret);
            _loginUserService = loginUserService;
            _refreshTokenService = refreshTokenService;
        }

        public JWTAuthResult GenerateTokens(LoginUser user)
        {
            JWTAuthResult jwtAuthResult = new JWTAuthResult();
            
            //create accessToken
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(new Claim[] { 
                    new Claim(ClaimTypes.Name, Convert.ToString(user.Id))
                }),
                Expires = DateTime.Now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            //create refresh token 
            RefreshToken refreshToken = _refreshTokenService.CreateRefreshToken(user);

            //set tokens
            jwtAuthResult.AccessToken = accessToken;
            jwtAuthResult.RefreshToken = refreshToken.Token;

            return jwtAuthResult;
        }

        public JWTAuthResult RefreshTokens(string accessToken, string refreshToken)
        {
            //(ClaimsPrincipal, JwtSecurityToken) tuple = DecodeJwtToken(accessToken);
            //if (tuple.Item1 == null || !tuple.Item1.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))

            var (principal, jwtToken) = GetClaimsPrincipalAndToken(accessToken);
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userId = principal.FindFirst(ClaimTypes.Name)?.Value;
            var user = _loginUserService.GetLoginUserById(Convert.ToInt32(userId));

            if (user == null)
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userRefreshToken = user.RefreshTokens.FirstOrDefault(r => r.Token == refreshToken && r.ExpiryDate > DateTime.UtcNow);

            if (userRefreshToken == null)
            {
                var refreshTokens = _refreshTokenService.GetRefreshTokenByUser(user.Id);

                if (refreshTokens != null)
                {
                    var validRefreshToken = refreshTokens.FirstOrDefault(r => r.Token == refreshToken && r.ExpiryDate > DateTime.UtcNow);

                    if (validRefreshToken == null)
                    {
                        throw new SecurityTokenException("Invalid token");
                    }

                }

                if (refreshTokens == null)
                {
                    throw new SecurityTokenException("Invalid token");
                }

                
            }

            var jwtAuthResult = GenerateTokens(user);

            return jwtAuthResult;
        }

        public void RemoveRefreshToken(int userId)
        {
            var user = _loginUserService.GetLoginUserById(userId);

            if (user != null && user.RefreshTokens.Any())
            {
                foreach (var userRefreshToken in user.RefreshTokens)
                {
                    _context.RefreshTokens.Remove(userRefreshToken);
                }

                _context.SaveChanges();
            }
            else
            {
                _logger.LogInformation($"User [{userId}] is not found");
            }

        }

        public (ClaimsPrincipal, JwtSecurityToken) GetClaimsPrincipalAndToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters 
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_secretKey),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            return (principal, securityToken as JwtSecurityToken);
        }

    }

    public class JWTAuthResult
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
