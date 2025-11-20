namespace UniversiteDomain.Exceptions.NoteExceptions;

public class DuplicateNoteException : Exception
{
    public DuplicateNoteException(string message) 
        : base(message)
    {
    }
}