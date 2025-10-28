using System.Linq.Expressions;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface IParcoursRepository
{
    Task<Parcours> CreateAsync(Parcours entity);
    Task UpdateAsync(Parcours entity);
    Task DeleteAsync(long id);
    Task DeleteAsync(Parcours entity);
    Task<Parcours?> FindAsync(long id);
    Task<Parcours?> FindAsync(params object[] keyValues);
    Task<List<Parcours>> FindByConditionAsync(Expression<Func<Parcours, bool>> condition);
    Task<List<Parcours>> FindAllAsync();
    Task SaveChangesAsync();
    Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant);
    Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant);
    Task<Parcours> AddEtudiantAsync(Parcours ? parcours, List<Etudiant> etudiants);
    Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants);
}