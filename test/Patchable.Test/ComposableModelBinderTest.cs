// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace Patchable.Test;

[TestClass]
public sealed class ComposableModelBinderTest
{
#pragma warning disable CS8618
  private Mock<ModelBindingContext> _modelBindingContextMock;

  private Mock<Action<object, object?>> _modelIdSetterMock;
  private Mock<Action<object, object?>> _modelNameSetterMock;

  private Mock<HttpContext> _httpContextMock;
  private Mock<HttpRequest> _httpRequestMock;

  private ComposableModelBinder _composableModelBinder;
#pragma warning restore CS8618

  [TestInitialize]
  public void Initialize()
  {
    _modelBindingContextMock = new();
    _modelBindingContextMock.SetupSet(context => context.Result = It.IsAny<ModelBindingResult>())
                            .Verifiable();

    _httpContextMock = new Mock<HttpContext>();
    _httpRequestMock = new Mock<HttpRequest>();

    _httpContextMock.SetupGet(context => context.Request)
                    .Returns(_httpRequestMock.Object)
                    .Verifiable();

    _modelBindingContextMock.SetupGet(context => context.HttpContext)
                            .Returns(_httpContextMock.Object)
                            .Verifiable();

    Mock<ModelMetadata> modelMetadataMock = new(
      ModelMetadataIdentity.ForType(typeof(TestComposableModel)));

    _modelIdSetterMock = new();
    _modelIdSetterMock.Setup(setter => setter.Invoke(It.IsAny<object>(), It.IsAny<object?>()))
                     .Verifiable();

    Mock<ModelMetadata> modelIdMetadataMock = new(
      ModelMetadataIdentity.ForProperty(
        typeof(TestComposableModel).GetProperty(nameof(TestComposableModel.Id))!,
        typeof(Guid),
        typeof(TestComposableModel)));

    modelIdMetadataMock.SetupGet(metadata => metadata.PropertySetter)
                       .Returns(_modelIdSetterMock.Object)
                       .Verifiable();

    _modelNameSetterMock = new();
    _modelNameSetterMock.Setup(setter => setter.Invoke(It.IsAny<object>(), It.IsAny<object?>()))
                        .Verifiable();

    Mock<ModelMetadata> modelNameMetadataMock = new(
      ModelMetadataIdentity.ForProperty(
        typeof(TestComposableModel).GetProperty(nameof(TestComposableModel.Name))!,
        typeof(string),
        typeof(TestComposableModel)));

    modelNameMetadataMock.SetupGet(metadata => metadata.PropertySetter)
                         .Returns(_modelNameSetterMock.Object)
                         .Verifiable();

    ModelPropertyCollection properties = new(
      new[]
      {
          modelIdMetadataMock.Object,
          modelNameMetadataMock.Object,
      });

    modelMetadataMock.Setup(metadata => metadata.Properties)
                     .Returns(properties)
                     .Verifiable();

    _modelBindingContextMock.SetupGet(context => context.ModelMetadata)
                            .Returns(modelMetadataMock.Object)
                            .Verifiable();

    _modelBindingContextMock.SetupGet(context => context.ModelType)
                            .Returns(modelMetadataMock.Object.ModelType)
                            .Verifiable();

    _composableModelBinder = new ComposableModelBinder();
  }
}
