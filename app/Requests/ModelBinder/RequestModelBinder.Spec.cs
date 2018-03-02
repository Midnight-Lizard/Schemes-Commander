using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MidnightLizard.Schemes.Commander.Requests.ModelBinder;
using MidnightLizard.Schemes.Commander.Requests.Serialization;
using MidnightLizard.Testing.Utilities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class RequestModelBinderSpec
    {
        private readonly RequestSerializer requestSerializer;
        private readonly RequestVersionAccessor versionAccessor;
        private readonly RequestBodyAccessor bodyAccessor;
        private readonly RequestModelBinder modelBinder;
        private readonly ModelBindingContext context;
        private readonly ApiVersion testApiVersion = new ApiVersion(1, 3);
        private readonly string testBody = "test";

        public RequestModelBinderSpec()
        {
            this.requestSerializer = Substitute.For<RequestSerializer>();
            this.versionAccessor = Substitute.For<RequestVersionAccessor>();
            this.bodyAccessor = Substitute.For<RequestBodyAccessor>();
            this.modelBinder = new RequestModelBinder(this.requestSerializer, this.versionAccessor, this.bodyAccessor);
            this.context = Substitute.For<ModelBindingContext>();

            this.context.ModelType.Returns(typeof(PublishSchemeRequest));
            this.versionAccessor.GetApiVersion(this.context).Returns(this.testApiVersion);
            this.bodyAccessor.ReadAsync(this.context).Returns(this.testBody);
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

            this.requestSerializer.Received(1)
                .Deserialize(typeof(PublishSchemeRequest), this.testApiVersion, this.testBody);
        }
    }
}
