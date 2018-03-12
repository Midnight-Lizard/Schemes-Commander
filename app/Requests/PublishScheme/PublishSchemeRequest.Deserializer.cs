using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Schemes.Commander.Infrastructure.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    [ApiVersion("1.0")]
    public class PublishSchemeRequestDeserializer_v1_0 : PublishSchemeRequestDeserializer_v1_1
    {
        public override void StartAdvancingToTheLatestVersion(PublishSchemeRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }
    }

    [ApiVersion("1.1")]
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

    [ApiVersion("1.2")]
    public class PublishSchemeRequestDeserializer_v1_2 : PublishSchemeRequestDeserializer_Latest
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

    [ApiVersion("1.3")]
    public class PublishSchemeRequestDeserializer_Latest : JsonRequestDeserializer<PublishSchemeRequest>
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
}
