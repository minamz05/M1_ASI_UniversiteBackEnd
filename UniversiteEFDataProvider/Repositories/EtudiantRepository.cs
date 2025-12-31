using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
namespace UniversiteEFDataProvider.Repositories;

public class EtudiantRepository(UniversiteDbContext context) : Repository<Etudiant>(context), IEtudiantRepository
{
    public async Task<Etudiant> AddParcoursAsync(Etudiant etudiant, Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(parcours);
        
        var e = await Context.Etudiants.FindAsync(etudiant.Id);
        var p = await Context.Parcours.FindAsync(parcours.Id);
        
        if (e != null && p != null)
        {
            e.ParcoursSuivi = p;
            await Context.SaveChangesAsync();
        }
        
        return e!;
    }

    public async Task<Etudiant> AddParcoursAsync(long idEtudiant, long idParcours)
    {
        ArgumentNullException.ThrowIfNull(idEtudiant);
        ArgumentNullException.ThrowIfNull(idParcours);
        
        var e = await Context.Etudiants.FindAsync(idEtudiant);
        var p = await Context.Parcours.FindAsync(idParcours);
        
        if (e != null && p != null)
        {
            e.ParcoursSuivi = p;
            await Context.SaveChangesAsync();
        }
        
        return e!;
    }

    public async Task<Etudiant> AddParcoursAsync(Etudiant? etudiant, List<Parcours> parcours)
    {
        // Pour un étudiant, un seul parcours, donc on prend le premier
        if (etudiant != null && parcours.Count > 0)
        {
            return await AddParcoursAsync(etudiant, parcours[0]);
        }
        return etudiant!;
    }

    public async Task<Etudiant> AddParcoursAsync(long idEtudiant, long[] idParcours)
    {
        // Pour un étudiant, un seul parcours, donc on prend le premier
        if (idParcours.Length > 0)
        {
            return await AddParcoursAsync(idEtudiant, idParcours[0]);
        }
        return (await Context.Etudiants.FindAsync(idEtudiant))!;
    }
}