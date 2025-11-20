namespace UniversiteDomain.Entities;

public class Ue
{
    public long Id { get; set; }
    public string NumeroUe { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    
    // ManyToMany : une Ue est enseignée dans plusieurs parcours
    public List<Parcours>? EnseigneeDans { get; set; } = new();

    // Constructeur par défaut (nécessaire pour EF Core)
    public Ue()
    {
    }

    // Constructeur avec paramètres
    public Ue(string numeroUe, string intitule)
    {
        NumeroUe = numeroUe;
        Intitule = intitule;
        EnseigneeDans = new();
    }

    public override string ToString()
    {
        return $"ID {Id} : {NumeroUe} - {Intitule}";
    }
}