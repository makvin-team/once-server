using FluentValidation;

namespace Once.Application.Services.Branches.Contracts;

public class UpdateBranchRequestValidator : AbstractValidator<UpdateBranchRequest>
{
    public UpdateBranchRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .When(x => x.Address is not null);
    }
}
