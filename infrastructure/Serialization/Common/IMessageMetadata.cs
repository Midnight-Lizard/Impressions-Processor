using SemVer;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Common
{
    public interface IMessageMetadata
    {
        string Type { get; set; }
        Range VersionRange { get; set; }
    }
}