namespace UniversiteDomain.Exceptions.EtudiantExceptions;

public class EtudiantNotFoundException : Exception
{
    public EtudiantNotFoundException(string message) 
        : base($"Étudiant non trouvé : {message}")
    {
    }
}