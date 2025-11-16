using ContactApp.Core.Entities;
using ContactApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.Migrate(); // Migration varsa uygular

            if (!context.Companies.Any())
            {
                var companies = new List<Company>
                {
                    new Company
                    {
                        Id = 1,
                        Name = "Apple",
                        Address = "123 Main St",
                        Phone = "555-1234",
                        Email = "info@apple.com",
                        CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new Company
                    {
                        Id = 2,
                        Name = "Beta LLC",
                        Address = "456 Oak Ave",
                        Phone = "555-5678",
                        Email = "contact@beta.com",
                        CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                    }
                };
                context.Companies.AddRange(companies);
                context.SaveChanges();
            }

            if (!context.Employees.Any())
            {
                var employees = new List<Employee>
                {
                    new Employee
                    {
                        Id = 1,
                        CompanyId = 1,
                        FirstName = "Sevval",
                        LastName = "Arslan",
                        Position = "Developer",
                        CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new Employee
                    {
                        Id = 2,
                        CompanyId = 2,
                        FirstName = "Zeynep",
                        LastName = "Suvak",
                        Position = "Manager",
                        CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                    }
                };
                context.Employees.AddRange(employees);
                context.SaveChanges();
            }

            if (!context.ContactInfos.Any())
            {
                var contactInfos = new List<ContactInfo>
                {
                    new ContactInfo
                    {
                        Id = 1,
                        EmployeeId = 1,
                        Type = "Email",
                        Value = "sevval.arslan@gmail.com",
                        IsPrimary = true,
                        CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new ContactInfo
                    {
                        Id = 2,
                        EmployeeId = 1,
                        Type = "Phone",
                        Value = "555-0001",
                        IsPrimary = true,
                        CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new ContactInfo
                    {
                        Id = 3,
                        EmployeeId = 2,
                        Type = "Email",
                        Value = "zeynep.suvak@ed.com",
                        IsPrimary = true,
                        CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new ContactInfo
                    {
                        Id = 4,
                        EmployeeId = 2,
                        Type = "Phone",
                        Value = "555-0002",
                        IsPrimary = true,
                        CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                    }
                };
                context.ContactInfos.AddRange(contactInfos);
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Id = 1,
                        Username = "admin",
                        PasswordHash = "admin123", // gerçek projede hashle
                        Role = "Admin",
                        CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new User
                    {
                        Id = 2,
                        Username = "user",
                        PasswordHash = "user123",
                        Role = "User",
                        CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                    }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }
    }
}
