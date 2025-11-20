namespace UniversiteDomain.Exceptions.NoteExceptions;

public class NoteNotFoundException : Exception
{
    public NoteNotFoundException(string message) 
        : base(message)
    {
    }
}