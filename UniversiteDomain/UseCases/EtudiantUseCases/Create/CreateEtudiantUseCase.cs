using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Util;

public class CreateEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Etudiant> ExecuteAsync(string numEtud, string nom, string prenom, string email)
    {
        var etudiant = new Etudiant
        {
            NumEtud = numEtud, 
            Nom = nom, 
            Prenom = prenom, 
            Email = email
        };
        return await ExecuteAsync(etudiant);
    }
    
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);
        Etudiant et = await repositoryFactory.EtudiantRepository().CreateAsync(etudiant);
        await repositoryFactory.SaveChangesAsync();
        return et;
    }
    
    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        // Vérification des paramètres
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.NumEtud);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        
        // Vérifions que nous sommes bien connectés aux datasources
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        
        // On recherche un étudiant avec le même numéro étudiant
        List<Etudiant> existe = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.NumEtud == etudiant.NumEtud);

        // Si un étudiant avec le même numéro étudiant existe déjà, on lève une exception
        if (existe.Count > 0) 
            throw new DuplicateNumEtudException(etudiant.NumEtud + " - ce numéro d'étudiant est déjà affecté");
        
        // Vérification du format du mail
        if (!CheckEmail.IsValidEmail(etudiant.Email)) 
            throw new InvalidEmailException(etudiant.Email + " - Email mal formé");
        
        // On vérifie si l'email est déjà utilisé
        existe = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.Email == etudiant.Email);
        
        if (existe.Count > 0) 
            throw new DuplicateEmailException(etudiant.Email + " est déjà affecté à un étudiant");
        
        // Le métier définit que le nom doit contenir plus de 3 lettres
        if (etudiant.Nom.Length < 3) 
            throw new InvalidNomEtudiantException(etudiant.Nom + " incorrect - Le nom d'un étudiant doit contenir plus de 3 caractères");
    }
}