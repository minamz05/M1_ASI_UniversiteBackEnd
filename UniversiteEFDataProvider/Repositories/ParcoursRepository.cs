using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(etudiant);
        
        var p = await Context.Parcours.Include(x => x.Inscrits).FirstOrDefaultAsync(x => x.Id == parcours.Id);
        var e = await Context.Etudiants.FindAsync(etudiant.Id);
        
        if (p != null && e != null && !p.Inscrits.Contains(e))
        {
            p.Inscrits.Add(e);
            await Context.SaveChangesAsync();
        }
        
        return p!;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(idParcours);
        ArgumentNullException.ThrowIfNull(idEtudiant);
        
        var p = await Context.Parcours.Include(x => x.Inscrits).FirstOrDefaultAsync(x => x.Id == idParcours);
        var e = await Context.Etudiants.FindAsync(idEtudiant);
        
        if (p != null && e != null && !p.Inscrits.Contains(e))
        {
            p.Inscrits.Add(e);
            await Context.SaveChangesAsync();
        }
        
        return p!;
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(etudiants);
        
        var p = await Context.Parcours.Include(x => x.Inscrits).FirstOrDefaultAsync(x => x.Id == parcours.Id);
        
        if (p != null)
        {
            foreach (var etudiant in etudiants)
            {
                var e = await Context.Etudiants.FindAsync(etudiant.Id);
                if (e != null && !p.Inscrits.Contains(e))
                {
                    p.Inscrits.Add(e);
                }
            }
            await Context.SaveChangesAsync();
        }
        
        return p!;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        ArgumentNullException.ThrowIfNull(idParcours);
        ArgumentNullException.ThrowIfNull(idEtudiants);
        
        var p = await Context.Parcours.Include(x => x.Inscrits).FirstOrDefaultAsync(x => x.Id == idParcours);
        
        if (p != null)
        {
            foreach (var idEtudiant in idEtudiants)
            {
                var e = await Context.Etudiants.FindAsync(idEtudiant);
                if (e != null && !p.Inscrits.Contains(e))
                {
                    p.Inscrits.Add(e);
                }
            }
            await Context.SaveChangesAsync();
        }
        
        return p!;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        ArgumentNullException.ThrowIfNull(idParcours);
        ArgumentNullException.ThrowIfNull(idUe);
        
        var p = await Context.Parcours.Include(x => x.UesEnseignees).FirstOrDefaultAsync(x => x.Id == idParcours);
        var ue = await Context.Ues.FindAsync(idUe);
        
        if (p != null && ue != null && !p.UesEnseignees!.Contains(ue))
        {
            p.UesEnseignees!.Add(ue);
            await Context.SaveChangesAsync();
        }
        
        return p!;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
    {
        ArgumentNullException.ThrowIfNull(idParcours);
        ArgumentNullException.ThrowIfNull(idUes);
        
        var p = await Context.Parcours.Include(x => x.UesEnseignees).FirstOrDefaultAsync(x => x.Id == idParcours);
        
        if (p != null)
        {
            foreach (var idUe in idUes)
            {
                var ue = await Context.Ues.FindAsync(idUe);
                if (ue != null && !p.UesEnseignees!.Contains(ue))
                {
                    p.UesEnseignees!.Add(ue);
                }
            }
            await Context.SaveChangesAsync();
        }
        
        return p!;
    }
}