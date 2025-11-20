namespace UniversiteDomain.Exceptions.UeExceptions;

public class InvalidIntituleUeException : Exception
{
    public InvalidIntituleUeException(string intitule) 
        : base($"L'intitulé '{intitule}' est invalide. Il doit contenir au moins 3 caractères.")
    {
    }
}