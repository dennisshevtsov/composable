// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using System.Text.Json;

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
  private Mock<ModelBindingContext> _modelBindingContextMock;

  private Mock<Action<object, object?>> _modelIdSetterMock;
  private Mock<Action<object, object?>> _modelNameSetterMock;

  private Mock<HttpContext> _httpContextMock;
  private Mock<HttpRequest> _httpRequestMock;

  private PatchableModelBinder _patchableModelBinder;
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
      ModelMetadataIdentity.ForType(typeof(TestPatchableModel)));

    _modelIdSetterMock = new();
    _modelIdSetterMock.Setup(setter => setter.Invoke(It.IsAny<object>(), It.IsAny<object?>()))
                     .Verifiable();

    Mock<ModelMetadata> modelIdMetadataMock = new(
      ModelMetadataIdentity.ForProperty(
        typeof(TestPatchableModel).GetProperty(nameof(TestPatchableModel.Id))!,
        typeof(Guid),
        typeof(TestPatchableModel)));

    modelIdMetadataMock.SetupGet(metadata => metadata.PropertySetter)
                       .Returns(_modelIdSetterMock.Object)
                       .Verifiable();

    _modelNameSetterMock = new();
    _modelNameSetterMock.Setup(setter => setter.Invoke(It.IsAny<object>(), It.IsAny<object?>()))
                        .Verifiable();

    Mock<ModelMetadata> modelNameMetadataMock = new(
      ModelMetadataIdentity.ForProperty(
        typeof(TestPatchableModel).GetProperty(nameof(TestPatchableModel.Name))!,
        typeof(string),
        typeof(TestPatchableModel)));

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

    _patchableModelBinder = new PatchableModelBinder();
  }

  [TestMethod]
  public async Task BindModelAsync_NoRouteNoQueryNoBody_SetSuccessResult()
  {
    // Arrange

    // No route params
    _modelBindingContextMock.SetupGet(context => context.ActionContext)
                            .Returns(new ActionContext()
                            {
                              RouteData = new RouteData(),
                            });

    // No query string params
    _httpRequestMock.SetupGet(request => request.Query)
                    .Returns(new QueryCollection());

    // No body
    _httpRequestMock.SetupGet(request => request.ContentLength)
                    .Returns(0);

    // Act
    await _patchableModelBinder.BindModelAsync(_modelBindingContextMock.Object);

    // Assert
    _modelBindingContextMock.VerifySet(context => context.Result = It.Is<ModelBindingResult>(result => result.IsModelSet));
  }

  [TestMethod]
  public async Task BindModelAsync_RouteParams_SetPropertiesFromRoute()
  {
    // Arrange
    TestPatchableModel model = new()
    {
      Id   = Guid.NewGuid(),
      Name = Guid.NewGuid().ToString(),
    };

    // Set up route params
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

    _modelBindingContextMock.SetupGet(context => context.ActionContext)
                            .Returns(actionContext);

    // No query params
    _httpRequestMock.SetupGet(context => context.Query)
                    .Returns(new QueryCollection());

    // No body
    _httpRequestMock.SetupGet(request => request.ContentLength)
                    .Returns(0);

    // Act
    await _patchableModelBinder.BindModelAsync(_modelBindingContextMock.Object);

    // Assert
    _modelIdSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((Guid)x) == model.Id)));
    _modelNameSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((string)x) == model.Name)));
  }

  [TestMethod]
  public async Task BindModelAsync_QueryStringParams_SetPropertiesFromString()
  {
    // Arrange
    TestPatchableModel model = new()
    {
      Id = Guid.NewGuid(),
      Name = Guid.NewGuid().ToString(),
    };

    // No route params
    _modelBindingContextMock.SetupGet(context => context.ActionContext)
                            .Returns(new ActionContext()
                            {
                                RouteData = new RouteData(),
                            })
                            .Verifiable();

    // Set up query string
    QueryCollection queryCollection = new(
      new Dictionary<string, StringValues>
      {
        { nameof(TestPatchableModel.Id)  , model.Id.ToString() },
        { nameof(TestPatchableModel.Name), model.Name          },
      });

    _httpRequestMock.SetupGet(context => context.Query)
                    .Returns(queryCollection)
                    .Verifiable();

    // No body
    _httpRequestMock.SetupGet(request => request.ContentLength)
                    .Returns(0);

    // Act
    await _patchableModelBinder.BindModelAsync(_modelBindingContextMock.Object);

    // Assert
    _modelIdSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((Guid)x) == model.Id)));
    _modelNameSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((string)x) == model.Name)));
  }

  [TestMethod]
  public async Task BindModelAsync_Body_SetPropertiesFromBody()
  {
    // Arrange
    TestPatchableModel model = new()
    {
      Id = Guid.NewGuid(),
      Name = Guid.NewGuid().ToString(),
    };

    // No route params
    _modelBindingContextMock.SetupGet(context => context.ActionContext)
                            .Returns(new ActionContext()
                            {
                              RouteData = new RouteData(),
                            })
                            .Verifiable();

    // No query params
    _httpRequestMock.SetupGet(context => context.Query)
                    .Returns(new QueryCollection())
                    .Verifiable();
 
    // Set up body
    MemoryStream stream = new();
    JsonSerializerOptions options = new()
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    await JsonSerializer.SerializeAsync(stream, model, options);
    stream.Position = 0;

    _httpRequestMock.SetupGet(context => context.Body)
                    .Returns(stream)
                    .Verifiable();

    _httpRequestMock.SetupGet(context => context.ContentLength)
                    .Returns(stream.Length)
                    .Verifiable();

    // Act
    await _patchableModelBinder.BindModelAsync(_modelBindingContextMock.Object);

    // Assert
    _modelIdSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((Guid)x) == model.Id)));
    _modelNameSetterMock.Verify(setter => setter.Invoke(It.IsAny<object>(), It.Is<object>(x => ((string)x) == model.Name)));
  }
}
