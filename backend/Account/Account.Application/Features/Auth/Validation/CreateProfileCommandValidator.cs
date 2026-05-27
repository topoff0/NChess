using Account.Application.Features.Auth.Commands.CreateProfile;
using FluentValidation;

namespace Account.Application.Features.Auth.Validation;

public class CreateProfileCommandValidator : AbstractValidator<CreateProfileCommand>
{
    public CreateProfileCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email field must not be empty")
            .EmailAddress()
            .WithMessage("Incorrect email address")
            .MaximumLength(256)
            .WithMessage("Length must be less than 256");

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username field must not be empty")
            .Length(3, 50)
            .WithMessage("Username length must be between 3 and 50");

        RuleFor(x => x.ProfileImage)
            .Must(img => img == null || img.Length <= 5 * 1024 * 1024)
            .WithMessage("Profile image size must not exceed 5 MB");
    }
}
