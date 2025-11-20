namespace UniversiteDomain.Exceptions.NoteExceptions;

public class InvalidNoteValueException : Exception
{
    public InvalidNoteValueException(float valeur) 
        : base($"La note '{valeur}' est invalide. Elle doit être comprise entre 0 et 20.")
    {
    }
}