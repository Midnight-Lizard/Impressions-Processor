using MidnightLizard.Commons.Domain.Model;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate
{
    public class ImpressionsObjectType : SingleValueType<string>
    {
        public static readonly ImpressionsObjectTypeValidator Validator = new ImpressionsObjectTypeValidator();

        public ImpressionsObjectType() : base()
        {
        }

        public ImpressionsObjectType(string value) : base(value)
        {
        }
    }
}
