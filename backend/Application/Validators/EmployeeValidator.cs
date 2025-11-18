using FluentValidation;
using ContactApp.Application.DTOs;

namespace ContactApp.Application.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeCreateDto>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Firstname is required.")
                .MaximumLength(50).WithMessage("Firstname cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Lastname is required");

            RuleFor(x => x.CompanyId)
                .GreaterThan(0).WithMessage("Company is required.");
        }
    }
}
