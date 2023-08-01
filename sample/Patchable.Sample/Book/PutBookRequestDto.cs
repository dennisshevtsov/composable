namespace Patchable.Sample.Book;

public record PutBookRequestDto(Guid BookId, string Title, string Description, string[] Authors);
