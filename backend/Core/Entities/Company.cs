namespace ContactApp.Core.Entities;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
