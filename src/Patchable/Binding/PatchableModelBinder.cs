// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using System.ComponentModel;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Patchable.Binding;

/// <summary>
/// The <see cref="PatchableModelBinder"/> provides a mechanism to create an instance
/// of a model that implements the <see cref="IPatchable"/> from HTTP request. The binder
/// allows to bind parameters from the body, the route and the query string of the HTTP.
/// The binder also sets a list of properties that have been populated from the HTTP request.
/// </summary>
public sealed class PatchableModelBinder : IModelBinder
{
  public async Task BindModelAsync(ModelBindingContext bindingContext)
  {
    object? model;

    if (bindingContext.HttpContext.Request.ContentLength != null &&
        bindingContext.HttpContext.Request.ContentLength != 0)
    {
      model = await PatchableModelBinder.GetModelValue(bindingContext);
    }
    else
    {
      model = Activator.CreateInstance(bindingContext.ModelType);
    }

    foreach (var propertyMetadata in bindingContext.ModelMetadata.Properties)
    {
      object? routeValue;
      TypeConverter? converter;

      if (propertyMetadata != null &&
          propertyMetadata.PropertySetter != null &&
          propertyMetadata.PropertyName != null &&
          (routeValue = bindingContext.ActionContext.RouteData.Values[propertyMetadata.PropertyName]) != null &&
          (converter = TypeDescriptor.GetConverter(propertyMetadata.ModelType)) != null)
      {
        propertyMetadata.PropertySetter(model!, converter.ConvertFrom(routeValue));
      }
    }

    bindingContext.Result = ModelBindingResult.Success(model);
  }

  private static async Task<object> GetModelValue(ModelBindingContext bindingContext)
  {
    JsonDocument? document = await JsonSerializer.DeserializeAsync<JsonDocument>(
        bindingContext.HttpContext.Request.Body);

    object model = document!.Deserialize(
      bindingContext.ModelType,
      new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      })!;

    if (model is IPatchable patchable)
    {
      ISet<string> properties =
        document!.RootElement.EnumerateObject()!
                             .Select(property => property.Name)
                             .ToHashSet(StringComparer.OrdinalIgnoreCase);

      patchable.Properties =
        bindingContext.ModelMetadata.Properties.Select(property => property.Name!)
                                               .Where(property => properties.Contains(property))
                                               .ToArray();
    }

    return model;
  }
}
