﻿// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using System;
using System.ComponentModel;
using System.Reflection;
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
  public async Task BindModelAsync(ModelBindingContext bindingContext)
  {
    object model = Activator.CreateInstance(bindingContext.ModelType)!;

    await FillOutFromBodyAsync(model, bindingContext);
    FillOutFromRoute(model, bindingContext);
    FillOutFromQueryString(model, bindingContext);

    bindingContext.Result = ModelBindingResult.Success(model);
  }

  private async Task FillOutFromBodyAsync(object model, ModelBindingContext bindingContext)
  {
    if (bindingContext.HttpContext.Request.ContentLength == null ||
        bindingContext.HttpContext.Request.ContentLength == 0)
    {
      return;
    }

    JsonDocument? document = await JsonSerializer.DeserializeAsync<JsonDocument>(
        bindingContext.HttpContext.Request.Body);

    if (document == null)
    {
      return;
    }

    foreach (var documentProperty in document.RootElement.EnumerateObject())
    {
      ModelMetadata? modelProperty = bindingContext.ModelMetadata.Properties.FirstOrDefault(
        property => string.Equals(property.Name, documentProperty.Name, StringComparison.OrdinalIgnoreCase));

      if (modelProperty != null && modelProperty.PropertySetter != null)
      {
        modelProperty.PropertySetter.Invoke(
          model, documentProperty.Value.Deserialize(modelProperty.ModelType));
      }
    }
  }

  private void FillOutFromRoute(object model, ModelBindingContext bindingContext)
  {
    foreach (ModelMetadata propertyMetadata in bindingContext.ModelMetadata.Properties)
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
  }

  private void FillOutFromQueryString(object model, ModelBindingContext bindingContext)
  {
    foreach (KeyValuePair<string, StringValues> queryParam in bindingContext.HttpContext.Request.Query)
    {
      ModelMetadata? propertyMetadata;
      TypeConverter? converter;

      if ((propertyMetadata = bindingContext.ModelMetadata.Properties[queryParam.Key]) != null &&
          propertyMetadata.PropertySetter != null &&
          (converter = TypeDescriptor.GetConverter(propertyMetadata.ModelType)) != null)
      {
        propertyMetadata.PropertySetter(model!, converter.ConvertFrom(queryParam.Value.ToString()));
      }
    }
  }

  private Dictionary<string, ModelMetadata> GetPropertyMetadata(ModelBindingContext bindingContext)
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

  private async Task<Dictionary<string, object?>> GetPropertyValues(
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
      ModelMetadata? modelProperty = metadata[documentProperty.Name];

      if (modelProperty != null && modelProperty.PropertySetter != null)
      {
        values[documentProperty.Name] =
          documentProperty.Value.Deserialize(modelProperty.ModelType);
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
          (propertyMetadata = metadata[routeParam.Key]) != null &&
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

      if ((propertyMetadata = metadata[param.Key]) != null &&
          propertyMetadata.PropertySetter != null &&
          (converter = TypeDescriptor.GetConverter(propertyMetadata.ModelType)) != null)
      {
        values[param.Key] = converter.ConvertFrom(param.Value.ToString());
      }
    }
  }
}
