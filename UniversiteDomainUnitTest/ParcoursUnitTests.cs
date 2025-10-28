using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomainUnitTests;

public class ParcoursUnitTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]

    public async Task CreateParcoursUseCase()
    {
        // Arrange - Préparation des données de test
        long id = 1;
        string nomParcours = "MIAGE";
        int anneeFormation = 2000; // ⬅️ Année valide (entre 1900 et 2100)

        // On crée le parcours qui doit être ajouté en base
        Parcours parcoursSansId = new Parcours
        {
            NomParcours = nomParcours,
            AnneeFormation = anneeFormation
        };

        // Créons le mock du repository
        var mock = new Mock<IParcoursRepository>();

        // Simulation de la fonction FindByConditionAsync
        var reponseFindByCondition = new List<Parcours>();

        mock.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(reponseFindByCondition);

        // Simulation de la fonction CreateAsync
        Parcours parcoursCree = new Parcours
        {
            Id = id,
            NomParcours = nomParcours,
            AnneeFormation = anneeFormation
        };
        
        mock.Setup(repo => repo.CreateAsync(It.IsAny<Parcours>()))  // ⬅️ It.IsAny pour matcher n'importe quel parcours
            .ReturnsAsync(parcoursCree);

        // Simulation de SaveChangesAsync
        mock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var fauxParcoursRepository = mock.Object;

        // Act - Création du use case
        CreateParcoursUseCase useCase = new CreateParcoursUseCase(fauxParcoursRepository);

        // Appel du use case
        var parcoursTeste = await useCase.ExecuteAsync(parcoursSansId);

        // Assert - Vérification du résultat
        Assert.That(parcoursTeste.Id, Is.EqualTo(parcoursCree.Id));
        Assert.That(parcoursTeste.NomParcours, Is.EqualTo(parcoursCree.NomParcours));
        Assert.That(parcoursTeste.AnneeFormation, Is.EqualTo(parcoursCree.AnneeFormation));
    }

    [Test]
    public async Task CreateParcoursUseCase_WithDuplicateParcours_ShouldThrowException()
    {
        // Arrange
        string nomParcours = "MIAGE";
        int anneeFormation = 2000;

        Parcours nouveauParcours = new Parcours
        {
            NomParcours = nomParcours,
            AnneeFormation = anneeFormation
        };

        Parcours parcoursExistant = new Parcours
        {
            Id = 1,
            NomParcours = nomParcours,
            AnneeFormation = anneeFormation
        };

        var mock = new Mock<IParcoursRepository>();

        var reponseFindByCondition = new List<Parcours> { parcoursExistant };

        mock.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(reponseFindByCondition);

        var fauxParcoursRepository = mock.Object;

        // Act & Assert
        CreateParcoursUseCase useCase = new CreateParcoursUseCase(fauxParcoursRepository);

        var exception = Assert.ThrowsAsync<DuplicateNomParcoursException>(
            async () => await useCase.ExecuteAsync(nouveauParcours)
        );

        // Vérification du message d'erreur
        Assert.That(exception!.Message, Does.Contain(nomParcours));
    }

    [Test]
    public async Task CreateParcoursUseCase_WithInvalidNomParcours_ShouldThrowException()
    {
        // Arrange
        string nomParcours = "IA"; // ⬅️ Trop court (< 3 caractères)
        int anneeFormation = 2000;

        Parcours nouveauParcours = new Parcours
        {
            NomParcours = nomParcours,
            AnneeFormation = anneeFormation
        };

        var mock = new Mock<IParcoursRepository>();

        var reponseFindByCondition = new List<Parcours>();

        mock.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(reponseFindByCondition);

        var fauxParcoursRepository = mock.Object;

        // Act & Assert
        CreateParcoursUseCase useCase = new CreateParcoursUseCase(fauxParcoursRepository);

        var exception = Assert.ThrowsAsync<InvalidNomParcoursException>(
            async () => await useCase.ExecuteAsync(nouveauParcours)
        );

        // Vérification du message d'erreur
        Assert.That(exception!.Message, Does.Contain("nom").IgnoreCase);  // ⬅️ Correction syntaxe
    }

    [Test]
    public async Task CreateParcoursUseCase_WithInvalidAnneeFormation_ShouldThrowException()
    {
        // Arrange
        string nomParcours = "MIAGE";
        int anneeFormation = 1500; // ⬅️ Invalide (< 1900)

        Parcours nouveauParcours = new Parcours
        {
            NomParcours = nomParcours,
            AnneeFormation = anneeFormation
        };

        var mock = new Mock<IParcoursRepository>();

        var reponseFindByCondition = new List<Parcours>();

        mock.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(reponseFindByCondition);

        var fauxParcoursRepository = mock.Object;

        // Act & Assert
        CreateParcoursUseCase useCase = new CreateParcoursUseCase(fauxParcoursRepository);

        var exception = Assert.ThrowsAsync<InvalidAnneeFormationException>(
            async () => await useCase.ExecuteAsync(nouveauParcours)
        );

        // Vérification du message d'erreur
        Assert.That(exception!.Message, Does.Contain("année").IgnoreCase);  // ⬅️ Correction syntaxe
    }
   
    [Test]
    public async Task AddEtudiantDansParcoursUseCase()
    {
        long idEtudiant = 1;
        long idParcours = 3;
        Etudiant etudiant= new Etudiant { Id = 1, NumEtud = "1", Nom = "nom1", Prenom = "prenom1", Email = "1" };
        Parcours parcours = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        
        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        List<Etudiant> etudiants = new List<Etudiant>();
        etudiants.Add(new Etudiant{Id=1});
        mockEtudiant
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idEtudiant)))
            .ReturnsAsync(etudiants);

        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);
        
        List<Parcours> parcoursFinaux = new List<Parcours>();
        Parcours parcoursFinal = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        parcoursFinal.Inscrits.Add(etudiant);
        parcoursFinaux.Add(parcours);
        
        mockParcours
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idParcours)))
            .ReturnsAsync(parcourses);
        mockParcours
            .Setup(repo => repo.AddEtudiantAsync(idParcours, idEtudiant))
            .ReturnsAsync(parcoursFinal);
        // Création du use case en utilisant le mock comme datasource
        AddEtudiantDansParcoursUseCase useCase=new AddEtudiantDansParcoursUseCase(mockEtudiant.Object, mockParcours.Object);
        
        // Appel du use case
        var parcoursTest=await useCase.ExecuteAsync(idParcours, idEtudiant);
        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.Inscrits, Is.Not.Null);
        Assert.That(parcoursTest.Inscrits.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.Inscrits[0].Id, Is.EqualTo(idEtudiant));
    }
}
}