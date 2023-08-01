// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace Patchable.Sample.Book;

public sealed class GetBookResponseDto
{
  public GetBookResponseDto(
    Guid bookId,
    string title,
    string description,
    string[] authors)
  {
    BookId = bookId;
    Title  = title;
    Description = description;
    Authors = authors;
  }

  public Guid BookId { get; }

  public string Title { get; }

  public string Description { get; }

  public string[] Authors { get; }
}
