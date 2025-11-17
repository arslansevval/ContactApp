using ContactApp.Core.Entities;
using ContactApp.Core.Interfaces;
using ContactApp.Infrastructure.Data;
using ContactApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class EmployeeWithContactInfoRepository : GenericRepository<Employee>, IEmployeeContactInfoRepository
{
    public EmployeeWithContactInfoRepository(AppDbContext context) : base(context) { }

    /// <summary>
    /// Tüm Employee'leri ContactInfos ile birlikte getirir
    /// </summary>
    public async Task<List<Employee>> GetAllWithContactInfosAsync()
    {
        return await _dbSet
            .Include(e => e.ContactInfos)
            .Include(e => e.Company) // <-- bunu ekle
            .ToListAsync();
    }


    /// <summary>
    /// Tek bir Employee'yi ContactInfos ile birlikte getirir
    /// </summary>
    public async Task<Employee?> GetByIdWithContactInfosAsync(int id)
    {
        return await _dbSet
            .Include(e => e.ContactInfos)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <summary>
    /// Yeni ContactInfo ekler
    /// </summary>
    public async Task AddContactInfoAsync(ContactInfo contactInfo)
    {
        await _context.Set<ContactInfo>().AddAsync(contactInfo);
    }

    /// <summary>
    /// Mevcut ContactInfo'yu günceller
    /// </summary>
    public void UpdateContactInfo(ContactInfo contactInfo)
    {
        _context.Set<ContactInfo>().Update(contactInfo);
    }

    /// <summary>
    /// ContactInfo siler
    /// </summary>
    public void RemoveContactInfo(ContactInfo contactInfo)
    {
        _context.Set<ContactInfo>().Remove(contactInfo);
    }
}
