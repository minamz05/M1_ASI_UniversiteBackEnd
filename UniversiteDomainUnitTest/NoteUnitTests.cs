using Moq;
using System.Linq.Expressions;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.NoteUseCases.Create;

namespace UniversiteDomainUnitTests;

public class NoteUnitTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task AddNoteToEtudiant_WithValidData_ShouldSucceed()
    {
        // Arrange
        long idEtudiant = 1;
        long idUe = 1;
        long idParcours = 1;
        float valeur = 15.5f;

        var parcours = new Parcours { Id = idParcours, NomParcours = "Informatique", AnneeFormation = 2024 };
        var ue = new Ue { Id = idUe, NumeroUe = "UE001", Intitule = "Programmation C#" };
        parcours.UesEnseignees = new List<Ue> { ue };

        var etudiant = new Etudiant 
        { 
            Id = idEtudiant, 
            NumEtud = "E001", 
            Nom = "Dupont", 
            Prenom = "Jean", 
            Email = "jean@example.com", 
            ParcoursSuivi = parcours 
        };

        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockUe = new Mock<IUeRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        var mockNote = new Mock<INoteRepository>();

        // Configurer les mocks
        mockEtudiant
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });

        mockUe
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        mockParcours
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours> { parcours });

        mockNote
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(new List<Note>());

        var noteCreated = new Note { Id = 1, Valeur = valeur, EtudiantId = idEtudiant, UeId = idUe };
        mockNote.Setup(repo => repo.CreateAsync(It.IsAny<Note>())).ReturnsAsync(noteCreated);
        mockNote.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(mockParcours.Object);
        mockFactory.Setup(facto => facto.NoteRepository()).Returns(mockNote.Object);
        mockFactory.Setup(facto => facto.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var useCase = new AddNoteToEtudiantUseCase(mockFactory.Object);
        var result = await useCase.ExecuteAsync(idEtudiant, idUe, valeur);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Valeur, Is.EqualTo(valeur));
        Assert.That(result.EtudiantId, Is.EqualTo(idEtudiant));
        Assert.That(result.UeId, Is.EqualTo(idUe));
    }

    [Test]
    public void AddNoteToEtudiant_WithInvalidNoteValue_ShouldThrowException()
    {
        // Arrange
        long idEtudiant = 1;
        long idUe = 1;
        float valeur = 25f; // Plus de 20

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.EtudiantRepository()).Returns(new Mock<IEtudiantRepository>().Object);
        mockFactory.Setup(facto => facto.UeRepository()).Returns(new Mock<IUeRepository>().Object);
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(new Mock<IParcoursRepository>().Object);
        mockFactory.Setup(facto => facto.NoteRepository()).Returns(new Mock<INoteRepository>().Object);

        var useCase = new AddNoteToEtudiantUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<InvalidNoteValueException>(
            () => useCase.ExecuteAsync(idEtudiant, idUe, valeur)
        );
    }

    [Test]
    public void AddNoteToEtudiant_WithNegativeNoteValue_ShouldThrowException()
    {
        // Arrange
        long idEtudiant = 1;
        long idUe = 1;
        float valeur = -5f; // Négatif

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.EtudiantRepository()).Returns(new Mock<IEtudiantRepository>().Object);
        mockFactory.Setup(facto => facto.UeRepository()).Returns(new Mock<IUeRepository>().Object);
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(new Mock<IParcoursRepository>().Object);
        mockFactory.Setup(facto => facto.NoteRepository()).Returns(new Mock<INoteRepository>().Object);

        var useCase = new AddNoteToEtudiantUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<InvalidNoteValueException>(
            () => useCase.ExecuteAsync(idEtudiant, idUe, valeur)
        );
    }

    [Test]
    public void AddNoteToEtudiant_WithEtudiantNotFound_ShouldThrowException()
    {
        // Arrange
        long idEtudiant = 1;
        long idUe = 1;
        float valeur = 15f;

        var mockEtudiant = new Mock<IEtudiantRepository>();
        mockEtudiant
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant>());

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto => facto.UeRepository()).Returns(new Mock<IUeRepository>().Object);
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(new Mock<IParcoursRepository>().Object);
        mockFactory.Setup(facto => facto.NoteRepository()).Returns(new Mock<INoteRepository>().Object);

        var useCase = new AddNoteToEtudiantUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<EtudiantNotFoundException>(
            () => useCase.ExecuteAsync(idEtudiant, idUe, valeur)
        );
    }

    [Test]
    public void AddNoteToEtudiant_WithUeNotFound_ShouldThrowException()
    {
        // Arrange
        long idEtudiant = 1;
        long idUe = 1;
        long idParcours = 1;
        float valeur = 15f;

        var parcours = new Parcours { Id = idParcours, NomParcours = "Informatique", AnneeFormation = 2024 };
        var etudiant = new Etudiant 
        { 
            Id = idEtudiant, 
            NumEtud = "E001", 
            Nom = "Dupont", 
            Prenom = "Jean", 
            Email = "jean@example.com", 
            ParcoursSuivi = parcours 
        };

        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockUe = new Mock<IUeRepository>();

        mockEtudiant
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });

        mockUe
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue>());

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(new Mock<IParcoursRepository>().Object);
        mockFactory.Setup(facto => facto.NoteRepository()).Returns(new Mock<INoteRepository>().Object);

        var useCase = new AddNoteToEtudiantUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<UeNotFoundException>(
            () => useCase.ExecuteAsync(idEtudiant, idUe, valeur)
        );
    }

    [Test]
    public void AddNoteToEtudiant_WithEtudiantNotInParcours_ShouldThrowException()
    {
        // Arrange
        long idEtudiant = 1;
        long idUe = 1;
        float valeur = 15f;

        var etudiant = new Etudiant 
        { 
            Id = idEtudiant, 
            NumEtud = "E001", 
            Nom = "Dupont", 
            Prenom = "Jean", 
            Email = "jean@example.com", 
            ParcoursSuivi = null  // Pas inscrit dans un parcours
        };

        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockUe = new Mock<IUeRepository>();

        mockEtudiant
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });

        mockUe
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { new Ue { Id = idUe, NumeroUe = "UE001", Intitule = "Test" } });

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(new Mock<IParcoursRepository>().Object);
        mockFactory.Setup(facto => facto.NoteRepository()).Returns(new Mock<INoteRepository>().Object);

        var useCase = new AddNoteToEtudiantUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<EtudiantNotInParcoursException>(
            () => useCase.ExecuteAsync(idEtudiant, idUe, valeur)
        );
    }

    [Test]
    public void AddNoteToEtudiant_WithUeNotInParcours_ShouldThrowException()
    {
        // Arrange
        long idEtudiant = 1;
        long idUe = 1;
        long idParcours = 1;
        float valeur = 15f;

        var parcours = new Parcours { Id = idParcours, NomParcours = "Informatique", AnneeFormation = 2024 };
        parcours.UesEnseignees = new List<Ue>(); // UE pas dans le parcours

        var ue = new Ue { Id = idUe, NumeroUe = "UE001", Intitule = "Programmation C#" };

        var etudiant = new Etudiant 
        { 
            Id = idEtudiant, 
            NumEtud = "E001", 
            Nom = "Dupont", 
            Prenom = "Jean", 
            Email = "jean@example.com", 
            ParcoursSuivi = parcours 
        };

        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockUe = new Mock<IUeRepository>();
        var mockParcours = new Mock<IParcoursRepository>();

        mockEtudiant
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });

        mockUe
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        mockParcours
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours> { parcours });

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(mockParcours.Object);
        mockFactory.Setup(facto => facto.NoteRepository()).Returns(new Mock<INoteRepository>().Object);

        var useCase = new AddNoteToEtudiantUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<UeNotInParcoursException>(
            () => useCase.ExecuteAsync(idEtudiant, idUe, valeur)
        );
    }

    [Test]
    public void AddNoteToEtudiant_WithDuplicateNote_ShouldThrowException()
    {
        // Arrange
        long idEtudiant = 1;
        long idUe = 1;
        long idParcours = 1;
        float valeur = 15.5f;

        var parcours = new Parcours { Id = idParcours, NomParcours = "Informatique", AnneeFormation = 2024 };
        var ue = new Ue { Id = idUe, NumeroUe = "UE001", Intitule = "Programmation C#" };
        parcours.UesEnseignees = new List<Ue> { ue };

        var etudiant = new Etudiant 
        { 
            Id = idEtudiant, 
            NumEtud = "E001", 
            Nom = "Dupont", 
            Prenom = "Jean", 
            Email = "jean@example.com", 
            ParcoursSuivi = parcours 
        };

        var existingNote = new Note { Id = 1, Valeur = 12f, EtudiantId = idEtudiant, UeId = idUe };

        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockUe = new Mock<IUeRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        var mockNote = new Mock<INoteRepository>();

        mockEtudiant
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });

        mockUe
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        mockParcours
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours> { parcours });

        mockNote
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(new List<Note> { existingNote });

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(mockParcours.Object);
        mockFactory.Setup(facto => facto.NoteRepository()).Returns(mockNote.Object);

        var useCase = new AddNoteToEtudiantUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<DuplicateNoteException>(
            () => useCase.ExecuteAsync(idEtudiant, idUe, valeur)
        );
    }
}