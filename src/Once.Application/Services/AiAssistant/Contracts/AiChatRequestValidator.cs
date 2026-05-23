using FluentValidation;

namespace Once.Application.Services.AiAssistant.Contracts;

public sealed class AiChatRequestValidator : AbstractValidator<AiChatRequest>
{
    public AiChatRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(8000);
    }
}
