namespace ContactApp.Application.DTOs;
public class EmployeeCreateDto
{
    public int CompanyId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Position { get; set; }
}
