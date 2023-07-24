// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Patchable;

/// <summary>
/// The <see cref="ComposableModelBinderProvider"/> allows to create an instance of the
/// <see cref="ComposableModelBinder"/> for a model that inherits the <see cref="IComposable"/>.
/// Register the <see cref="ComposableModelBinderProvider"/> in the <see cref="Microsoft.AspNetCore.Mvc.MvcOptions"/>.
/// </summary>
public sealed class ComposableModelBinderProvider : IModelBinderProvider
{
  /// <summary>
  /// The <see cref="GetBinder"/> creates an instance of the <see cref="ComposableModelBinderProvider"/>
  /// for a model that inherits the <see cref="IComposable"/>. Otherwice <see cref="GetBinder"/> returns null.
  /// </summary>
  /// <param name="context">A context object for <see cref="IModelBinderProvider.GetBinder"/>.</param>
  /// <returns>An instance of the <see cref="ComposableModelBinder"/> or null.</returns>
  public IModelBinder? GetBinder(ModelBinderProviderContext context)
  {
    if (context.Metadata.ModelType.IsAssignableTo(typeof(IComposable)))
    {
      return new ComposableModelBinder();
    }

    return null;
  }
}
