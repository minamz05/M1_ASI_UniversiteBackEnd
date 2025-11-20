using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue> ExecuteAsync(string numeroUe, string intitule)
    {
        var ue = new Ue(numeroUe, intitule);
        return await ExecuteAsync(ue);
    }

    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        Ue createdUe = await repositoryFactory.UeRepository().CreateAsync(ue);
        await repositoryFactory.SaveChangesAsync();
        return createdUe;
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        // Vérifier que le numéro d'UE n'est pas vide ou null
        if (string.IsNullOrEmpty(ue.NumeroUe))
            throw new InvalidNumeroUeException(ue.NumeroUe ?? "");

        // Vérifier que l'intitulé n'est pas vide ou null
        if (string.IsNullOrEmpty(ue.Intitule))
            throw new InvalidIntituleUeException(ue.Intitule ?? "");

        // Vérifier que l'intitulé a au moins 3 caractères
        if (ue.Intitule.Length < 3)
            throw new InvalidIntituleUeException(ue.Intitule);

        // Vérifier que deux UEs n'ont pas le même numéro
        List<Ue> uesExistantes = await repositoryFactory.UeRepository().FindByConditionAsync(u => u.NumeroUe.Equals(ue.NumeroUe));
        if (uesExistantes is { Count: > 0 })
            throw new DuplicateNumeroUeException(ue.NumeroUe);
    }
}