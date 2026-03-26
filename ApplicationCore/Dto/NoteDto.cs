namespace ApplicationCore.Dto;

public record NoteDto(Guid Id, string Content, DateTime CreatedAt)
{
    public static NoteDto FromEntity(Models.ContactAggregate.Note note) =>
        new(note.Id, note.Content, note.CreatedAt);
}

public record CreateNoteDto(string Content);
