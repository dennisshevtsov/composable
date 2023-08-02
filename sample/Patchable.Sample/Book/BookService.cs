// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace Patchable.Sample.Book;

public sealed class BookService
{
  private readonly Dictionary<Guid, BookEntity> _books = new();

  public BookEntity? GetBook(Guid bookId) =>
    _books.TryGetValue(bookId, out BookEntity? book) ? book : null;

  public void SaveBook(BookEntity bookEntity) => _books[bookEntity.BookId] = bookEntity;
}
