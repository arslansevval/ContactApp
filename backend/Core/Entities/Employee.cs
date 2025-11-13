namespace ContactApp.Core.Entities;
public class Employee
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Position { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Company Company { get; set; } = null!;
    public ICollection<ContactInfo> ContactInfos { get; set; } = new List<ContactInfo>();
}
