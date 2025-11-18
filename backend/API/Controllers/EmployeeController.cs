using Microsoft.AspNetCore.Mvc;
using ContactApp.Application.Services;
using ContactApp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using FluentValidation.Results;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly EmployeeService _employeeService;
    private readonly IValidator<EmployeeCreateDto> _employeeValidator;

    public EmployeeController(EmployeeService employeeService, IValidator<EmployeeCreateDto> employeeValidator)
    {
        _employeeService = employeeService;
        _employeeValidator = employeeValidator;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var employeesDto = await _employeeService.GetAllAsync();
        return Ok(employeesDto);
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employeeDto = await _employeeService.GetByIdAsync(id);
        if (employeeDto == null) return NotFound();

        return Ok(employeeDto);
    }

    [HttpGet("GetAllWithContactInfos")]
    public async Task<IActionResult> GetAllWithContactInfos()
    {
        var employeesWithContacts = await _employeeService.GetEmployeesWithContactInfosAsync();
        return Ok(employeesWithContacts);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(EmployeeWithContactInfoDto dto)
    {
        // DTO içinden EmployeeCreateDto alıyoruz, örnek olarak FirstName, LastName, CompanyId
        var employeeDto = new EmployeeCreateDto
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CompanyId = dto.CompanyId
        };

        ValidationResult validationResult = await _employeeValidator.ValidateAsync(employeeDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _employeeService.CreateEmployeeWithContactInfosAsync(dto);
        return Ok(result);
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(int id, EmployeeWithContactInfoDto dto)
    {
        if (id != dto.Id) return BadRequest();

        var employeeDto = new EmployeeCreateDto
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CompanyId = dto.CompanyId
        };

        ValidationResult validationResult = await _employeeValidator.ValidateAsync(employeeDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        await _employeeService.UpdateEmployeeWithContactInfosAsync(dto);
        return NoContent();
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _employeeService.DeleteWithContactInfosAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
