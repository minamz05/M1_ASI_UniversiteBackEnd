using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface IEtudiantRepository : IRepository<Etudiant>
{
    Task AddParcoursAsync(Etudiant etudiant, Parcours parcours);
    Task AddParcoursAsync(long idEtudiant, long idParcours);
    Task AddParcoursAsync(Etudiant etudiant, Parcours[] parcours);
    Task AddParcoursAsync(long idEtudiant, long[] idParcours);
    Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant);
    Task<List<Etudiant>> FindAllEtudiantsCompletAsync();
}