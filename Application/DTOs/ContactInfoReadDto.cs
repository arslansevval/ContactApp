namespace ContactApp.Application.DTOs;

public class ContactInfoReadDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
}
