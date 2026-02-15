using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Repositories;
using UniversiteEFDataProvider.Data;
using Microsoft.AspNetCore.Identity;
using UniversiteEFDataProvider.Entities;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory(UniversiteDbContext context, RoleManager<UniversiteRole> roleManager, UserManager<UniversiteUser> userManager) : IRepositoryFactory
{
    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;
    private IUniversiteRoleRepository? _roles;
    private IUniversiteUserRepository? _users;
    
    public IParcoursRepository ParcoursRepository()
    {
        if (_parcours == null)
        {
            _parcours = new ParcoursRepository(context ?? throw new InvalidOperationException());
        }
        return _parcours;
    }

    public IEtudiantRepository EtudiantRepository()
    {
        if (_etudiants == null)
        {
            _etudiants = new EtudiantRepository(context ?? throw new InvalidOperationException());
        }
        return _etudiants;
    }

    public IUeRepository UeRepository()
    {
        if (_ues == null)
        {
            _ues = new UeRepository(context ?? throw new InvalidOperationException());
        }
        return _ues;
    }

    public INoteRepository NoteRepository()
    {
        if (_notes == null)
        {
            _notes = new NoteRepository(context ?? throw new InvalidOperationException());
        }
        return _notes;
    }

    public IUniversiteRoleRepository UniversiteRoleRepository()
    {
        if (_roles == null)
        {
            _roles = new UniversiteRoleRepository(context ?? throw new InvalidOperationException(), roleManager ?? throw new InvalidOperationException());
        }
        return _roles;
    }

    public IUniversiteUserRepository UniversiteUserRepository()
    {
        if (_users == null)
        {
            _users = new UniversiteUserRepository(context ?? throw new InvalidOperationException(), userManager ?? throw new InvalidOperationException(), roleManager ?? throw new InvalidOperationException());
        }
        return _users;
    }
       
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task EnsureCreatedAsync()
    {
        context.Database.EnsureCreated();
        await Task.CompletedTask;
    }

    public async Task EnsureDeletedAsync()
    {
        context.Database.EnsureDeleted();
        await Task.CompletedTask;
    }
}