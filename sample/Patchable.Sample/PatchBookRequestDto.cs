// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace Patchable.Sample;

public sealed class PatchBookRequestDto : IPatchable
{
  public PatchBookRequestDto()
  {
    Title       = string.Empty;
    Description = string.Empty;
    Pages       = 0;
    Properties  = Array.Empty<string>();
  }

  public string Title { get; set; }

  public string Description { get; set; }

  public int Pages { get; set; }

  public string[] Properties { get; set; }
}
