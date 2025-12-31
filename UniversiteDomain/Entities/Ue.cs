namespace UniversiteDomain.Entities;

public class Ue
{
    public long Id { get; set; }
    public string NumeroUe { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    
    // ManyToMany : une Ue est enseignée dans plusieurs parcours
    public List<Parcours>? EnseigneeDans { get; set; } = new();
    
    // OneToMany : une UE a plusieurs notes
    public List<Note> Notes { get; set; } = new();

    // Constructeur par défaut
    public Ue()
    {
    }

    // Constructeur avec paramètres
    public Ue(string numeroUe, string intitule)
    {
        NumeroUe = numeroUe;
        Intitule = intitule;
        EnseigneeDans = new();
        Notes = new();
    }

    public override string ToString()
    {
        return $"ID {Id} : {NumeroUe} - {Intitule}";
    }
}