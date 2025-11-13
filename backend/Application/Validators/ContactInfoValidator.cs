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
                .WithMessage("Çalışan (EmployeeId) değeri geçerli olmalıdır.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("İletişim türü boş olamaz.")
                .MaximumLength(50).WithMessage("İletişim türü en fazla 50 karakter olabilir.")
                .Must(BeAValidType).WithMessage("İletişim türü 'Email', 'Phone' veya 'Address' olmalıdır.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("İletişim bilgisi (Value) boş olamaz.")
                .MaximumLength(255).WithMessage("İletişim bilgisi en fazla 255 karakter olabilir.")
                .Must(BeValidValue).WithMessage("Girilen iletişim bilgisi formatı geçerli değil.");
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
