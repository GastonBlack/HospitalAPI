using System.Security.Claims;
using HospitalAPI.Features.Auth.DTOs;
using HospitalAPI.Features.Auth.IServices;
using HospitalAPI.Infrastructure.Auth;
using HospitalAPI.Infrastructure.Data;
using HospitalAPI.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Features.Auth.Services;

public class AuthService : IAuthService
{
    // //////////////////////////////////////////
    // Inyections
    // //////////////////////////////////////////
    private readonly HospitalDbContext _db;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    public AuthService(HospitalDbContext db, JwtTokenGenerator jwtTokenGenerator)
    {
        _db = db;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    // //////////////////////////////////////////
    // Auth
    // //////////////////////////////////////////
    public async Task<LoginResultDto> LoginAsync(LoginDto dto)
    {
        if (dto == null) throw new BadRequestException("Los datos ingresados son invalidos.");

        var normalizedDocument = dto.Document.Trim();

        var admin = await _db.Admins
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Document == normalizedDocument && a.IsActive);

        if (admin != null)
        {
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, admin.PasswordHash))
                throw new UnauthorizedException("Documento o contrasena invalidos.");

            var currentUser = new CurrentUserDto
            {
                Id = admin.Id,
                Document = admin.Document,
                Role = admin.Role,
                FullName = admin.FullName
            };

            var tokenResult = _jwtTokenGenerator.GenerateToken(currentUser);

            return new LoginResultDto
            {
                Token = tokenResult.Token,
                User = currentUser,
                ExpiresAtUtc = tokenResult.ExpiresAtUtc
            };
        }

        var patient = await _db.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Document == normalizedDocument && p.IsActive);

        if (patient != null)
        {
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, patient.PasswordHash))
                throw new UnauthorizedException("Documento o contrasena invalidos.");

            var currentUser = new CurrentUserDto
            {
                Id = patient.Id,
                Document = patient.Document,
                Role = patient.Role,
                FullName = patient.FullName
            };

            var tokenResult = _jwtTokenGenerator.GenerateToken(currentUser);

            return new LoginResultDto
            {
                Token = tokenResult.Token,
                User = currentUser,
                ExpiresAtUtc = tokenResult.ExpiresAtUtc
            };
        }

        var medic = await _db.Medics
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Document == normalizedDocument && m.IsActive);

        if (medic != null)
        {
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, medic.PasswordHash))
                throw new UnauthorizedException("Documento o contrasena invalidos.");

            var currentUser = new CurrentUserDto
            {
                Id = medic.Id,
                Document = medic.Document,
                Role = medic.Role,
                FullName = medic.FullName
            };

            var tokenResult = _jwtTokenGenerator.GenerateToken(currentUser);

            return new LoginResultDto
            {
                Token = tokenResult.Token,
                User = currentUser,
                ExpiresAtUtc = tokenResult.ExpiresAtUtc
            };
        }

        throw new UnauthorizedException("Documento o contrasena invalidos.");
    }

    // //////////////////////////////////////////
    // Current User
    // //////////////////////////////////////////
    public CurrentUserDto GetCurrentUser(ClaimsPrincipal user)
    {
        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
        var nameClaim = user.FindFirst(ClaimTypes.Name)?.Value;
        var documentClaim = user.FindFirst(AuthConstants.DocumentClaimType)?.Value;

        if (string.IsNullOrWhiteSpace(idClaim) ||
            string.IsNullOrWhiteSpace(roleClaim) ||
            string.IsNullOrWhiteSpace(nameClaim) ||
            string.IsNullOrWhiteSpace(documentClaim))
        {
            throw new UnauthorizedException("El token del usuario es invalido.");
        }

        return new CurrentUserDto
        {
            Id = int.Parse(idClaim),
            Role = roleClaim,
            FullName = nameClaim,
            Document = documentClaim
        };
    }
}
