// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Patchable.Sample.Book;

public record class PatchBookRequestDto : IPatchable
{
  public Guid BookId { get; set; }

  [MinLength(1), MaxLength(255)]
  public string Title { get; set; } = string.Empty;

  [MaxLength(255)]
  public string Description { get; set; } = string.Empty;

  [Required, MinLength(1)]
  public string[] Authors { get; set; } = Array.Empty<string>();

  [JsonIgnore]
  public string[] Properties { get; set; } = Array.Empty<string>();

  internal BookEntity Patch(BookEntity bookEntity) => new(
    bookEntity.BookId,
    Properties.Contains(nameof(Title), StringComparer.OrdinalIgnoreCase) ? Title : bookEntity.Title,
    Properties.Contains(nameof(Description), StringComparer.OrdinalIgnoreCase) ? Description : bookEntity.Description,
    Properties.Contains(nameof(Authors), StringComparer.OrdinalIgnoreCase) ? Authors : bookEntity.Authors);
}
