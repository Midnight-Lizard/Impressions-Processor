using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Requests;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Deserializers
{
    [Message(Version = "1.0")]
    public class PublishSchemeRequestDeserializer_v1_0 : PublishSchemeRequestDeserializer_v1_1
    {
        public override void StartAdvancingToTheLatestVersion(PublishSchemeRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }
    }

    [Message(Version = "1.1")]
    public class PublishSchemeRequestDeserializer_v1_1 : PublishSchemeRequestDeserializer_v1_2
    {
        public override void StartAdvancingToTheLatestVersion(PublishSchemeRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }

        protected override void AdvanceToTheLatestVersion(PublishSchemeRequest message)
        {
            // in version 1.1 scrollbar size and image hover options are added
            var cs = message.ColorScheme;
            cs.scrollbarSize = 10;//px
            cs.useImageHoverAnimation = cs.imageLightnessLimit > 80;

            base.AdvanceToTheLatestVersion(message);
        }
    }

    [Message(Version = "1.2")]
    public class PublishSchemeRequestDeserializer_v1_2 : PublishSchemeRequestDeserializer_v1_3
    {
        public override void StartAdvancingToTheLatestVersion(PublishSchemeRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }

        protected override void AdvanceToTheLatestVersion(PublishSchemeRequest message)
        {
            // in version 1.2 button component is added
            var cs = message.ColorScheme;
            cs.buttonSaturationLimit = (int)Math.Round(Math.Min(cs.backgroundSaturationLimit * 1.1, 100));
            cs.buttonContrast = cs.backgroundContrast;
            cs.buttonLightnessLimit = (int)Math.Round(cs.backgroundLightnessLimit * 0.8);
            cs.buttonGraySaturation = (int)Math.Round(Math.Min(cs.backgroundGraySaturation * 1.1, 100));
            cs.buttonGrayHue = cs.borderGrayHue;

            base.AdvanceToTheLatestVersion(message);
        }
    }

    [Message(Version = ">=1.3 <9.3")]
    public class PublishSchemeRequestDeserializer_v1_3 : PublishSchemeRequestDeserializer_v9_3
    {
        public override void StartAdvancingToTheLatestVersion(PublishSchemeRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }

        protected override void AdvanceToTheLatestVersion(PublishSchemeRequest message)
        {
            // in version 1.3 option to ignore color hues is added
            var cs = message.ColorScheme;
            cs.linkReplaceAllHues = true;

            base.AdvanceToTheLatestVersion(message);
        }
    }

    [Message(Version = ">=9.3 <10.1")]
    public class PublishSchemeRequestDeserializer_v9_3 : PublishSchemeRequestDeserializer_v10_1
    {
        public override void StartAdvancingToTheLatestVersion(PublishSchemeRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }

        protected override void AdvanceToTheLatestVersion(PublishSchemeRequest message)
        {
            var cs = message.ColorScheme;

            cs.doNotInvertContent = false;
            cs.mode = ColorSchemeMode.Auto;
            cs.modeAutoSwitchLimit = 5000;
            cs.includeMatches = null;
            cs.excludeMatches = null;
            cs.backgroundHueGravity = 0;
            cs.buttonHueGravity = 0;
            cs.textHueGravity = 0;
            cs.linkHueGravity = 80;
            cs.borderHueGravity = 0;
            cs.scrollbarStyle = "true";

            base.AdvanceToTheLatestVersion(message);
        }
    }

    [Message(Version = ">=10.1")]
    public class PublishSchemeRequestDeserializer_v10_1 : AbstractMessageDeserializer<PublishSchemeRequest, PublicSchemeId>
    {
        public override void StartAdvancingToTheLatestVersion(PublishSchemeRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }

        protected override void AdvanceToTheLatestVersion(PublishSchemeRequest message)
        {
            var cs = message.ColorScheme;

            cs.hideBigBackgroundImages = true;
            cs.maxBackgroundImageSize = 500;

            base.AdvanceToTheLatestVersion(message);
        }
    }
}
