using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MidnightLizard.Schemes.Commander.Infrastructure.ModelBinding;
using MidnightLizard.Schemes.Commander.Requests.PublishScheme;
using MidnightLizard.Schemes.Commander.Infrastructure.Serialization;
using MidnightLizard.Testing.Utilities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Infrastructure.ModelBinding
{
    public class RequestModelBinderSpec
    {
        private readonly IRequestMetaDeserializer requestMetaDeserializer;
        private readonly RequestVersionAccessor versionAccessor;
        private readonly RequestBodyAccessor bodyAccessor;
        private readonly RequestModelBinder modelBinder;
        private readonly ModelBindingContext context;
        private readonly ApiVersion testApiVersion = new ApiVersion(1, 3);
        private readonly string testBody = "test";

        public RequestModelBinderSpec()
        {
            this.requestMetaDeserializer = Substitute.For<IRequestMetaDeserializer>();
            this.versionAccessor = Substitute.For<RequestVersionAccessor>();
            this.bodyAccessor = Substitute.For<RequestBodyAccessor>();
            this.modelBinder = new RequestModelBinder(this.requestMetaDeserializer, this.versionAccessor, this.bodyAccessor);
            this.context = Substitute.For<ModelBindingContext>();

            this.context.ModelType.Returns(typeof(PublishSchemeRequest));
            this.versionAccessor.GetApiVersion(this.context).Returns(this.testApiVersion);
            this.bodyAccessor.ReadAsync(this.context).Returns(this.testBody);
            this.context.ModelState = new ModelStateDictionary();
        }

        [It(nameof(RequestModelBinder.BindModelAsync))]
        public async Task Should_read_ModelType()
        {
            await this.modelBinder.BindModelAsync(this.context);

            var test = this.context.Received(1).ModelType;
        }

        [It(nameof(RequestModelBinder.BindModelAsync))]
        public async Task Should_read_ApiVersion()
        {
            await this.modelBinder.BindModelAsync(this.context);

            this.versionAccessor.Received(1).GetApiVersion(this.context);
        }

        [It(nameof(RequestModelBinder.BindModelAsync))]
        public async Task Should_read_Request__Body()
        {
            await this.modelBinder.BindModelAsync(this.context);

            await this.bodyAccessor.Received(1).ReadAsync(this.context);
        }

        [It(nameof(RequestModelBinder.BindModelAsync))]
        public async Task Should_call_RequestSerializer()
        {
            await this.modelBinder.BindModelAsync(this.context);

            this.requestMetaDeserializer.Received(1)
                .Deserialize(typeof(PublishSchemeRequest), this.testApiVersion, this.testBody);
        }

        [It(nameof(RequestModelBinder.BindModelAsync))]
        public async Task Should_set_ModelBindingContext__Result_to_Success()
        {
            await this.modelBinder.BindModelAsync(this.context);

            this.context.Result.IsModelSet.Should().BeTrue();
        }

        [It(nameof(RequestModelBinder.BindModelAsync))]
        public async Task Should_set_ModelBindingContext__Result_to_Fail_when_Exception()
        {
            this.requestMetaDeserializer.Deserialize(typeof(PublishSchemeRequest), this.testApiVersion, this.testBody)
                .Returns(x => throw new Exception("test"));

            await this.modelBinder.BindModelAsync(this.context);

            this.context.Result.IsModelSet.Should().BeFalse();
        }

        [It(nameof(RequestModelBinder.BindModelAsync))]
        public async Task Should_add_Error_to_ModelState_when_Exception_raised()
        {
            var testErrorMessage = "test error message";
            this.requestMetaDeserializer.Deserialize(typeof(PublishSchemeRequest), this.testApiVersion, this.testBody)
                .Returns(x => throw new Exception(testErrorMessage));

            await this.modelBinder.BindModelAsync(this.context);

            this.context.ModelState.Should().HaveCount(1);
            var firstState = this.context.ModelState.First();
            firstState.Value.Errors.Should().HaveCount(1);
            firstState.Value.Errors[0].ErrorMessage.Should().Be(testErrorMessage);
        }
    }
}
