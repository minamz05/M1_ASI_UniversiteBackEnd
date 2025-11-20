namespace UniversiteDomain.Exceptions.UeExceptions;

public class UeNotFoundException : Exception
{
    public UeNotFoundException(long id) 
        : base($"L'UE avec l'ID '{id}' n'a pas été trouvée.")
    {
    }

    public UeNotFoundException(string numeroUe) 
        : base($"L'UE avec le numéro '{numeroUe}' n'a pas été trouvée.")
    {
    }
}