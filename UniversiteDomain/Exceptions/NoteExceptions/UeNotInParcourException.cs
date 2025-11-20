namespace UniversiteDomain.Exceptions.NoteExceptions;

public class UeNotInParcoursException : Exception
{
    public UeNotInParcoursException(string message) 
        : base(message)
    {
    }
}