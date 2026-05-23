using FluentValidation;

namespace Once.Application.Services.Branches.Contracts;

public class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequest>
{
    public CreateBranchRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[a-z0-9\-]+$")
            .WithMessage("Code must contain only lowercase letters, digits, and hyphens.");

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .When(x => x.Address is not null);
    }
}
