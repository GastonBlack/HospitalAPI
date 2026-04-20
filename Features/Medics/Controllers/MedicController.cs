using System.Security.Claims;
using HospitalAPI.Features.Medics.DTOs;
using HospitalAPI.Features.Medics.IServices;
using HospitalAPI.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Features.Medics.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MedicController : ControllerBase
{
    // //////////////////////////////////////////
    // Inyections
    // //////////////////////////////////////////
    private readonly IMedicService _service;
    public MedicController(IMedicService service)
    {
        _service = service;
    }

    // //////////////////////////////////////////
    // Getters
    // //////////////////////////////////////////
    [Authorize(Roles = $"{AuthRoles.Admin},{AuthRoles.Patient}")]
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableForTicketsAsync()
    {
        return Ok(await _service.GetAvailableForTicketsAsync());
    }

    [Authorize(Roles = AuthRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [Authorize(Roles = $"{AuthRoles.Admin},{AuthRoles.Medic}")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        // A medic can not see other medic's info.
        if (User.IsInRole(AuthRoles.Medic) && GetAuthenticatedUserId() != id)
            return Forbid();

        return Ok(await _service.GetByIdAsync(id));
    }

    // //////////////////////////////////////////
    // Mutations
    // //////////////////////////////////////////
    // Only an Administrator can create a Medic.
    [Authorize(Roles = AuthRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMedicDto dto)
    {
        return Ok(await _service.CreateAsync(dto));
    }

    [Authorize(Roles = $"{AuthRoles.Admin},{AuthRoles.Medic}")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateMedicDto dto)
    {
        if (User.IsInRole(AuthRoles.Medic) && GetAuthenticatedUserId() != id)
            return Forbid();

        return Ok(await _service.UpdateAsync(id, dto));
    }

    [Authorize(Roles = AuthRoles.Admin)]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeActiveStatusAsync(int id, [FromBody] DisableMedicDto dto)
    {
        return Ok(await _service.ChangeActiveStatusAsync(id, dto));
    }

    // //////////////////////////////////////////
    // Helpers
    // //////////////////////////////////////////
    private int GetAuthenticatedUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
