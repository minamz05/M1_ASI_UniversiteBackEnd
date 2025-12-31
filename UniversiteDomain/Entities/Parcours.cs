namespace UniversiteDomain.Entities;

public class Parcours
{
    public long Id { get; set; }
    public string NomParcours { get; set; } = string.Empty;
    public int AnneeFormation { get; set; }
    
    // OneToMany : un parcours contient plusieurs étudiants
    public List<Etudiant> Inscrits { get; set; } = new List<Etudiant>();
    
    // ManyToMany : un parcours contient plusieurs UEs
    public List<Ue>? UesEnseignees { get; set; } = new();

    // Constructeur par défaut
    public Parcours()
    {
    }

    // Constructeur avec paramètres
    public Parcours(string nomParcours, int anneeFormation)
    {
        NomParcours = nomParcours;
        AnneeFormation = anneeFormation;
        Inscrits = new List<Etudiant>();
        UesEnseignees = new();
    }

    public override string ToString()
    {
        return $"ID {Id} : {NomParcours} - Année {AnneeFormation} ({Inscrits.Count} étudiants inscrits)";
    }
}