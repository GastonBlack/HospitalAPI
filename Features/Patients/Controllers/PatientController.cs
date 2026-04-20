using System.Security.Claims;
using HospitalAPI.Features.Patients.DTOs;
using HospitalAPI.Features.Patients.IServices;
using HospitalAPI.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Features.Patients.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    // //////////////////////////////////////////
    // Inyections
    // //////////////////////////////////////////
    private readonly IPatientService _service;
    public PatientController(IPatientService service)
    {
        _service = service;
    }

    // //////////////////////////////////////////
    // Getters
    // //////////////////////////////////////////
    [Authorize(Roles = $"{AuthRoles.Admin},{AuthRoles.Medic}")]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [Authorize(Roles = $"{AuthRoles.Admin},{AuthRoles.Medic},{AuthRoles.Patient}")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // If a patient tries to see the info of another patient.
        if (User.IsInRole(AuthRoles.Patient) && GetAuthenticatedUserId() != id)
            return Forbid();

        return Ok(await _service.GetByIdAsync(id));
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePatientDto dto)
    {
        return Ok(await _service.CreateAsync(dto));
    }

    [Authorize(Roles = $"{AuthRoles.Admin},{AuthRoles.Patient}")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdatePatientDto dto)
    {
        if (User.IsInRole(AuthRoles.Patient) && GetAuthenticatedUserId() != id)
            return Forbid();

        return Ok(await _service.UpdateAsync(id, dto));
    }

    [Authorize(Roles = $"{AuthRoles.Admin},{AuthRoles.Patient}")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeActiveStatusAsync(int id, [FromBody] DisablePatientDto dto)
    {
        if (User.IsInRole(AuthRoles.Patient) && GetAuthenticatedUserId() != id)
            return Forbid();

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
