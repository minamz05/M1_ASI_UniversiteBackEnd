namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }
    public float Valeur { get; set; }
    
    // Clés étrangères pour la classe d'association
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
    
    // Propriétés de navigation (pour EF Core)
    public Etudiant? Etudiant { get; set; }
    public Ue? Ue { get; set; }

    public Note()
    {
    }

    public Note(float valeur, long etudiantId, long ueId)
    {
        Valeur = valeur;
        EtudiantId = etudiantId;
        UeId = ueId;
    }

    public override string ToString()
    {
        return $"Note {Id} : Étudiant {EtudiantId} - UE {UeId} - Valeur {Valeur}/20";
    }
}