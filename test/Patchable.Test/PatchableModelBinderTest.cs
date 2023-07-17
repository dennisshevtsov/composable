// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

    // Act
    await _patchableModelBinder.BindModelAsync(modelBindingContextMock.Object);

    // Assert
    modelBindingContextMock.VerifySet(context => context.Result = It.Is<ModelBindingResult>(result => result.IsModelSet));
  }
}
