using System.Linq.Expressions;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

public class AddUeDansParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    // Rajout d'une Ue dans un parcours
    public async Task<Parcours> ExecuteAsync(Parcours parcours, Ue ue)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(ue);
        return await ExecuteAsync(parcours.Id, ue.Id); 
    }  
    
    public async Task<Parcours> ExecuteAsync(long idParcours, long idUe)
    {
        await CheckBusinessRules(idParcours, idUe); 
        return await repositoryFactory.ParcoursRepository().AddUeAsync(idParcours, idUe);
    }

    // Rajout de plusieurs étudiants dans un parcours
    public async Task<Parcours> ExecuteAsync(Parcours parcours, List<Ue> ues)
    {
        ArgumentNullException.ThrowIfNull(ues);
        ArgumentNullException.ThrowIfNull(parcours);
        long[] idUes = ues.Select(x => x.Id).ToArray();
        return await ExecuteAsync(parcours.Id, idUes); 
    }  
    
    public async Task<Parcours> ExecuteAsync(long idParcours, long [] idUes)
    { 
        // Comme demandé par le client, on teste tous les règles avant de modifier les données
        foreach(var id in idUes) 
            await CheckBusinessRules(idParcours, id);
        return await repositoryFactory.ParcoursRepository().AddUeAsync(idParcours, idUes);
    }   

    private async Task CheckBusinessRules(long idParcours, long idUe)
    {
        // Vérification des paramètres
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idParcours);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUe);
        
        // Vérifions tout d'abord que nous sommes bien connectés aux datasources
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        
        // On recherche l'ue
        List<Ue> ues = await repositoryFactory.UeRepository().FindByConditionAsync(e => e.Id.Equals(idUe));
        if (ues == null || ues.Count == 0) 
            throw new UeNotFoundException(idUe.ToString());
        
        // On recherche le parcours
        List<Parcours> parcoursList = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p => p.Id.Equals(idParcours));
        if (parcoursList == null || parcoursList.Count == 0) 
            throw new ParcoursNotFoundException(idParcours.ToString());
        
        Parcours parcours = parcoursList[0];
        
        // On vérifie que l'Ue n'est pas déjà dans le parcours
        if (parcours.UesEnseignees != null && parcours.UesEnseignees.Count > 0)
        {
            // Des ues sont déjà enregistrées dans le parcours
            // On recherche si l'ue qu'on veut ajouter n'existe pas déjà
            bool existe = parcours.UesEnseignees.Any(e => e.Id.Equals(idUe));
            if (existe) 
                throw new DuplicateUeDansParcoursException(idUe + " est déjà présente dans le parcours : " + idParcours);   
        }
    }
}