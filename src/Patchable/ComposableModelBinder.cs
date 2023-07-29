// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using System.ComponentModel;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace Patchable;

/// <summary>
/// The <see cref="ComposableModelBinder"/> provides a mechanism to create an instance
/// of a model that implements the <see cref="IComposable"/> from HTTP request. The binder
/// allows to bind parameters from the body, the route and the query string of the HTTP.
/// </summary>
public class ComposableModelBinder : IModelBinder
{
  /// <summary>
  /// Attempts to bind a model.
  /// </summary>
  /// <param name="bindingContext">A context that contains operating information for model binding and validation.</param>
  /// <returns>An instance of the <see cref="Task"/> that represents an asynchronous operation.</returns>
  public virtual async Task BindModelAsync(ModelBindingContext bindingContext)
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

  protected Dictionary<string, ModelMetadata> GetPropertyMetadata(ModelBindingContext bindingContext)
  {
    Dictionary<string, ModelMetadata> properties = new(StringComparer.OrdinalIgnoreCase);

    for (int i = 0; i < bindingContext.ModelMetadata.Properties.Count; i++)
    {
      ModelMetadata property = bindingContext.ModelMetadata.Properties[i];

      if (property.PropertySetter != null && property.PropertyName != null)
      {
        properties.Add(property.PropertyName, property);
      }
    }

    return properties;
  }

  protected async Task<Dictionary<string, object?>> GetPropertyValuesAsync(
    Dictionary<string, ModelMetadata> metadata,
    ModelBindingContext bindingContext)
  {
    Dictionary<string, object?> values = new(StringComparer.OrdinalIgnoreCase);

    await AddPropertyValuesFromBodyAsync(values, metadata, bindingContext.HttpContext.Request);
    AddPropertyValuesFromRoute(values, metadata, bindingContext.ActionContext.RouteData.Values);
    AddPropertyValuesFromQuery(values, metadata, bindingContext.HttpContext.Request.Query);

    return values;
  }

  private async Task AddPropertyValuesFromBodyAsync(
    Dictionary<string, object?> values,
    Dictionary<string, ModelMetadata> metadata,
    HttpRequest request)
  {
    if (request.ContentLength == null || request.ContentLength == 0)
    {
      return;
    }

    JsonDocument? document = await JsonSerializer.DeserializeAsync<JsonDocument>(
        request.Body);

    if (document == null)
    {
      return;
    }

    foreach (JsonProperty documentProperty in document.RootElement.EnumerateObject())
    {
      ModelMetadata? propertyMetadata;

      if (metadata.TryGetValue(documentProperty.Name, out propertyMetadata))
      {
        values[documentProperty.Name] =
          documentProperty.Value.Deserialize(propertyMetadata.ModelType);
      }
    }
  }

  private void AddPropertyValuesFromRoute(
    Dictionary<string, object?> values,
    Dictionary<string, ModelMetadata> metadata,
    RouteValueDictionary routeValues)
  {
    foreach (KeyValuePair<string, object?> routeParam in routeValues)
    {
      ModelMetadata? propertyMetadata;
      TypeConverter? converter;

      if (routeParam.Value != null &&
          metadata.TryGetValue(routeParam.Key, out propertyMetadata) &&
          propertyMetadata.PropertySetter != null &&
          (converter = TypeDescriptor.GetConverter(propertyMetadata.ModelType)) != null)
      {
        values[routeParam.Key] = converter.ConvertFrom(routeParam.Value.ToString()!);
      }
    }
  }

  private void AddPropertyValuesFromQuery(
    Dictionary<string, object?> values,
    Dictionary<string, ModelMetadata> metadata,
    IQueryCollection querystring)
  {
    foreach (KeyValuePair<string, StringValues> param in querystring)
    {
      ModelMetadata? propertyMetadata;
      TypeConverter? converter;

      if (metadata.TryGetValue(param.Key, out propertyMetadata) &&
          propertyMetadata.PropertySetter != null &&
          (converter = TypeDescriptor.GetConverter(propertyMetadata.ModelType)) != null)
      {
        values[param.Key] = converter.ConvertFrom(param.Value.ToString());
      }
    }
  }
}
