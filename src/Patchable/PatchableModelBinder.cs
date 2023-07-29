// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Patchable;

/// <summary>
/// The <see cref="PatchableModelBinder"/> provides a mechanism to create an instance
/// of a model that implements the <see cref="IPatchable"/> from HTTP request. The binder
/// allows to bind parameters from the body, the route and the query string of the HTTP.
/// The binder also sets a list of properties that have been populated from the HTTP request.
/// </summary>
public sealed class PatchableModelBinder : ComposableModelBinder
{
  public async override Task BindModelAsync(ModelBindingContext bindingContext)
  {
    object model = Activator.CreateInstance(bindingContext.ModelType)!;

    Dictionary<string, ModelMetadata> metadata = GetPropertyMetadata(bindingContext);
    Dictionary<string, object?> values = await GetPropertyValuesAsync(metadata, bindingContext);

    foreach (var value in values)
    {
      metadata[value.Key].PropertySetter!.Invoke(model, value.Value);
    }

    bindingContext.Result = ModelBindingResult.Success(model);
  }
}
