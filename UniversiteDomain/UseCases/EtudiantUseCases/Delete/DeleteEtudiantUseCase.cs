using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<int> ExecuteAsync(long idEtudiant)
    {
        // Vérification des paramètres
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idEtudiant);

        // Vérification que l'étudiant existe
        Etudiant? etudiant = await repositoryFactory.EtudiantRepository().FindAsync(idEtudiant);
        if (etudiant == null)
            throw new ArgumentException($"L'étudiant avec l'ID {idEtudiant} n'existe pas");

        // Suppression de l'étudiant
        await repositoryFactory.EtudiantRepository().DeleteAsync(idEtudiant);
        
        // Sauvegarde des changements
        await repositoryFactory.SaveChangesAsync();
        
        return 1;
    }
}