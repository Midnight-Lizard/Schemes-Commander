using FluentAssertions;
using MidnightLizard.Schemes.Commander.Requests.PublishScheme;
using MidnightLizard.Testing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.Serialization
{
    public class RequestSerializerSpec
    {
        private readonly RequestSerializer serialiser;
        private readonly Guid
            id = Guid.NewGuid(),
            aggregateId = Guid.NewGuid(),
            publisherId = Guid.NewGuid();
        private readonly string snapshot;

        public RequestSerializerSpec()
        {
            this.serialiser = new RequestSerializer(AppVersion.Latest);
            snapshot =
                $@"{{" +
                    $@"""CorrelationId"":""{id}""," +
                    $@"""Type"":""PublishSchemeRequest""," +
                    $@"""Version"":""{AppVersion.Latest}""," +
                    $@"""Payload"":" +
                    $@"{{" +
                        $@"""AggregateId"":""{aggregateId}""," +
                        $@"""Id"":""{id}""," +
                        $@"""PublisherId"":""{publisherId}""," +
                        $@"""ColorScheme"":{{""colorSchemeId"":""almondRipe"",""colorSchemeName"":""Almond Ripe"",""runOnThisSite"":true,""restoreColorsOnCopy"":false,""blueFilter"":5,""useDefaultSchedule"":true,""scheduleStartHour"":0,""scheduleFinishHour"":24,""backgroundSaturationLimit"":80,""backgroundContrast"":50,""backgroundLightnessLimit"":11,""backgroundGraySaturation"":30,""backgroundGrayHue"":36,""backgroundReplaceAllHues"":false,""textSaturationLimit"":90,""textContrast"":60,""textLightnessLimit"":80,""textGraySaturation"":10,""textGrayHue"":88,""textSelectionHue"":36,""textReplaceAllHues"":false,""linkSaturationLimit"":80,""linkContrast"":50,""linkLightnessLimit"":70,""linkDefaultSaturation"":60,""linkDefaultHue"":88,""linkVisitedHue"":122,""linkReplaceAllHues"":true,""borderSaturationLimit"":80,""borderContrast"":30,""borderLightnessLimit"":50,""borderGraySaturation"":20,""borderGrayHue"":54,""borderReplaceAllHues"":false,""imageLightnessLimit"":80,""imageSaturationLimit"":90,""backgroundImageLightnessLimit"":40,""backgroundImageSaturationLimit"":80,""scrollbarSaturationLimit"":20,""scrollbarContrast"":0,""scrollbarLightnessLimit"":40,""scrollbarGrayHue"":45,""buttonSaturationLimit"":90,""buttonContrast"":50,""buttonLightnessLimit"":30,""buttonGraySaturation"":30,""buttonGrayHue"":40,""buttonReplaceAllHues"":false,""useImageHoverAnimation"":false,""scrollbarSize"":10}}" +
                    $@"}}" +
                $@"}}";
        }

        [It(nameof(RequestSerializer))]
        public void Should_serialize_request_the_same_way_as_in_snapshot()
        {
            var testRequest = PublishSchemeRequestSpec.CorrectPublishSchemeRequest;

            testRequest.AggregateId = this.aggregateId;
            testRequest.Id = this.id;
            testRequest.PublisherId = this.publisherId;

            var json = this.serialiser.Serialize(testRequest);
            json.Should().Be(this.snapshot);
        }
    }
}
