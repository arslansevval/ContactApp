using Microsoft.AspNetCore.Mvc;
using ContactApp.Core.Entities;
using ContactApp.Core.Interfaces;
using ContactApp.Application.DTOs;
using AutoMapper;
using ContactApp.Application.Services;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;  

    public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var employeesDto = _mapper.Map<IEnumerable<EmployeeReadDto>>(employees);
        return Ok(employeesDto);
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null) return NotFound();

        var employeeDto = _mapper.Map<EmployeeReadDto>(employee);
        return Ok(employeeDto);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(EmployeeCreateDto dto)
    {
        var employee = _mapper.Map<Employee>(dto); // AutoMapper kullanıyoruz
        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.CompleteAsync();

        var readDto = _mapper.Map<EmployeeCreateDto>(employee);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, readDto);
    }


    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(int id, Employee employee)
    {
        if (id != employee.Id) return BadRequest();
        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.CompleteAsync();
        return NoContent();
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null) return NotFound();
        _unitOfWork.Employees.Remove(employee);
        await _unitOfWork.CompleteAsync();
        return NoContent();
    }
}
