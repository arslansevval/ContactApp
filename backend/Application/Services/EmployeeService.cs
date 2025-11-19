using ContactApp.Core.Entities;
using ContactApp.Core.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using ContactApp.Application.DTOs;
using ContactApp.Infrastructure.Repositories;

namespace ContactApp.Application.Services;

public class EmployeeService
{
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);
    private readonly IEmployeeContactInfoRepository _employeeContactInfoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMemoryCache cache,
        IEmployeeContactInfoRepository employeeContactInfoRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
        _employeeContactInfoRepository = employeeContactInfoRepository;
    }


    /// Tüm çalışanları getirir (cache destekli)
    public async Task<IEnumerable<EmployeeReadDto>> GetAllAsync()
    {
        const string cacheKey = "all_employees";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<EmployeeReadDto>? employees))
            return employees!;

        var entities = await _employeeContactInfoRepository.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<EmployeeReadDto>>(entities);

        if (dtos.Any())
            _cache.Set(cacheKey, dtos, new MemoryCacheEntryOptions().SetSlidingExpiration(_cacheDuration));

        return dtos;
    }


    /// Tek bir çalışanı ID'ye göre getirir (cache destekli)
    public async Task<EmployeeReadDto?> GetByIdAsync(int id)
    {
        string cacheKey = $"employee_{id}";

        if (_cache.TryGetValue(cacheKey, out EmployeeReadDto? cachedEmployee))
            return cachedEmployee;

        var employee = await _employeeContactInfoRepository.GetByIdAsync(id);
        if (employee == null) return null;

        var dto = _mapper.Map<EmployeeReadDto>(employee);
        _cache.Set(cacheKey, dto, new MemoryCacheEntryOptions().SetSlidingExpiration(_cacheDuration));

        return dto;
    }


    /// Tüm çalışanları ContactInfos ile getirir
    public async Task<List<EmployeeWithContactInfoDto>> GetEmployeesWithContactInfosAsync()
    {
        var employees = await _employeeContactInfoRepository.GetAllWithContactInfosAsync();
        return employees.Select(e => new EmployeeWithContactInfoDto
        {
            Id = e.Id,
            CompanyId = e.CompanyId,
            CompanyName = e.Company.Name,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Position = e.Position,
            CreatedAt = e.CreatedAt,
            ContactInfos = e.ContactInfos
            .Select(c => new ContactInfoReadDto
            {
                Id = c.Id,
                Type = c.Type,
                Value = c.Value,
                IsPrimary = c.IsPrimary,
                CreatedAt = c.CreatedAt
            }).ToList()
        }).ToList();
    }


    /// Employee + ContactInfos birlikte ekleme
    public async Task<EmployeeWithContactInfoDto> CreateEmployeeWithContactInfosAsync(EmployeeWithContactInfoDto dto)
    {
        // 1) Employee ekle (ContactInfos map edilmeden)
        var employee = _mapper.Map<Employee>(dto);
        employee.CompanyId = dto.CompanyId;
        employee.CreatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.CompleteAsync(); // Employee ID'si için commit


        // 3) Cache temizle
        InvalidateCache(employee.Id);

        // 4) DTO dön
        var resDto = _mapper.Map<EmployeeWithContactInfoDto>(employee);
        resDto.CompanyName = dto.CompanyName;
        return resDto;
    }

    public async Task UpdateEmployeeWithContactInfosAsync(EmployeeWithContactInfoDto dto)
    {
        var employee = await _employeeContactInfoRepository.GetByIdWithContactInfosAsync(dto.Id);
        if (employee == null)
            throw new KeyNotFoundException("Employee not found");

        // 1) Employee güncelle
        employee.CompanyId = dto.CompanyId;
        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Position = dto.Position;
        _unitOfWork.Employees.Update(employee);

        // 2) ContactInfos sıfırla ve yeniden ekle
        if (employee.ContactInfos != null)
        {
            foreach (var c in employee.ContactInfos.ToList())
                _unitOfWork.ContactInfos.Remove(c);
        }

        if (dto.ContactInfos != null && dto.ContactInfos.Any())
        {
            foreach (var cDto in dto.ContactInfos)
            {
                var newContact = new ContactInfo
                {
                    EmployeeId = employee.Id,
                    Type = cDto.Type,
                    Value = cDto.Value,
                    IsPrimary = cDto.IsPrimary,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.ContactInfos.AddAsync(newContact);
            }
        }


        // 3) Commit + Cache temizle
        await _unitOfWork.CompleteAsync();
        InvalidateCache(employee.Id);
    }

    /// Employee + ContactInfos birlikte silme
    public async Task DeleteWithContactInfosAsync(int employeeId)
    {
        var employee = await _employeeContactInfoRepository.GetByIdWithContactInfosAsync(employeeId);
        if (employee == null) throw new KeyNotFoundException("Employee not found");

        foreach (var contact in employee.ContactInfos)
        {
            _unitOfWork.ContactInfos.Remove(contact);
        }

        _unitOfWork.Employees.Remove(employee);
        await _unitOfWork.CompleteAsync();

        InvalidateCache(employeeId);
    }

    /// Cache temizleme
    private void InvalidateCache(int employeeId)
    {
        _cache.Remove("all_employees");
        _cache.Remove($"employee_{employeeId}");
    }
}
