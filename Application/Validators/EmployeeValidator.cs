using FluentValidation;
using ContactApp.Application.DTOs;

namespace ContactApp.Application.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeCreateDto>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad alanı zorunludur")
                .MaximumLength(50).WithMessage("Ad 50 karakterden uzun olamaz");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad alanı zorunludur");

            RuleFor(x => x.CompanyId)
                .GreaterThan(0).WithMessage("Geçerli bir şirket seçiniz");
        }
    }
}
