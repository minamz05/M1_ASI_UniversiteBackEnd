using System.Linq.Expressions;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class AddNoteToEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(long idEtudiant, long idUe, float valeur)
    {
        var note = new Note(valeur, idEtudiant, idUe);
        return await ExecuteAsync(note);
    }

    public async Task<Note> ExecuteAsync(Note note)
    {
        await CheckBusinessRules(note);
        Note createdNote = await repositoryFactory.NoteRepository().CreateAsync(note);
        await repositoryFactory.SaveChangesAsync();
        return createdNote;
    }

    private async Task CheckBusinessRules(Note note)
    {
        // Vérification des paramètres
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(note.EtudiantId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(note.UeId);

        // Vérifier que la note est entre 0 et 20 EN PREMIER (avant les autres vérifications)
        if (note.Valeur < 0 || note.Valeur > 20)
            throw new InvalidNoteValueException(note.Valeur);

        // Vérifier que les repositories existent
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());

        // On recherche l'étudiant
        List<Etudiant> etudiants = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.Id.Equals(note.EtudiantId));
        if (etudiants == null || etudiants.Count == 0)
            throw new EtudiantNotFoundException(note.EtudiantId.ToString());

        Etudiant etudiant = etudiants[0];

        // On recherche l'UE
        List<Ue> ues = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.Id.Equals(note.UeId));
        if (ues == null || ues.Count == 0)
            throw new UeNotFoundException(note.UeId.ToString());

        // Vérifier que l'étudiant est inscrit dans un parcours
        if (etudiant.ParcoursSuivi == null || etudiant.ParcoursSuivi.Id == 0)
            throw new EtudiantNotInParcoursException($"L'étudiant {note.EtudiantId} n'est inscrit dans aucun parcours");

        // Vérifier que l'UE est dans le parcours de l'étudiant
        List<Parcours> parcours = await repositoryFactory.ParcoursRepository()
            .FindByConditionAsync(p => p.Id.Equals(etudiant.ParcoursSuivi.Id));
        if (parcours == null || parcours.Count == 0)
            throw new ParcoursNotFoundException(etudiant.ParcoursSuivi.Id.ToString());

        Parcours parcoursEtudiant = parcours[0];
        if (parcoursEtudiant.UesEnseignees == null || !parcoursEtudiant.UesEnseignees.Any(u => u.Id.Equals(note.UeId)))
            throw new UeNotInParcoursException($"L'UE {note.UeId} n'est pas dans le parcours {etudiant.ParcoursSuivi.Id}");

        // Vérifier que l'étudiant n'a pas déjà une note dans cette UE
        List<Note> notesExistantes = await repositoryFactory.NoteRepository()
            .FindByConditionAsync(n => n.EtudiantId.Equals(note.EtudiantId) && n.UeId.Equals(note.UeId));
        if (notesExistantes != null && notesExistantes.Count > 0)
            throw new DuplicateNoteException($"L'étudiant {note.EtudiantId} a déjà une note dans l'UE {note.UeId}");
    }
}