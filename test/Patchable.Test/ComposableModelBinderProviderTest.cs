// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Moq;

namespace Patchable.Test;

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
}
