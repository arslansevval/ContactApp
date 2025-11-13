namespace ContactApp.Application.DTOs;
public class EmployeeReadDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string? Position { get; set; }
}