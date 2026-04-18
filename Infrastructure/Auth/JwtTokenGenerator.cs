using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HospitalAPI.Features.Auth.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HospitalAPI.Infrastructure.Auth;

public class JwtTokenGenerator
{
    // //////////////////////////////////////////
    // Inyections
    // //////////////////////////////////////////
    private readonly JwtOptions _jwtOptions;
    public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    // //////////////////////////////////////////
    // Token
    // //////////////////////////////////////////
    public (string Token, DateTime ExpiresAtUtc) GenerateToken(CurrentUserDto user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role),
            new(ClaimTypes.Name, user.FullName),
            new(AuthConstants.DocumentClaimType, user.Document)
        };

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return (token, expiresAtUtc);
    }
}
