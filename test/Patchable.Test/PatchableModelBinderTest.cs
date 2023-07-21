// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
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

    ActionContext actionContext = new()
    {
      RouteData = new RouteData(),
    };

    modelBindingContextMock.SetupGet(context => context.ActionContext)
                           .Returns(actionContext)
                           .Verifiable();

    Mock<ModelMetadata> modelMetadataMock = new(
      ModelMetadataIdentity.ForType(typeof(TestPatchableModel)));

    modelMetadataMock.Setup(metadata => metadata.Properties)
                     .Returns(new ModelPropertyCollection(Array.Empty<ModelMetadata>()))
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

  [TestMethod]
  public async Task BindModelAsync_RouteParams_SetPropertiesFromRoute()
  {
    // Arrange
    Mock<ModelBindingContext> modelBindingContextMock = new();
    modelBindingContextMock.SetupSet(context => context.Result = It.IsAny<ModelBindingResult>())
                           .Verifiable();

    TestPatchableModel model = new()
    {
      Id   = Guid.NewGuid(),
      Name = Guid.NewGuid().ToString(),
    };

    ActionContext actionContext = new()
    {
      RouteData = new RouteData
      {
        Values = {
          { nameof(TestPatchableModel.Id)  , model.Id.ToString() },
          { nameof(TestPatchableModel.Name), model.Name          },
        },
      },
    };

    modelBindingContextMock.SetupGet(context => context.ActionContext)
                           .Returns(actionContext)
                           .Verifiable();

    Mock<ModelMetadata> modelMetadataMock = new(
      ModelMetadataIdentity.ForType(typeof(TestPatchableModel)));

    Mock<Action<object, object?>> modelIdSetterMock = new();
    modelIdSetterMock.Setup(setter => setter.Invoke(It.IsAny<object>(), It.IsAny<object?>()))
                     .Verifiable();

    Mock<ModelMetadata> modelIdMetadataMock = new(
      ModelMetadataIdentity.ForProperty(
        typeof(TestPatchableModel).GetProperty(nameof(TestPatchableModel.Id))!,
        typeof(Guid),
        typeof(TestPatchableModel)));

    modelIdMetadataMock.SetupGet(metadata => metadata.PropertySetter)
                       .Returns(modelIdSetterMock.Object)
                       .Verifiable();

    Mock<Action<object, object?>> modelNameSetterMock = new();
    modelIdSetterMock.Setup(setter => setter.Invoke(It.IsAny<object>(), It.IsAny<object?>()))
                     .Verifiable();

    Mock<ModelMetadata> modelNameMetadataMock = new(
      ModelMetadataIdentity.ForProperty(
        typeof(TestPatchableModel).GetProperty(nameof(TestPatchableModel.Name))!,
        typeof(string),
        typeof(TestPatchableModel)));

    modelNameMetadataMock.SetupGet(metadata => metadata.PropertySetter)
                         .Returns(modelNameSetterMock.Object)
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
    modelIdSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((Guid)x) == model.Id)));
    modelNameSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((string)x) == model.Name)));
  }

  [TestMethod]
  public async Task BindModelAsync_QueryStringParams_SetPropertiesFromString()
  {
    // Arrange
    Mock<ModelBindingContext> modelBindingContextMock = new();
    modelBindingContextMock.SetupSet(context => context.Result = It.IsAny<ModelBindingResult>())
                           .Verifiable();

    TestPatchableModel model = new()
    {
      Id = Guid.NewGuid(),
      Name = Guid.NewGuid().ToString(),
    };

    ActionContext actionContext = new()
    {
      RouteData = new RouteData(),
    };

    modelBindingContextMock.SetupGet(context => context.ActionContext)
                           .Returns(actionContext)
                           .Verifiable();

    Mock<ModelMetadata> modelMetadataMock = new(
      ModelMetadataIdentity.ForType(typeof(TestPatchableModel)));

    Mock<Action<object, object?>> modelIdSetterMock = new();
    modelIdSetterMock.Setup(setter => setter.Invoke(It.IsAny<object>(), It.IsAny<object?>()))
                     .Verifiable();

    Mock<ModelMetadata> modelIdMetadataMock = new(
      ModelMetadataIdentity.ForProperty(
        typeof(TestPatchableModel).GetProperty(nameof(TestPatchableModel.Id))!,
        typeof(Guid),
        typeof(TestPatchableModel)));

    modelIdMetadataMock.SetupGet(metadata => metadata.PropertySetter)
                       .Returns(modelIdSetterMock.Object)
                       .Verifiable();

    Mock<Action<object, object?>> modelNameSetterMock = new();
    modelIdSetterMock.Setup(setter => setter.Invoke(It.IsAny<object>(), It.IsAny<object?>()))
                     .Verifiable();

    Mock<ModelMetadata> modelNameMetadataMock = new(
      ModelMetadataIdentity.ForProperty(
        typeof(TestPatchableModel).GetProperty(nameof(TestPatchableModel.Name))!,
        typeof(string),
        typeof(TestPatchableModel)));

    modelNameMetadataMock.SetupGet(metadata => metadata.PropertySetter)
                         .Returns(modelNameSetterMock.Object)
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

    QueryCollection queryCollection = new(new Dictionary<string, StringValues>
    {
      { nameof(TestPatchableModel.Id)  , model.Id.ToString() },
      { nameof(TestPatchableModel.Name), model.Name          },
    });

    httpRequestMock.SetupGet(context => context.Query)
                   .Returns(queryCollection)
                   .Verifiable();

    modelBindingContextMock.SetupGet(context => context.HttpContext)
                           .Returns(httpContextMock.Object)
                           .Verifiable();

    // Act
    await _patchableModelBinder.BindModelAsync(modelBindingContextMock.Object);

    // Assert
    modelIdSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((Guid)x) == model.Id)));
    modelNameSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((string)x) == model.Name)));
  }

  [TestMethod]
  public async Task BindModelAsync_Body_SetPropertiesFromBody()
  {
    // Arrange
    Mock<ModelBindingContext> modelBindingContextMock = new();
    modelBindingContextMock.SetupSet(context => context.Result = It.IsAny<ModelBindingResult>())
                           .Verifiable();

    TestPatchableModel model = new()
    {
      Id = Guid.NewGuid(),
      Name = Guid.NewGuid().ToString(),
    };

    ActionContext actionContext = new()
    {
      RouteData = new RouteData(),
    };

    modelBindingContextMock.SetupGet(context => context.ActionContext)
                           .Returns(actionContext)
                           .Verifiable();

    Mock<ModelMetadata> modelMetadataMock = new(
      ModelMetadataIdentity.ForType(typeof(TestPatchableModel)));

    Mock<Action<object, object?>> modelIdSetterMock = new();
    modelIdSetterMock.Setup(setter => setter.Invoke(It.IsAny<object>(), It.IsAny<object?>()))
                     .Verifiable();

    Mock<ModelMetadata> modelIdMetadataMock = new(
      ModelMetadataIdentity.ForProperty(
        typeof(TestPatchableModel).GetProperty(nameof(TestPatchableModel.Id))!,
        typeof(Guid),
        typeof(TestPatchableModel)));

    modelIdMetadataMock.SetupGet(metadata => metadata.PropertySetter)
                       .Returns(modelIdSetterMock.Object)
                       .Verifiable();

    Mock<Action<object, object?>> modelNameSetterMock = new();
    modelIdSetterMock.Setup(setter => setter.Invoke(It.IsAny<object>(), It.IsAny<object?>()))
                     .Verifiable();

    Mock<ModelMetadata> modelNameMetadataMock = new(
      ModelMetadataIdentity.ForProperty(
        typeof(TestPatchableModel).GetProperty(nameof(TestPatchableModel.Name))!,
        typeof(string),
        typeof(TestPatchableModel)));

    modelNameMetadataMock.SetupGet(metadata => metadata.PropertySetter)
                         .Returns(modelNameSetterMock.Object)
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
    modelIdSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((Guid)x) == model.Id)));
    modelNameSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((string)x) == model.Name)));
  }
}
