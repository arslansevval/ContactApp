using ContactApp.Core.Entities;
using ContactApp.Core.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace ContactApp.Application.Services;

public class EmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    /// <summary>
    /// Tüm çalışanları getirir (cache destekli)
    /// </summary>
    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        const string cacheKey = "all_employees";

        // Cache kontrolü
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Employee>? employees))
            return employees!;

        // Cache'de yoksa veritabanından çek
        employees = await _unitOfWork.Employees.GetAllAsync();

        // Eğer veri null değilse cache’e ekle
        if (employees != null && employees.Any())
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_cacheDuration);

            _cache.Set(cacheKey, employees, cacheOptions);
        }

        return employees ?? Enumerable.Empty<Employee>();
    }

    /// <summary>
    /// Tek bir çalışanı ID'ye göre getirir (cache destekli)
    /// </summary>
    public async Task<Employee?> GetByIdAsync(int id)
    {
        string cacheKey = $"employee_{id}";

        if (_cache.TryGetValue(cacheKey, out Employee? employee))
            return employee;

        employee = await _unitOfWork.Employees.GetByIdAsync(id);

        if (employee is not null)
            _cache.Set(cacheKey, employee, _cacheDuration);

        return employee;
    }

    /// <summary>
    /// Yeni çalışan ekler ve cache’i temizler
    /// </summary>
    public async Task AddAsync(Employee employee)
    {
        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.CompleteAsync();

        InvalidateCache(employee.Id);
    }

    /// <summary>
    /// Mevcut çalışanı günceller ve cache’i temizler
    /// </summary>
    public async Task UpdateAsync(Employee employee)
    {
        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.CompleteAsync();

        InvalidateCache(employee.Id);
    }

    /// <summary>
    /// Çalışanı siler ve cache’i temizler
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee is not null)
        {
            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.CompleteAsync();
        }

        InvalidateCache(id);
    }

    /// <summary>
    /// Cache invalidation işlemlerini merkezi olarak yönetir
    /// </summary>
    private void InvalidateCache(int employeeId)
    {
        _cache.Remove("all_employees");
        _cache.Remove($"employee_{employeeId}");
    }
}
