namespace UniversiteDomain.Exceptions.UeExceptions;

public class InvalidNumeroUeException : Exception
{
    public InvalidNumeroUeException(string numeroUe) 
        : base($"Le numéro d'UE '{numeroUe}' est invalide. Il ne peut pas être vide ou null.")
    {
    }
}