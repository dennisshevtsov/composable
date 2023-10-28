// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace Composable.Sample.Book;

public record DeleteBookRequestDto(Guid BookId) : IComposable
{
  public DeleteBookRequestDto() : this(Guid.Empty) { }
}
