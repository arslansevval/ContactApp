using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContactApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ContactApp.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // 1) DB/Migration için retry mekanizması (Postgres geç ayağa kalkarsa API çakılmasın)
            const int maxAttempts = 10;
            var delay = TimeSpan.FromSeconds(3);

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    context.Database.Migrate(); // Tablolar yoksa oluştur, migration'ları uygula
                    break; // Başarılı → döngüden çık
                }
                catch (NpgsqlException)
                {
                    if (attempt == maxAttempts)
                        throw; // Son denemede de patlarsa exception fırlat (logda görürsün)

                    Thread.Sleep(delay); // Biraz bekle, tekrar dene
                }
            }

            // 2) Companies + Employees + ContactInfos seed (Id set ETMİYORUZ)
            if (!context.Companies.Any() && !context.Employees.Any() && !context.ContactInfos.Any())
            {
                // Apple
                var apple = new Company
                {
                    Name = "Apple",
                    Address = "123 Main St",
                    Phone = "555-1234",
                    Email = "info@apple.com",
                    CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                };

                var sevval = new Employee
                {
                    Company = apple, // FK yerine navigation kullanıyoruz
                    FirstName = "Sevval",
                    LastName = "Arslan",
                    Position = "Developer",
                    CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc),
                    ContactInfos = new List<ContactInfo>
                    {
                        new ContactInfo
                        {
                            Type = "Email",
                            Value = "sevval.arslan@gmail.com",
                            IsPrimary = true,
                            CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                        },
                        new ContactInfo
                        {
                            Type = "Phone",
                            Value = "555-0001",
                            IsPrimary = true,
                            CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                        }
                    }
                };

                // Beta LLC
                var beta = new Company
                {
                    Name = "Beta LLC",
                    Address = "456 Oak Ave",
                    Phone = "555-5678",
                    Email = "contact@beta.com",
                    CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                };

                var zeynep = new Employee
                {
                    Company = beta,
                    FirstName = "Zeynep",
                    LastName = "Suvak",
                    Position = "Manager",
                    CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc),
                    ContactInfos = new List<ContactInfo>
                    {
                        new ContactInfo
                        {
                            Type = "Email",
                            Value = "zeynep.suvak@ed.com",
                            IsPrimary = true,
                            CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                        },
                        new ContactInfo
                        {
                            Type = "Phone",
                            Value = "555-0002",
                            IsPrimary = true,
                            CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                        }
                    }
                };

                // Root entity'leri eklemek yeterli,
                // EF navigation'lardan Companies / Employees / ContactInfos zincirini takip edip hepsini insert eder.
                context.Companies.AddRange(apple, beta);
                context.Employees.AddRange(sevval, zeynep);

                context.SaveChanges();
            }

            // 3) Users seed
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        PasswordHash = "AQAAAAIAAYagAAAAEFqtbOPE2idxUpyojdztJmdIuFZRmhofTP5g8NIu/q+hqF8FXWYZ+3s9iagNW9jKKg==", // admin123
                        Role = "Admin",
                        CreatedAt = new DateTime(2025, 11, 16, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new User
                    {
                        Username = "user",
                        PasswordHash = "AQAAAAIAAYagAAAAEN2hsSrePGAnhJwh204VG2GpErdFNpQJDVCivC4h2qy7uJcB1Uu8ardHMMPk5WlHQg==", // user123
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
