using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

namespace UniversiteDomainUnitTests;

public class EtudiantUnitTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateEtudiantUseCase()
    {
        long id = 1;
        string numEtud = "et1";
        string nom = "Durant";
        string prenom = "Jean";
        string email = "jean.durant@etud.u-picardie.fr";

        // Étudiant à créer
        Etudiant etudiantSansId = new Etudiant
        {
            NumEtud = numEtud,
            Nom = nom,
            Prenom = prenom,
            Email = email
        };

        // Mock du repository Etudiant
        var mockEtudiantRepository = new Mock<IEtudiantRepository>();

        // Simulation : l'étudiant n'existe pas déjà
        mockEtudiantRepository
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant>());  // retourne liste vide

        // Simulation : après création, l'étudiant reçoit un ID
        Etudiant etudiantCree = new Etudiant
        {
            Id = id,
            NumEtud = numEtud,
            Nom = nom,
            Prenom = prenom,
            Email = email
        };

        mockEtudiantRepository
            .Setup(repo => repo.CreateAsync(etudiantSansId))
            .ReturnsAsync(etudiantCree);

        // Mock de la factory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory
            .Setup(factory => factory.EtudiantRepository())
            .Returns(mockEtudiantRepository.Object);

        // Use case avec factory
        var useCase = new CreateEtudiantUseCase(mockFactory.Object);

        // Appel du use case
        var resultat = await useCase.ExecuteAsync(etudiantSansId);

        // Assertions
        Assert.That(resultat.Id, Is.EqualTo(etudiantCree.Id));
        Assert.That(resultat.NumEtud, Is.EqualTo(etudiantCree.NumEtud));
        Assert.That(resultat.Nom, Is.EqualTo(etudiantCree.Nom));
        Assert.That(resultat.Prenom, Is.EqualTo(etudiantCree.Prenom));
        Assert.That(resultat.Email, Is.EqualTo(etudiantCree.Email));
    }
}
