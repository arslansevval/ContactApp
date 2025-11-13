namespace ContactApp.Core.Entities;

public class ContactInfo
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string Type { get; set; } = null!; // email, phone, etc.
    public string Value { get; set; } = null!;
    public bool IsPrimary { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Employee Employee { get; set; } = null!;
}
