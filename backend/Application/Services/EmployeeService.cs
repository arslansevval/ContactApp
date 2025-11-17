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

    /// <summary>
    /// Tüm çalışanları getirir (cache destekli)
    /// </summary>
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

    /// <summary>
    /// Tek bir çalışanı ID'ye göre getirir (cache destekli)
    /// </summary>
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

    /// <summary>
    /// Tüm çalışanları ContactInfos ile getirir
    /// </summary>
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
            ContactInfos = e.ContactInfos.Select(c => new ContactInfoReadDto
            {
                Id = c.Id,
                Type = c.Type,
                Value = c.Value,
                IsPrimary = c.IsPrimary,
                CreatedAt = c.CreatedAt
            }).ToList()
        }).ToList();
    }

    /// <summary>
    /// Employee + ContactInfos birlikte ekleme
    /// </summary>
    public async Task<EmployeeWithContactInfoDto> CreateEmployeeWithContactInfosAsync(EmployeeWithContactInfoDto dto)
    {
        // 1) Employee ekle
        var employee = _mapper.Map<Employee>(dto);
        // Eğer Company bilgisi de DTO’da geliyorsa, yeni nesne oluştur
        employee.CompanyId = dto.CompanyId;
        employee.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.CompleteAsync(); // Employee ID'si için commit

        // 2) ContactInfo ekle
        foreach (var cDto in dto.ContactInfos)
        {
            var contact = _mapper.Map<ContactInfo>(cDto);
            contact.EmployeeId = employee.Id; // FK mutlaka verilmeli
            await _unitOfWork.ContactInfos.AddAsync(contact);
        }

        // 3) Transaction commit
        await _unitOfWork.CompleteAsync();

        // 4) Cache temizle
        InvalidateCache(employee.Id);
        var resDto = _mapper.Map<EmployeeWithContactInfoDto>(employee);
        resDto.CompanyName = dto.CompanyName;
        return resDto;
    }


    /// <summary>
    /// Employee + ContactInfos birlikte güncelleme
        /// </summary>
    public async Task UpdateEmployeeWithContactInfosAsync(EmployeeWithContactInfoDto dto)
    {
        var employee = await _employeeContactInfoRepository.GetByIdWithContactInfosAsync(dto.Id);
        if (employee == null)
            throw new KeyNotFoundException("Employee not found");

        // ---------------------------
        // 1) Employee güncelle
        employee.CompanyId = dto.CompanyId;
        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Position = dto.Position;

        _unitOfWork.Employees.Update(employee);

        // Tüm ContactInfos'u sil
        foreach(var c in employee.ContactInfos.ToList())
            _unitOfWork.ContactInfos.Remove(c);

        // Sonra DTO'dan gelenleri ekle
        foreach(var cDto in dto.ContactInfos)
        {
            var newContact = _mapper.Map<ContactInfo>(cDto);
            newContact.EmployeeId = employee.Id;
            await _unitOfWork.ContactInfos.AddAsync(newContact);
        }

        // ---------------------------
        // 3) Commit + Cache temizle
        // ---------------------------
        await _unitOfWork.CompleteAsync();
        InvalidateCache(employee.Id);
    }



    /// <summary>
    /// Employee + ContactInfos birlikte silme
    /// </summary>
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

    /// <summary>
    /// Cache temizleme
    /// </summary>
    private void InvalidateCache(int employeeId)
    {
        _cache.Remove("all_employees");
        _cache.Remove($"employee_{employeeId}");
    }
}
