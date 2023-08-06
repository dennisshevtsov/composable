// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;

using Patchable;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Contains the extension method to set up support of the <see cref="IPatchable"/> in the MVC pipeline.
/// </summary>
public static class PatchableServicesExtensions
{
  /// <summary>
  /// Sets up support of the <see cref="IComposable"/> and the <see cref="IPatchable"/> in the MVC pipeline.
  /// </summary>
  /// <param name="options">An instance of the <see cref="MvcOptions"/> that provides programmatic configuration for the MVC framework.</param>
  public static void AddPatchable(this MvcOptions options)
  {
    options.ModelBinderProviders.Insert(0, new PatchableModelBinderProvider());
    options.ModelBinderProviders.Insert(1, new ComposableModelBinderProvider());

    options.Filters.Insert(0, new SuppressValidationFilter());
  }
}
