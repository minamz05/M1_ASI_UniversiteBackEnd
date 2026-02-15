using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.ImportFromCsv;

public class ImportNotesFromCsvUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<(int success, List<string> errors)> ExecuteAsync(Stream csvStream, long ueId)
    {
        int successCount = 0;
        List<string> errors = new List<string>();

        try
        {
            using (var reader = new StreamReader(csvStream))
            {
                // ✅ CONFIGURATION POUR POINT-VIRGULE
                var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                {
                    Delimiter = ";"
                };
            
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<NoteCsvDto>();
                    int lineNumber = 2;

                    foreach (var record in records)
                    {
                        try
                        {
                            await ProcessNoteAsync(record, ueId);
                            successCount++;
                        }
                        catch (Exception e)
                        {
                            errors.Add($"Ligne {lineNumber}: {e.Message}");
                        }
                        lineNumber++;
                    }
                }
            }
        }
        catch (Exception e)
        {
            errors.Add($"Erreur de lecture du CSV: {e.Message}");
        }

        return (successCount, errors);
    }

private async Task ProcessNoteAsync(NoteCsvDto noteDto, long ueId)
{
    // Validation
    if (string.IsNullOrEmpty(noteDto.NumEtud))
        throw new ArgumentException("NumEtud est obligatoire");

    if (noteDto.Valeur.HasValue && (noteDto.Valeur < 0 || noteDto.Valeur > 20))
        throw new ArgumentException("La note doit être entre 0 et 20");

    // VÉRIFIER L'UE
    var ue = await repositoryFactory.UeRepository().FindAsync(ueId);
    if (ue == null)
        throw new ArgumentException($"UE avec ID {ueId} non trouvée");

    // VÉRIFIER QUE LE NUMÉRO UE CORRESPOND
    if (!string.IsNullOrEmpty(noteDto.NumeroUe) && ue.NumeroUe != noteDto.NumeroUe)
        throw new ArgumentException($"L'UE '{noteDto.NumeroUe}' ne correspond pas à l'UE attendue '{ue.NumeroUe}'");

    // RÉCUPÉRER L'ÉTUDIANT ET VÉRIFIER NOM/PRENOM
    var etudiants = await repositoryFactory.EtudiantRepository().FindAllAsync();
    var etudiant = etudiants.FirstOrDefault(e => e.NumEtud == noteDto.NumEtud);

    if (etudiant == null)
        throw new ArgumentException($"Étudiant '{noteDto.NumEtud}' non trouvé");

    //  VÉRIFIER QUE NOM ET PRENOM CORRESPONDENT
    if (!string.IsNullOrEmpty(noteDto.Nom) && etudiant.Nom != noteDto.Nom)
        throw new ArgumentException($"Le nom '{noteDto.Nom}' ne correspond pas pour {noteDto.NumEtud}");

    if (!string.IsNullOrEmpty(noteDto.Prenom) && etudiant.Prenom != noteDto.Prenom)
        throw new ArgumentException($"Le prénom '{noteDto.Prenom}' ne correspond pas pour {noteDto.NumEtud}");

    // Créer/Mettre à jour la note
    if (noteDto.Valeur.HasValue)
    {
        var notesEtudiant = await repositoryFactory.NoteRepository().FindAllAsync();
        var noteExistante = notesEtudiant.FirstOrDefault(n => n.EtudiantId == etudiant.Id && n.UeId == ueId);

        if (noteExistante != null)
        {
            noteExistante.Valeur = noteDto.Valeur.Value;
            await repositoryFactory.NoteRepository().UpdateAsync(noteExistante);
            await repositoryFactory.SaveChangesAsync();
        }
        else
        {
            Note nouvelleNote = new Note
            {
                EtudiantId = etudiant.Id,
                UeId = ueId,
                Valeur = noteDto.Valeur.Value
            };
            await repositoryFactory.NoteRepository().CreateAsync(nouvelleNote);
            await repositoryFactory.SaveChangesAsync();
        }
    }
}

    public bool IsAuthorized(string role)
    {
        // Seule la Scolarité peut importer les notes
        return role == Roles.Scolarite;
    }
}