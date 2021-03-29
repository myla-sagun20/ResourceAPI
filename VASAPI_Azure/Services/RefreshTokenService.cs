using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using VASAPI_Azure.Infrastructure;
using VASAPI_Azure.Models;

namespace VASAPI_Azure.Services
{
    public interface IRefreshTokenService
    {
        RefreshToken CreateRefreshToken(LoginUser user);
        IEnumerable<RefreshToken> GetRefreshTokenByUser(int userId);
    }

    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly VASContext _context;
        private readonly JwtTokenConfig _jwtTokenConfig;

        public RefreshTokenService(VASContext context, JwtTokenConfig jwtTokenConfig)
        {
            _context = context;
            _jwtTokenConfig = jwtTokenConfig;
        }

        public RefreshToken CreateRefreshToken(LoginUser user)
        {
            RefreshToken refreshToken = new RefreshToken();
            refreshToken.Token = GetRefreshTokenString();
            refreshToken.ExpiryDate = DateTime.UtcNow.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration);

            //1. or
            //refreshToken.Loginuser = user;
            //_context.RefreshTokens.Add(refreshToken);

            //2.
            user.RefreshTokens.Add(refreshToken);

            _context.SaveChanges();

            return refreshToken;
        }

        public IEnumerable<RefreshToken> GetRefreshTokenByUser(int userId)
        {
            return _context.RefreshTokens.Where(r => r.LoginuserId == userId);
        }

        private string GetRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }


    }
}
