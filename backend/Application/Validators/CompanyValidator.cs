using FluentValidation;
using ContactApp.Application.DTOs;

namespace ContactApp.Application.Validators
{
    public class CompanyValidator : AbstractValidator<CompanyCreateDto>
    {
        public CompanyValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Şirket adı zorunludur")
                .MaximumLength(100);

            RuleFor(x => x.Address)
                .MaximumLength(200);
        }
    }
}
