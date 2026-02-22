using Account.Application.Features.Auth.Commands.EmailAuthentication;
using FluentValidation;

namespace Account.Application.Features.Auth.Validation;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email field must not be empty")
            .EmailAddress()
            .WithMessage("Incorrect email address")
            .MaximumLength(256)
            .WithMessage("Length must be less than 256");

        RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .WithMessage("VerificationCode field must not be empty")
            .Length(6)
            .WithMessage("Verification code must be 6 digits")
            .Matches("^[0-9]{6}$")
            .WithMessage("Verification code must contain only digits");
    }
}
