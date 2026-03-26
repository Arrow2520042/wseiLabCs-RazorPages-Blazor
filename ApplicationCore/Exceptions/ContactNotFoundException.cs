namespace ApplicationCore.Exceptions;

public class ContactNotFoundException : KeyNotFoundException
{
    public ContactNotFoundException(string message) : base(message)
    {
    }
}

public class NoteNotFoundException : KeyNotFoundException
{
    public NoteNotFoundException(string message) : base(message)
    {
    }
}
