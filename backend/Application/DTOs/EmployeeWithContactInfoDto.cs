using System.Text.Json.Serialization;

namespace ContactApp.Application.DTOs
{
    public class EmployeeWithContactInfoDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Position { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        public List<ContactInfoReadDto> ContactInfos { get; set; } = new List<ContactInfoReadDto>();
    }
}
