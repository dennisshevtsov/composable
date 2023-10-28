// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Composable;

/// <summary>
/// The <see cref="PatchableModelBinderProvider"/> allows to create an instance of the
/// <see cref="PatchableModelBinder"/> for a model that inherits the <see cref="IPatchable"/>.
/// Register the <see cref="PatchableModelBinderProvider"/> in the <see cref="Microsoft.AspNetCore.Mvc.MvcOptions"/>.
/// </summary>
public sealed class PatchableModelBinderProvider : IModelBinderProvider
{
  /// <summary>
  /// The <see cref="GetBinder"/> creates an instance of the <see cref="PatchableModelBinder"/>
  /// for a model that inherits the <see cref="IPatchable"/>. Otherwice <see cref="GetBinder"/> returns null.
  /// </summary>
  /// <param name="context">A context object for <see cref="IModelBinderProvider.GetBinder"/>.</param>
  /// <returns>An instance of the <see cref="PatchableModelBinder"/> or null.</returns>
  public IModelBinder? GetBinder(ModelBinderProviderContext context)
  {
    if (context.Metadata.ModelType.IsAssignableTo(typeof(IPatchable)))
    {
      return new PatchableModelBinder();
    }

    return null;
  }
}
