namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class ParcoursNotFoundException : Exception
{
    public ParcoursNotFoundException(string message) 
        : base($"Parcours non trouvé : {message}")
    {
    }
}