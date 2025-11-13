using ContactApp.Core.Entities;
using ContactApp.Core.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace ContactApp.Application.Services;

public class ContactInfoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public ContactInfoService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    /// <summary>
    /// Belirli bir çalışanın iletişim bilgilerini getirir (cache destekli)
    /// </summary>
    public async Task<IEnumerable<ContactInfo>> GetByEmployeeIdAsync(int employeeId)
    {
        string cacheKey = $"employee_contacts_{employeeId}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<ContactInfo>? contacts))
            return contacts!;

        var allContacts = await _unitOfWork.ContactInfos.GetAllAsync();
        contacts = allContacts.Where(c => c.EmployeeId == employeeId);

        if (contacts.Any())
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_cacheDuration);

            _cache.Set(cacheKey, contacts, cacheOptions);
        }

        return contacts ?? Enumerable.Empty<ContactInfo>();
    }

    /// <summary>
    /// ID’ye göre iletişim bilgisi getirir (cache destekli)
    /// </summary>
    public async Task<ContactInfo?> GetByIdAsync(int id)
    {
        string cacheKey = $"contact_{id}";

        if (_cache.TryGetValue(cacheKey, out ContactInfo? contact))
            return contact;

        contact = await _unitOfWork.ContactInfos.GetByIdAsync(id);

        if (contact is not null)
            _cache.Set(cacheKey, contact, _cacheDuration);

        return contact;
    }

    /// <summary>
    /// Yeni iletişim bilgisi ekler ve cache’i temizler
    /// </summary>
    public async Task AddAsync(ContactInfo contact)
    {
        await _unitOfWork.ContactInfos.AddAsync(contact);
        await _unitOfWork.CompleteAsync();

        InvalidateCache(contact);
    }

    /// <summary>
    /// Mevcut iletişim bilgisini günceller ve cache’i temizler
    /// </summary>
    public async Task UpdateAsync(ContactInfo contact)
    {
        _unitOfWork.ContactInfos.Update(contact);
        await _unitOfWork.CompleteAsync();

        InvalidateCache(contact);
    }

    /// <summary>
    /// İletişim bilgisini siler ve cache’i temizler
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var contact = await _unitOfWork.ContactInfos.GetByIdAsync(id);
        if (contact is not null)
        {
            _unitOfWork.ContactInfos.Remove(contact);
            await _unitOfWork.CompleteAsync();

            InvalidateCache(contact);
        }
    }

    /// <summary>
    /// Cache invalidation işlemlerini merkezi olarak yönetir
    /// </summary>
    private void InvalidateCache(ContactInfo contact)
    {
        _cache.Remove($"contact_{contact.Id}");
        _cache.Remove($"employee_contacts_{contact.EmployeeId}");
    }
}
