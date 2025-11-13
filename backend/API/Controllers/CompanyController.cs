using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ContactApp.Core.Entities;
using ContactApp.Core.Interfaces;
using ContactApp.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CompanyController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // GET: api/company
    [HttpGet("GetAll")]
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

    // POST: api/company
    [HttpPost("Create")]
    public async Task<IActionResult> Create(CompanyCreateDto dto)
    {
        var company = _mapper.Map<Company>(dto);
        await _unitOfWork.Companies.AddAsync(company);
        await _unitOfWork.CompleteAsync();

        var readDto = _mapper.Map<CompanyReadDto>(company);
        return CreatedAtAction(nameof(GetById), new { id = company.Id }, readDto);
    }

    // PUT: api/company/{id}
    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(int id, CompanyCreateDto dto)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company == null) return NotFound();

        _mapper.Map(dto, company); // DTO'daki alanları entity’ye map et
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
