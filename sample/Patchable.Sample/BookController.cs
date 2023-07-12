// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace Patchable.Sample;

[Route("book")]
[ApiController]
public sealed class BookController : ControllerBase
{
  [HttpPatch("{bookId}", Name = nameof(BookController.PatchBook))]
  public IActionResult PatchBook(PatchBookRequestDto requestDto)
  {
    return Ok();
  }
}
