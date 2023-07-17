﻿// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

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
}
