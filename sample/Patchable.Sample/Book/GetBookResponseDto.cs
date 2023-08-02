// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace Patchable.Sample.Book;

public record class GetBookResponseDto(Guid BookId, string Title, string Description, string[] Authors)
{
  public GetBookResponseDto(BookEntity bookEntity)
  : this(bookEntity.BookId, bookEntity.Title, bookEntity.Description, bookEntity.Authors)
  { }
};
