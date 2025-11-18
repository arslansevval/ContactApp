using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ContactApp.Core.Entities;
using ContactApp.Core.Interfaces;
using ContactApp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using FluentValidation.Results;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CompanyCreateDto> _validator;

    public CompanyController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CompanyCreateDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    // GET: api/company
    [HttpGet("GetAll")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var companies = await _unitOfWork.Companies.GetAllAsync();
        var companiesDto = _mapper.Map<IEnumerable<CompanyReadDto>>(companies);
        return Ok(companiesDto);
    }

    // GET: api/company/{id}
    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company == null) return NotFound();

        var companyDto = _mapper.Map<CompanyReadDto>(company);
        return Ok(companyDto);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CompanyCreateDto dto)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            // Tüm hataları bir array olarak dönüyoruz
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var company = _mapper.Map<Company>(dto);
        await _unitOfWork.Companies.AddAsync(company);
        await _unitOfWork.CompleteAsync();

        var readDto = _mapper.Map<CompanyReadDto>(company);
        return CreatedAtAction(nameof(GetById), new { id = company.Id }, readDto);
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(int id, CompanyCreateDto dto)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company == null) return NotFound();

        _mapper.Map(dto, company);
        _unitOfWork.Companies.Update(company);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    // DELETE: api/company/{id}
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company == null) return NotFound();

        _unitOfWork.Companies.Remove(company);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}
