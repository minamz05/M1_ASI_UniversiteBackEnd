namespace UniversiteDomain.Exceptions.UeExceptions;

public class DuplicateNumeroUeException : Exception
{
    public DuplicateNumeroUeException(string numeroUe) 
        : base($"Une UE avec le numéro '{numeroUe}' existe déjà.")
    {
    }
}