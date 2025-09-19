using DotNetApiLambdaTemplate.Domain.Enums;
using FluentValidation;

namespace DotNetApiLambdaTemplate.Application.Users.Commands.CreateUser;

/// <summary>
/// Validator for the CreateUserCommand
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(254).WithMessage("Email cannot exceed 254 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid user role");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .Matches(@"^[\+]?[1-9][\d]{0,15}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Phone number must be a valid phone number");

        RuleFor(x => x.Department)
            .MaximumLength(100).WithMessage("Department cannot exceed 100 characters");

        RuleFor(x => x.JobTitle)
            .MaximumLength(100).WithMessage("Job title cannot exceed 100 characters");

        RuleFor(x => x.TimeZone)
            .NotEmpty().WithMessage("Time zone is required")
            .MaximumLength(50).WithMessage("Time zone cannot exceed 50 characters");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required")
            .MaximumLength(10).WithMessage("Language cannot exceed 10 characters")
            .Matches(@"^[a-z]{2}-[A-Z]{2}$").WithMessage("Language must be in format 'en-US'");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("CreatedBy is required")
            .MaximumLength(100).WithMessage("CreatedBy cannot exceed 100 characters");
    }
}
