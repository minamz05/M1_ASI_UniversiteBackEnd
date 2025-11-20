namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class DuplicateInscriptionException : Exception
{
    public DuplicateInscriptionException(string message) 
        : base(message)
    {
    }
}