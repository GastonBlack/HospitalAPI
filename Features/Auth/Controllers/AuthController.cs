using HospitalAPI.Features.Auth.DTOs;
using HospitalAPI.Features.Auth.IServices;
using HospitalAPI.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Features.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // //////////////////////////////////////////
    // Inyections
    // //////////////////////////////////////////
    private readonly IAuthService _service;
    private readonly AuthCookieFactory _authCookieFactory;
    public AuthController(IAuthService service, AuthCookieFactory authCookieFactory)
    {
        _service = service;
        _authCookieFactory = authCookieFactory;
    }

    // //////////////////////////////////////////
    // Auth
    // //////////////////////////////////////////
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
    {
        var result = await _service.LoginAsync(dto);

        Response.Cookies.Append(
            AuthConstants.AccessTokenCookieName,
            result.Token,
            _authCookieFactory.Create(result.ExpiresAtUtc));

        return Ok(new AuthResponseDto
        {
            User = result.User,
            ExpiresAtUtc = result.ExpiresAtUtc
        });
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(
            AuthConstants.AccessTokenCookieName,
            _authCookieFactory.Create());

        return Ok(new { message = "Sesion cerrada correctamente." });
    }

    // //////////////////////////////////////////
    // Current User
    // //////////////////////////////////////////
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(_service.GetCurrentUser(User));
    }
}