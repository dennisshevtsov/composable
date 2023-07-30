// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace Patchable.Test;

public sealed class TestPatchableModel : IPatchable
{
  public Guid Id { get; set; }

  public string? Name { get; set; }

  public string[] Properties { get; set; } = Array.Empty<string>();
}
