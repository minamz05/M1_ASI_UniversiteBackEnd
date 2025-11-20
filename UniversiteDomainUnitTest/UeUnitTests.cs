using Moq;
using System.Linq.Expressions;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.UeUseCases.Create;

namespace UniversiteDomainUnitTests;

public class UeUnitTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateUeUseCase_WithValidData_ShouldSucceed()
    {
        long idUe = 1;
        string numeroUe = "UE001";
        string intitule = "Programmation C#";

        // On initialise une fausse datasource qui va simuler un IUeRepository
        var mockUe = new Mock<IUeRepository>();

        // On dit à ce mock que l'UE n'existe pas déjà
        mockUe
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue>());

        // On lui dit que l'ajout d'une UE renvoie une UE avec l'Id 1
        Ue ueFinal = new Ue { Id = idUe, NumeroUe = numeroUe, Intitule = intitule };
        mockUe.Setup(repo => repo.CreateAsync(It.IsAny<Ue>())).ReturnsAsync(ueFinal);

        // On configure le mock pour SaveChangesAsync
        mockUe.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);

        // Création du use case en utilisant le mock comme datasource
        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);

        // Appel du use case
        var ueTestee = await useCase.ExecuteAsync(numeroUe, intitule);

        // Vérification du résultat
        Assert.That(ueTestee, Is.Not.Null);
        Assert.That(ueTestee.Id, Is.EqualTo(ueFinal.Id));
        Assert.That(ueTestee.NumeroUe, Is.EqualTo(ueFinal.NumeroUe));
        Assert.That(ueTestee.Intitule, Is.EqualTo(ueFinal.Intitule));
    }

    [Test]
    public void CreateUeUseCase_WithDuplicateNumero_ShouldThrowException()
    {
        string numeroUe = "UE001";
        string intitule = "Programmation C#";

        // On crée une UE existante avec le même numéro
        Ue ueExistante = new Ue { Id = 1, NumeroUe = numeroUe, Intitule = "Autre intitulé" };

        // On initialise le mock
        var mockUe = new Mock<IUeRepository>();

        // Configurer pour retourner une liste avec l'UE existante
        mockUe
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ueExistante });

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);

        // Création du use case
        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);

        // Appel du use case et vérification que l'exception est levée
        Assert.ThrowsAsync<DuplicateNumeroUeException>(
            () => useCase.ExecuteAsync(numeroUe, intitule)
        );
    }

    [Test]
    public void CreateUeUseCase_WithIntituleTooShort_ShouldThrowException()
    {
        string numeroUe = "UE001";
        string intitule = "AB"; // Moins de 3 caractères

        var mockUe = new Mock<IUeRepository>();
        mockUe
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue>());

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);

        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);

        Assert.ThrowsAsync<InvalidIntituleUeException>(
            () => useCase.ExecuteAsync(numeroUe, intitule)
        );
    }

    [Test]
    public void CreateUeUseCase_WithEmptyIntitule_ShouldThrowException()
    {
        string numeroUe = "UE001";
        string intitule = "";

        var mockUe = new Mock<IUeRepository>();
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);

        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);

        Assert.ThrowsAsync<InvalidIntituleUeException>(
            () => useCase.ExecuteAsync(numeroUe, intitule)
        );
    }

    [Test]
    public void CreateUeUseCase_WithNullIntitule_ShouldThrowException()
    {
        var mockUe = new Mock<IUeRepository>();
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);

        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);

        Assert.ThrowsAsync<InvalidIntituleUeException>(
            () => useCase.ExecuteAsync("UE001", null!)
        );
    }

    [Test]
    public void CreateUeUseCase_WithEmptyNumero_ShouldThrowException()
    {
        string numeroUe = "";
        string intitule = "Programmation C#";

        var mockUe = new Mock<IUeRepository>();
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);

        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);

        Assert.ThrowsAsync<InvalidNumeroUeException>(
            () => useCase.ExecuteAsync(numeroUe, intitule)
        );
    }

    [Test]
    public void CreateUeUseCase_WithNullNumero_ShouldThrowException()
    {
        string intitule = "Programmation C#";

        var mockUe = new Mock<IUeRepository>();
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);

        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);

        Assert.ThrowsAsync<InvalidNumeroUeException>(
            () => useCase.ExecuteAsync(null!, intitule)
        );
    }
}