using FluentValidation.TestHelper;
using MidnightLizard.Testing.Utilities;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate
{
    public class ImpressionsObjectTypeSpec
    {
        public class ValidatorSpec
        {
            private readonly ImpressionsObjectTypeValidator validator = new ImpressionsObjectTypeValidator();

            [It(nameof(ImpressionsObjectTypeValidator))]
            public void Should_fail_when_Value_is_null_or_empty()
            {
                this.validator.ShouldHaveValidationErrorFor(x => x.Value, null as string);
                this.validator.ShouldHaveValidationErrorFor(x => x.Value, string.Empty);
            }

            [It(nameof(ImpressionsObjectTypeValidator))]
            public void Should_fail_when_Value_is_too_long()
            {
                this.validator.ShouldHaveValidationErrorFor(x => x.Value, new string('*', 51));
            }
        }
    }
}
