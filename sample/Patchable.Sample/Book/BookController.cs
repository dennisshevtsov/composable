// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace Patchable.Sample.Book;

[Route("book")]
[ApiController]
public sealed class BookController : ControllerBase
{
  [HttpGet("{bookId}", Name = nameof(BookController.GetBook))]
  public IActionResult GetBook(GetBookRequestDto requestDto)
  {
    return Ok();
  }

  [HttpPost(Name = nameof(BookController.PostBook))]
  public IActionResult PostBook(PostBookRequestDto requestDto)
  {
    return Ok();
  }

  [HttpPut("{bookId}", Name = nameof(PutBook))]
  public IActionResult PutBook(PutBookRequestDto requestDto)
  {
    return Ok();
  }

  [HttpPatch("{bookId}", Name = nameof(PatchBook))]
  public IActionResult PatchBook(PatchBookRequestDto requestDto)
  {
    return Ok();
  }

  [HttpDelete("{bookId}", Name = nameof(DeleteBook))]
  public IActionResult DeleteBook(DeleteBookRequestDto requestDto)
  {
    return Ok();
  }
}
