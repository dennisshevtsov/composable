// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Patchable.Test;

[TestClass]
public sealed class PatchableModelBinderTest
{
#pragma warning disable CS8618
  private PatchableModelBinder _patchableModelBinder;
#pragma warning restore CS8618

  [TestInitialize]
  public void Initialize()
  {
    _patchableModelBinder = new PatchableModelBinder();
  }

  [TestMethod]
  public async Task BindModelAsync_NoBody_SetSuccessResult()
  {
    // Arrange
    Mock<ModelBindingContext> modelBindingContextMock = new();
    modelBindingContextMock.SetupSet(context => context.Result = It.IsAny<ModelBindingResult>())
                           .Verifiable();

    var modelId   = Guid.NewGuid().ToString();
    var modelName = Guid.NewGuid().ToString();

    var actionContext = new ActionContext
    {
      RouteData = new RouteData
      {
        Values = {
          { nameof(TestPatchableModel.Id)  , modelId   },
          { nameof(TestPatchableModel.Name), modelName },
        },
      },
    };

    modelBindingContextMock.SetupGet(context => context.ActionContext)
                           .Returns(actionContext)
                           .Verifiable();

    var modelMetadataMock = new Mock<ModelMetadata>(
      ModelMetadataIdentity.ForType(typeof(TestPatchableModel)));

    var modelIdMetadataMock = new Mock<ModelMetadata>(
      ModelMetadataIdentity.ForProperty(
        typeof(TestPatchableModel).GetProperty(nameof(TestPatchableModel.Id))!,
        typeof(Guid),
        typeof(TestPatchableModel)));

    modelIdMetadataMock.SetupGet(metadata => metadata.PropertySetter)
                        .Returns((object a, object? b) => { })
                        .Verifiable();

    var modelNameMetadataMock = new Mock<ModelMetadata>(
      ModelMetadataIdentity.ForProperty(
        typeof(TestPatchableModel).GetProperty(nameof(TestPatchableModel.Name))!,
        typeof(Guid),
        typeof(TestPatchableModel)));

    modelNameMetadataMock.SetupGet(metadata => metadata.PropertySetter)
                        .Returns((object a, object? b) => { })
                        .Verifiable();

    var properties = new ModelPropertyCollection(
      new[]
      {
          modelIdMetadataMock.Object,
          modelNameMetadataMock.Object,
      });

    modelMetadataMock.Setup(metadata => metadata.Properties)
                     .Returns(properties)
                     .Verifiable();

    modelBindingContextMock.SetupGet(context => context.ModelMetadata)
                           .Returns(modelMetadataMock.Object)
                           .Verifiable();

    modelBindingContextMock.SetupGet(context => context.ModelType)
                           .Returns(modelMetadataMock.Object.ModelType)
                           .Verifiable();

    Mock<HttpContext> httpContextMock = new();
    Mock<HttpRequest> httpRequestMock = new();

    httpContextMock.SetupGet(context => context.Request)
                   .Returns(httpRequestMock.Object)
                   .Verifiable();

    httpRequestMock.SetupGet(context => context.Query)
                   .Returns(new QueryCollection())
                   .Verifiable();

    modelBindingContextMock.SetupGet(context => context.HttpContext)
                           .Returns(httpContextMock.Object)
                           .Verifiable();

    // Act
    await _patchableModelBinder.BindModelAsync(modelBindingContextMock.Object);

    // Assert
    modelBindingContextMock.VerifySet(context => context.Result = It.Is<ModelBindingResult>(result => result.IsModelSet));
  }
}
