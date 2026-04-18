using System.Security.Claims;
using HospitalAPI.Features.Auth.DTOs;

namespace HospitalAPI.Features.Auth.IServices;

public interface IAuthService
{
    Task<LoginResultDto> LoginAsync(LoginDto dto);
    CurrentUserDto GetCurrentUser(ClaimsPrincipal user);
}
