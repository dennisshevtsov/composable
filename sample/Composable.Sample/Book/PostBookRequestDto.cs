// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Composable.Sample.Book;

public record class PostBookRequestDto : IComposable
{
  [Required, MinLength(1), MaxLength(255)]
  public string Title { get; set; } = string.Empty;

  [MaxLength(255)]
  public string Description { get; set;} = string.Empty;

  [Required, MinLength(1)]
  public string[] Authors { get; set; } = Array.Empty<string>();

  internal BookEntity ToBookEntity() => new(Guid.NewGuid(), Title, Description, Authors);
}
