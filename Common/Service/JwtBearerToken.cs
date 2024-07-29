using Common.Interfaces;
using Common.Models.Token;
using Common.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Common.Service
{
    public class JwtBearerToken : IJwtBearerToken
    {
        private readonly JwtTokenOptions jwtTokenOptions;
        public JwtBearerToken(IOptions<JwtTokenOptions> jwtTokenOptions)
        {
            this.jwtTokenOptions = jwtTokenOptions.Value;
        }

        public JwtToken GenerateToken()
        {
            var symmetricSignature = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenOptions.Signature!));
            var signingCredentials = new SigningCredentials(symmetricSignature, SecurityAlgorithms.HmacSha256Signature);

            var securityToken = new JwtSecurityToken(
                jwtTokenOptions.Issuer,
                jwtTokenOptions.Audience,
                null, //claims
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(jwtTokenOptions.AccessTokenExpirationMinutes),
                signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new JwtToken
            {
                Token = token,
                RefreshToken = GenerateRefreshToken()
            };
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[256];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
