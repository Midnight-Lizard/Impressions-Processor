namespace MidnightLizard.SImpressionsDomain.PublicSchemeAggregate.Events
{
    public class SchemePublishedEvent : SchemeDomainEvent
    {
        public string Description { get; set; }
        public ColorScheme ColorScheme { get; private set; }

        protected SchemePublishedEvent() { }

        public SchemePublishedEvent(PublicSchemeId aggregateId, ColorScheme colorScheme, string description)
            : base(aggregateId)
        {
            this.ColorScheme = colorScheme;
            this.Description = description;
        }
    }
}
