namespace UniversiteDomain.Entities;

public class Parcours
{
    public long Id { get; set; }
    public string NomParcours { get; set; } = string.Empty;
    public int AnneeFormation { get; set; }
    
    // Collection d'étudiants (pour la relation OneToMany)
    public List<Etudiant> Etudiants { get; set; } = new List<Etudiant>();

    // ⬇️ AJOUT : Constructeur par défaut (nécessaire pour EF Core)
    public Parcours()
    {
    }

    // ⬇️ AJOUT : Constructeur avec paramètres
    public Parcours(string nomParcours, int anneeFormation)
    {
        NomParcours = nomParcours;
        AnneeFormation = anneeFormation;
        Etudiants = new List<Etudiant>();
    }

    public override string ToString()
    {
        return $"ID {Id} : {NomParcours} - Année {AnneeFormation} ({Etudiants.Count} étudiants inscrits)";
    }
}