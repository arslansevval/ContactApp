using ContactApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Migration varsa uygular (tabloları oluşturur)
            //context.Database.Migrate();

            // -------------------- Companies -------------------- //
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

            foreach (var company in companies)
            {
                if (!context.Companies.Any(c => c.Id == company.Id))
                {
                    context.Companies.Add(company);
                }
            }
            context.SaveChanges();

            // -------------------- Employees -------------------- //
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

            foreach (var employee in employees)
            {
                if (!context.Employees.Any(e => e.Id == employee.Id))
                {
                    context.Employees.Add(employee);
                }
            }
            context.SaveChanges();

            // -------------------- ContactInfos -------------------- //
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

            foreach (var ci in contactInfos)
            {
                if (!context.ContactInfos.Any(c => c.Id == ci.Id))
                {
                    context.ContactInfos.Add(ci);
                }
            }
            context.SaveChanges();

            // -------------------- Users -------------------- //
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "AQAAAAIAAYagAAAAEFqtbOPE2idxUpyojdztJmdIuFZRmhofTP5g8NIu/q+hqF8FXWYZ+3s9iagNW9jKKg==", // admin123
                    Role = "Admin",
                    CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    Username = "user",
                    PasswordHash = "AQAAAAIAAYagAAAAEN2hsSrePGAnhJwh204VG2GpErdFNpQJDVCivC4h2qy7uJcB1Uu8ardHMMPk5WlHQg==", // user123
                    Role = "User",
                    CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                }
            };

            foreach (var user in users)
            {
                if (!context.Users.Any(u => u.Username == user.Username))
                {
                    context.Users.Add(user);
                }
            }
            context.SaveChanges();
        }
    }
}
