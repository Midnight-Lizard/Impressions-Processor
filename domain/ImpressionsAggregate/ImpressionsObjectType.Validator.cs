using FluentValidation;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate
{
    public class ImpressionsObjectTypeValidator : AbstractValidator<ImpressionsObjectType>
    {
        public ImpressionsObjectTypeValidator()
        {
            RuleFor(x => x.Value).NotEmpty().MaximumLength(50);
        }
    }
}
