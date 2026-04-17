using HospitalAPI.Features.Medics.DTOs;
using HospitalAPI.Features.Medics.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Features.Medics.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    // //////////////////////////////////////////
    // Mutations
    // //////////////////////////////////////////
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMedicDto dto)
    {
        return Ok(await _service.CreateAsync(dto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateMedicDto dto)
    {
        return Ok(await _service.UpdateAsync(id, dto));
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeActiveStatusAsync(int id, [FromBody] DisableMedicDto dto)
    {
        return Ok(await _service.ChangeActiveStatusAsync(id, dto));
    }
}
