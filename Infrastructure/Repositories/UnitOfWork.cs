using ContactApp.Core.Entities;
using ContactApp.Core.Interfaces;
using ContactApp.Infrastructure.Data;

namespace ContactApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Employees = new GenericRepository<Employee>(_context);
        Companies = new GenericRepository<Company>(_context);
        ContactInfos = new GenericRepository<ContactInfo>(_context);
    }

    public IRepository<Employee> Employees { get; private set; }
    public IRepository<Company> Companies { get; private set; }
    public IRepository<ContactInfo> ContactInfos { get; private set; }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
