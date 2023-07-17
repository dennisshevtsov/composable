// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Moq;

namespace Patchable.Test;

[TestClass]
public sealed class PatchableModelBinderProviderTest
{
#pragma warning disable CS8618
  private PatchableModelBinderProvider _patchableModelBinderProvider;
#pragma warning restore CS8618

  [TestInitialize]
  public void Initialize()
  {
    _patchableModelBinderProvider = new PatchableModelBinderProvider();
  }

  [TestMethod]
  public void GetBinder_ModelNotInheritIPatchable_ReturnNull()
  {
    // Arrange
    Mock<ModelBinderProviderContext> modelBinderProviderContextMock = new();
    Mock<ModelMetadata> modelMetadataMock = new(
        ModelMetadataIdentity.ForType(typeof(TestUnpatchableModel)));

    modelMetadataMock.SetupGet(metadata => metadata.Properties)
                     .Returns(new ModelPropertyCollection(Array.Empty<ModelMetadata>()));

    modelBinderProviderContextMock.SetupGet(context => context.Metadata)
                                  .Returns(modelMetadataMock.Object)
                                  .Verifiable();

    // Act
    IModelBinder? modelBinder = _patchableModelBinderProvider.GetBinder(
      modelBinderProviderContextMock.Object);

    // Assert
    Assert.IsNull(modelBinder);
  }

  [TestMethod]
  public void GetBinder_ModelInheritsIPatchable_ReturnPatchableBinder()
  {
    // Arrange
    Mock<ModelBinderProviderContext> modelBinderProviderContextMock = new();
    Mock<ModelMetadata> modelMetadataMock = new(
        ModelMetadataIdentity.ForType(typeof(TestPatchableModel)));

    modelMetadataMock.SetupGet(metadata => metadata.Properties)
                     .Returns(new ModelPropertyCollection(Array.Empty<ModelMetadata>()));

    modelBinderProviderContextMock.SetupGet(context => context.Metadata)
                                  .Returns(modelMetadataMock.Object)
                                  .Verifiable();

    // Act
    IModelBinder? modelBinder = _patchableModelBinderProvider.GetBinder(
      modelBinderProviderContextMock.Object);

    // Assert
    Assert.IsNotNull(modelBinder);
    Assert.IsInstanceOfType<PatchableModelBinder>(modelBinder);
  }
}
