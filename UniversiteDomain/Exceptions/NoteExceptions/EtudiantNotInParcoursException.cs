namespace UniversiteDomain.Exceptions.NoteExceptions;

public class EtudiantNotInParcoursException : Exception
{
    public EtudiantNotInParcoursException(string message) 
        : base(message)
    {
    }
}