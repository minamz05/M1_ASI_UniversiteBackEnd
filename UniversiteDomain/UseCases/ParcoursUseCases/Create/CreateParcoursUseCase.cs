using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours(nomParcours, anneeFormation);
        return await ExecuteAsync(parcours);
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours p = await repositoryFactory.ParcoursRepository().CreateAsync(parcours);
        await repositoryFactory.SaveChangesAsync();
        return p;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        // Vérification des paramètres
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        
        // Vérifions que nous sommes bien connectés aux datasources
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());

        // On recherche un parcours avec le même nom
        List<Parcours> existe = await repositoryFactory.ParcoursRepository()
            .FindByConditionAsync(p => p.NomParcours == parcours.NomParcours);

        // Si un parcours avec le même nom existe déjà, on lève une exception personnalisée
        if (existe.Count > 0) 
            throw new DuplicateNomParcoursException(parcours.NomParcours + " - ce nom de parcours est déjà utilisé");

        // Le métier définit que l'année de formation doit être comprise entre 1900 et 2100
        if (parcours.AnneeFormation < 1900 || parcours.AnneeFormation > 2100) 
            throw new InvalidAnneeFormationException(parcours.AnneeFormation + " - L'année de formation doit être comprise entre 1900 et 2100");

        // Le métier définit que le nom du parcours doit contenir au moins 3 caractères
        if (parcours.NomParcours.Length < 3) 
            throw new InvalidNomParcoursException(parcours.NomParcours + " - Le nom du parcours doit contenir au moins 3 caractères");
    }
}