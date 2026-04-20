using System.Security.Claims;
using HospitalAPI.Features.Tickets.DTOs;
using HospitalAPI.Features.Tickets.IServices;
using HospitalAPI.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Features.Tickets.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketController : ControllerBase
{
    // //////////////////////////////////////////
    // Inyections
    // //////////////////////////////////////////
    private readonly ITicketService _service;
    public TicketController(ITicketService service)
    {
        _service = service;
    }

    // //////////////////////////////////////////
    // Getters
    // //////////////////////////////////////////
    [Authorize(Roles = AuthRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync() // ToDo: Add optional search filters for admin.
    {
        return Ok(await _service.GetAllAsync());
    }

    [Authorize(Roles = AuthRoles.Medic)]
    [HttpGet("medic/{medicId}")]
    public async Task<IActionResult> GetAllByMedicIdAsync(int medicId)
    {
        if (GetAuthenticatedUserId() != medicId)
            return Forbid();

        return Ok(await _service.GetAllByMedicIdAsync(medicId));
    }

    [Authorize(Roles = AuthRoles.Patient)]
    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetAllByPatientIdAsync(int patientId)
    {
        if (GetAuthenticatedUserId() != patientId)
            return Forbid();

        return Ok(await _service.GetAllByPatientIdAsync(patientId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var ticket = await _service.GetByIdAsync(id);

        if (User.IsInRole(AuthRoles.Patient) && ticket!.PatientId != GetAuthenticatedUserId())
            return Forbid();

        if (User.IsInRole(AuthRoles.Medic) && ticket!.MedicId != GetAuthenticatedUserId())
            return Forbid();

        return Ok(ticket);
    }

    // //////////////////////////////////////////
    // Mutations
    // //////////////////////////////////////////
    [Authorize(Roles = AuthRoles.Patient)]
    [HttpPost("{patientId}")]
    public async Task<IActionResult> CreateAsync(int patientId, [FromBody] CreateTicketDto dto)
    {
        if (GetAuthenticatedUserId() != patientId)
            return Forbid();

        return Ok(await _service.CreateAsync(patientId, dto));
    }

    [Authorize(Roles = $"{AuthRoles.Admin},{AuthRoles.Medic}")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTicketDto dto)
    {
        if (User.IsInRole(AuthRoles.Medic) && dto.MedicId != GetAuthenticatedUserId())
            return Forbid();

        var existingTicket = await _service.GetByIdAsync(id);
        if (User.IsInRole(AuthRoles.Medic) && existingTicket!.MedicId != GetAuthenticatedUserId())
            return Forbid();

        return Ok(await _service.UpdateAsync(id, dto));
    }

    // //////////////////////////////////////////
    // Helpers
    // //////////////////////////////////////////
    private int GetAuthenticatedUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
