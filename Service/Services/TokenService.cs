using Microsoft.IdentityModel.Tokens;
using Repository.models;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class TokenService : ITokenService
    {
        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKeyMustBeAtLeast32CharactersLong"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // ה-Claims הם המידע ש"מוצפן" בתוך הטוקן
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                
                // השורה הקריטית: מוסיפה את סוג המשתמש לתוך הטוקן
                // וודאי שהשדה במודל שלך נקרא UserType או Role
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "YourIssuer",
                audience: "YourAudience",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
