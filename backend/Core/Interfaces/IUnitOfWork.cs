using ContactApp.Core.Entities;

namespace ContactApp.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Employee> Employees { get; }
    IRepository<Company> Companies { get; }
    IRepository<ContactInfo> ContactInfos { get; }
    IUserRepository Users { get; }
    IEmployeeContactInfoRepository EmployeeContactInfos { get; }
    Task<int> CompleteAsync();
}
