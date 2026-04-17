using HospitalAPI.Features.Patients.DTOs;
using HospitalAPI.Features.Patients.IServices;
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
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePatientDto dto)
    {
        return Ok(await _service.CreateAsync(dto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdatePatientDto dto)
    {
        return Ok(await _service.UpdateAsync(id, dto));
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeActiveStatusAsync(int id, [FromBody] DisablePatientDto dto)
    {
        return Ok(await _service.ChangeActiveStatusAsync(id, dto));
    } 
}
