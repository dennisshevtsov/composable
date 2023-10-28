// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Moq;

namespace Composable.Test;

[TestClass]
public sealed class ComposableModelBinderProviderTest
{
#pragma warning disable CS8618
  private ComposableModelBinderProvider _composableModelBinderProvider;
#pragma warning restore CS8618

  [TestInitialize]
  public void Initialize()
  {
    _composableModelBinderProvider = new ComposableModelBinderProvider();
  }

  [TestMethod]
  public void GetBinder_ModelNotInheritIComposable_ReturnNull()
  {
    // Arrange
    Mock<ModelBinderProviderContext> modelBinderProviderContextMock = new();
    Mock<ModelMetadata> modelMetadataMock = new(
        ModelMetadataIdentity.ForType(typeof(TestUncomposableModel)));

    modelMetadataMock.SetupGet(metadata => metadata.Properties)
                     .Returns(new ModelPropertyCollection(Array.Empty<ModelMetadata>()));

    modelBinderProviderContextMock.SetupGet(context => context.Metadata)
                                  .Returns(modelMetadataMock.Object)
                                  .Verifiable();

    // Act
    IModelBinder? modelBinder = _composableModelBinderProvider.GetBinder(
      modelBinderProviderContextMock.Object);

    // Assert
    Assert.IsNull(modelBinder);
  }

  [TestMethod]
  public void GetBinder_ModelInheritsIComposable_ReturnPatchableBinder()
  {
    // Arrange
    Mock<ModelBinderProviderContext> modelBinderProviderContextMock = new();
    Mock<ModelMetadata> modelMetadataMock = new(
        ModelMetadataIdentity.ForType(typeof(TestComposableModel)));

    modelMetadataMock.SetupGet(metadata => metadata.Properties)
                     .Returns(new ModelPropertyCollection(Array.Empty<ModelMetadata>()));

    modelBinderProviderContextMock.SetupGet(context => context.Metadata)
                                  .Returns(modelMetadataMock.Object)
                                  .Verifiable();

    // Act
    IModelBinder? modelBinder = _composableModelBinderProvider.GetBinder(
      modelBinderProviderContextMock.Object);

    // Assert
    Assert.IsNotNull(modelBinder);
    Assert.IsInstanceOfType<ComposableModelBinder>(modelBinder);
  }
}
