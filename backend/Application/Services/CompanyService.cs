using ContactApp.Core.Entities;
using ContactApp.Core.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace ContactApp.Application.Services;

public class CompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public CompanyService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    /// Tüm şirketleri getirir (cache destekli)
    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        const string cacheKey = "all_companies";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Company>? companies))
            return companies!;

        companies = await _unitOfWork.Companies.GetAllAsync();

        if (companies != null && companies.Any())
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_cacheDuration);

            _cache.Set(cacheKey, companies, cacheOptions);
        }

        return companies ?? Enumerable.Empty<Company>();
    }


    /// ID’ye göre tek bir şirket getirir (cache destekli)
    public async Task<Company?> GetByIdAsync(int id)
    {
        string cacheKey = $"company_{id}";

        if (_cache.TryGetValue(cacheKey, out Company? company))
            return company;

        company = await _unitOfWork.Companies.GetByIdAsync(id);

        if (company is not null)
            _cache.Set(cacheKey, company, _cacheDuration);

        return company;
    }

    // Yeni şirket ekler ve cache’i temizler
    public async Task AddAsync(Company company)
    {
        await _unitOfWork.Companies.AddAsync(company);
        await _unitOfWork.CompleteAsync();

        InvalidateCache(company.Id);
    }

    // Şirketi günceller ve cache’i temizler
    public async Task UpdateAsync(Company company)
    {
        _unitOfWork.Companies.Update(company);
        await _unitOfWork.CompleteAsync();

        InvalidateCache(company.Id);
    }

    /// Şirketi siler ve cache’i temizler
    public async Task DeleteAsync(int id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company is not null)
        {
            _unitOfWork.Companies.Remove(company);
            await _unitOfWork.CompleteAsync();
        }

        InvalidateCache(id);
    }

    /// Cache invalidation işlemlerini merkezi olarak yönetir
    private void InvalidateCache(int companyId)
    {
        _cache.Remove("all_companies");
        _cache.Remove($"company_{companyId}");
    }
}
