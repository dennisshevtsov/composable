// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace Patchable;

/// <summary>
/// Inherit a model from the <see cref="IPatchable"/> if you need a model that binds to
/// parameters from the body, the route and the query string of the HTTP request
/// in one class and/or a list of properties that have been populated from the HTTP request.
/// </summary>
public interface IPatchable
{
  /// <summary>
  /// A list of properties that have been populated from the HTTP request.
  /// </summary>
  public string[] Properties { get; set; }
}
