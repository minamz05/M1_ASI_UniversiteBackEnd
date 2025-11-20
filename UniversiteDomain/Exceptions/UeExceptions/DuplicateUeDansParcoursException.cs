namespace UniversiteDomain.Exceptions.UeExceptions;

public class DuplicateUeDansParcoursException : Exception
{
    public DuplicateUeDansParcoursException(string message) 
        : base(message)
    {
    }
}