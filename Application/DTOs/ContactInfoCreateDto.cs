namespace ContactApp.Application.DTOs;

public class ContactInfoCreateDto
{
    public int EmployeeId { get; set; }
    public string Type { get; set; } = null!; // phone, email, vs.
    public string Value { get; set; } = null!;
    public bool IsPrimary { get; set; } = false;
}
