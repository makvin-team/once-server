using FluentValidation;

namespace Once.Application.Services.Positions.Contracts;

public class CreatePositionRequestValidator : AbstractValidator<CreatePositionRequest>
{
    public CreatePositionRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[a-z0-9\-]+$")
            .WithMessage("Code must contain only lowercase letters, digits, and hyphens.");
    }
}
