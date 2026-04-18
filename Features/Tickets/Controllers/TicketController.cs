using HospitalAPI.Features.Tickets.DTOs;
using HospitalAPI.Features.Tickets.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Features.Tickets.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("medic/{medicId}")]
    public async Task<IActionResult> GetAllByMedicIdAsync(int medicId)
    {
        return Ok(await _service.GetAllByMedicIdAsync(medicId));
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetAllByPatientIdAsync(int patientId)
    {
        return Ok(await _service.GetAllByPatientIdAsync(patientId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    // //////////////////////////////////////////
    // Mutations
    // //////////////////////////////////////////
    [HttpPost("{patientId}")]
    public async Task<IActionResult> CreateAsync(int patientId, [FromBody] CreateTicketDto dto)
    {
        return Ok(await _service.CreateAsync(patientId, dto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTicketDto dto)
    {
        return Ok(await _service.UpdateAsync(id, dto));
    }
}