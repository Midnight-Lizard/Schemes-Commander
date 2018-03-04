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
        protected override void AdvanceToTheLatestVersion(PublishSchemeRequest request)
        {
            // version 1.0 does not have scrollbar size and image hover options
            var cs = request.ColorScheme;
            cs.scrollbarSize = 10;//px
            cs.useImageHoverAnimation = cs.imageLightnessLimit > 80;

            base.AdvanceToTheLatestVersion(request);
        }
    }

    [ApiVersion("1.1")]
    public class PublishSchemeRequestDeserializer_v1_1 : PublishSchemeRequestDeserializer_v1_2
    {
        protected override void AdvanceToTheLatestVersion(PublishSchemeRequest request)
        {
            // version 1.1 does not have button component
            var cs = request.ColorScheme;
            cs.buttonSaturationLimit = (int)Math.Round(Math.Min(cs.backgroundSaturationLimit * 1.1, 100));
            cs.buttonContrast = cs.backgroundContrast;
            cs.buttonLightnessLimit = (int)Math.Round(cs.backgroundLightnessLimit * 0.8);
            cs.buttonGraySaturation = (int)Math.Round(Math.Min(cs.backgroundGraySaturation * 1.1, 100));
            cs.buttonGrayHue = cs.borderGrayHue;

            base.AdvanceToTheLatestVersion(request);
        }
    }

    [ApiVersion("1.2")]
    public class PublishSchemeRequestDeserializer_v1_2 : PublishSchemeRequestDeserializer_Latest
    {
        protected override void AdvanceToTheLatestVersion(PublishSchemeRequest request)
        {
            // version 1.2 does not have option to ignore color hues
            var cs = request.ColorScheme;
            cs.linkReplaceAllHues = true;

            base.AdvanceToTheLatestVersion(request);
        }
    }

    [ApiVersion("1.3")]
    public class PublishSchemeRequestDeserializer_Latest : GenericRequestDeserializer<PublishSchemeRequest>
    {
    }
}
