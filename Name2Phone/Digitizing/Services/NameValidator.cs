using FluentValidation;

namespace Digitizing;

public class NameValidator: AbstractValidator<string>
{
    public NameValidator()
    {
        RuleFor(s=>s).Length(13).WithMessage("Must be 13 alpha-num characters long");
        RuleFor(s=>s).Matches("\\d{3,3}[a-zA-Z]{10,10}").WithMessage("In format nnnAAABBBCCCC");
        RuleFor(s=>s).NotEmpty().WithMessage("Name cannot be empty");
    }
}
