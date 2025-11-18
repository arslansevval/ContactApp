using ContactApp.Application.DTOs;
using FluentValidation;

namespace ContactApp.Application.Validators
{
    public class ContactInfoValidator : AbstractValidator<ContactInfoCreateDto>
    {
        public ContactInfoValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("EmployeeId must be greater than zero.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Contact type cannot be empty.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Contact value cannot be empty.");
        }

        private bool BeAValidType(string type)
        {
            var validTypes = new[] { "Email", "Phone", "Address" };
            return validTypes.Contains(type, StringComparer.OrdinalIgnoreCase);
        }

        private bool BeValidValue(ContactInfoCreateDto dto, string value)
        {
            if (dto.Type.Equals("Email", StringComparison.OrdinalIgnoreCase))
                return value.Contains("@");

            if (dto.Type.Equals("Phone", StringComparison.OrdinalIgnoreCase))
                return value.All(char.IsDigit) && value.Length >= 10;

            // Address için özel kontrol yok
            return true;
        }
    }
}
