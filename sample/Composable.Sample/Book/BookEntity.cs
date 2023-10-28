// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace Composable.Sample.Book
{
  public record class BookEntity(Guid BookId, string Title, string Description, string[] Authors);
}
