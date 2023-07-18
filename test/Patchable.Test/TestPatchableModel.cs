// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace Patchable.Test;

public sealed class TestPatchableModel : IPatchable
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public string[] Properties { get; set; } = Array.Empty<string>();

  public override bool Equals(object? obj)
  {
    if (obj == null)
    {
      return false;
    }

    if (obj == this)
    {
      return true;
    }

    if (obj is TestPatchableModel model)
    {
      return Id == model.Id && Name == model.Name;
    }

    return false;
  }

  public override int GetHashCode() => HashCode.Combine(Id.GetHashCode(), Name.GetHashCode());
}
