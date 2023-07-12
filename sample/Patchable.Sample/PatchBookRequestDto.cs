// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

namespace Patchable.Sample;

public sealed class PatchBookRequestDto : IPatchable
{
  public PatchBookRequestDto()
  {
    Title       = string.Empty;
    Description = null;
    Pages       = 0;
    Properties  = Array.Empty<string>();
  }

  [FromRoute]
  [Required]
  public Guid BoodId { get; set; }

  [FromBody]
  [Required]
  public string Title { get; set; }

  [FromBody]
  public string? Description { get; set; }

  [FromBody]
  public int Pages { get; set; }

  [JsonIgnore]
  public string[] Properties { get; set; }
}
