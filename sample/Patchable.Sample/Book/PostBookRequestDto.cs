// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace Patchable.Sample.Book;

public record class PostBookRequestDto(string Title, string Description, string[] Authors)
{
  internal BookEntity ToBookEntity() => new(Guid.NewGuid(), Title, Description, Authors);
}
