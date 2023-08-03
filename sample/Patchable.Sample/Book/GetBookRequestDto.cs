// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace Patchable.Sample.Book;

public record class GetBookRequestDto(Guid BookId)
{
  public GetBookRequestDto(BookEntity bookEntity) : this (bookEntity.BookId) { }
}
