namespace ApplicationCore.Exceptions;

public class ContactNotFoundException : Exception
{
    public ContactNotFoundException(string message) : base(message)
    {
    }
}

public class NoteNotFoundException : Exception
{
    public NoteNotFoundException(string message) : base(message)
    {
    }
}
