using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using Microsoft.EntityFrameworkCore;

namespace UniversiteEFDataProvider.Repositories;

public class EtudiantRepository(UniversiteDbContext context) : Repository<Etudiant>(context), IEtudiantRepository
{
    public async Task AddParcoursAsync(Etudiant etudiant, Parcours parcours)
    {
        await AddParcoursAsync(etudiant.Id, parcours.Id);
    }

    public async Task AddParcoursAsync(long idEtudiant, long idParcours)
    {
        Etudiant? etudiant = await FindAsync(idEtudiant);
        Parcours? parcours = await context.Parcours!.FindAsync(idParcours);
        
        if (etudiant != null && parcours != null)
        {
            etudiant.ParcoursSuivi = parcours;
            await context.SaveChangesAsync();
        }
    }

    public async Task AddParcoursAsync(Etudiant etudiant, Parcours[] parcours)
    {
        foreach (Parcours p in parcours)
        {
            await AddParcoursAsync(etudiant, p);
        }
    }

    public async Task AddParcoursAsync(long idEtudiant, long[] idParcours)
    {
        foreach (long id in idParcours)
        {
            await AddParcoursAsync(idEtudiant, id);
        }
    }
    
    public async Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        return await Context.Etudiants
            .Include(e => e.ParcoursSuivi)
            .Include(e => e.NotesObtenues)
            .ThenInclude(n => n.Ue)
            .FirstOrDefaultAsync(e => e.Id == idEtudiant);
    }

    // ✅ RENOMMÉE POUR LA COHÉRENCE
    public async Task<List<Etudiant>> FindAllEtudiantsCompletAsync()
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        return await Context.Etudiants
            .Include(e => e.ParcoursSuivi)
            .Include(e => e.NotesObtenues)
            .ThenInclude(n => n.Ue)
            .ToListAsync();
    }
}