// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace Patchable.Sample.Book;

public record class PatchBookRequestDto(
  string Title, string Description, string[] Authors) : IPatchable
{
  [JsonIgnore]
  public string[] Properties { get; set; } = Array.Empty<string>();
}
