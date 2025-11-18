using FluentValidation;
using ContactApp.Application.DTOs;

namespace ContactApp.Application.Validators
{
    public class CompanyValidator : AbstractValidator<CompanyCreateDto>
    {
        public CompanyValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(50).WithMessage("Company name cannot exceed 50 characters.");

        }
    }
}
