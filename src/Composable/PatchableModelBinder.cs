// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Composable;

/// <summary>
/// The <see cref="PatchableModelBinder"/> provides a mechanism to create an instance
/// of a model that implements the <see cref="IPatchable"/> from HTTP request. The binder
/// allows to bind parameters from the body, the route and the query string of the HTTP.
/// The binder also sets a list of properties that have been populated from the HTTP request.
/// </summary>
public sealed class PatchableModelBinder : ComposableModelBinder
{
  /// <summary>
  /// Populates the model.
  /// </summary>
  /// <param name="model">The instance of the model.</param>
  /// <param name="metadata">The dictionary of a property name and property metadata.</param>
  /// <param name="values">The dictionary of a property name and a property value.</param>
  protected override void PopulateModel(object model, Dictionary<string, ModelMetadata> metadata, Dictionary<string, object?> values)
  {
    base.PopulateModel(model, metadata, values);
    ((IPatchable)model).Properties = values.Select(value => value.Key).ToArray();
  }
}
