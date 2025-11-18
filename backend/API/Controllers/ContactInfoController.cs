using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ContactApp.Core.Entities;
using ContactApp.Core.Interfaces;
using ContactApp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ContactInfoController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ContactInfoController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // GET: api/contactinfo/}
    [HttpGet("GetByEmployeeId/{employeeId}")]
    public async Task<IActionResult> GetByEmployeeId(int employeeId)
    {
        var infos = await _unitOfWork.ContactInfos.FindAsync(c => c.EmployeeId == employeeId);
        var dto = _mapper.Map<IEnumerable<ContactInfoReadDto>>(infos);
        return Ok(dto);
    }

    // POST: api/contactinfo
    [HttpPost("Create")]
    public async Task<IActionResult> Create(ContactInfoCreateDto dto)
    {
        var contact = _mapper.Map<ContactInfo>(dto);
        await _unitOfWork.ContactInfos.AddAsync(contact);
        await _unitOfWork.CompleteAsync();

        var readDto = _mapper.Map<ContactInfoReadDto>(contact);
        return CreatedAtAction(nameof(GetById), new { id = contact.Id }, readDto);
    }

    // GET: api/contactinfo/{id}
    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var contact = await _unitOfWork.ContactInfos.GetByIdAsync(id);
        if (contact == null) return NotFound();

        var dto = _mapper.Map<ContactInfoReadDto>(contact);
        return Ok(dto);
    }

    // PUT: api/contactinfo/{id}
    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(int id, ContactInfoCreateDto dto)
    {
        var contact = await _unitOfWork.ContactInfos.GetByIdAsync(id);
        if (contact == null) return NotFound();

        _mapper.Map(dto, contact);
        _unitOfWork.ContactInfos.Update(contact);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    // DELETE: api/contactinfo/{id}
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var contact = await _unitOfWork.ContactInfos.GetByIdAsync(id);
        if (contact == null) return NotFound();

        _unitOfWork.ContactInfos.Remove(contact);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}
