using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Entities;

namespace UniversiteEFDataProvider.Data;
 
public class UniversiteDbContext : IdentityDbContext<UniversiteUser>
{
    public static readonly ILoggerFactory consoleLogger = LoggerFactory.Create(builder => { builder.AddConsole(); });
    
    public UniversiteDbContext(DbContextOptions<UniversiteDbContext> options)
        : base(options)
    {
    }
 
    public UniversiteDbContext():base()
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(consoleLogger)
            .EnableSensitiveDataLogging() 
            .EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

// ========== Configuration des clés Identity pour MySQL ==========
        modelBuilder.Entity<UniversiteUser>().Property(u => u.Id).HasMaxLength(85);
        modelBuilder.Entity<UniversiteRole>().Property(r => r.Id).HasMaxLength(85);
        

// Réduire la taille de NormalizedName pour éviter les problèmes d'index
        modelBuilder.Entity<UniversiteRole>().Property(r => r.NormalizedName).HasMaxLength(128);
        modelBuilder.Entity<UniversiteUser>().Property(u => u.NormalizedEmail).HasMaxLength(128);
        modelBuilder.Entity<UniversiteUser>().Property(u => u.NormalizedUserName).HasMaxLength(128);

// Désactiver les tables Identity inutiles pour éviter les problèmes de clé trop longue
        modelBuilder.Entity<IdentityUserLogin<string>>()
            .ToTable("AspNetUserLogins", t => t.ExcludeFromMigrations());

        modelBuilder.Entity<IdentityUserToken<string>>()
            .ToTable("AspNetUserTokens", t => t.ExcludeFromMigrations());

        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
// ========== Fin configuration Identity ==========

        // ========== Propriétés de la table Etudiant ==========
        modelBuilder.Entity<Etudiant>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.ParcoursSuivi)
            .WithMany(p => p.Inscrits)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Etudiant>()
            .HasMany(e => e.NotesObtenues)
            .WithOne(n => n.Etudiant)
            .OnDelete(DeleteBehavior.Cascade);
        // ========== Fin Etudiant ==========

        // ========== Propriétés de la table Parcours ==========
        modelBuilder.Entity<Parcours>()
            .HasKey(p => p.Id);
        
        modelBuilder.Entity<Parcours>()
            .HasMany(p => p.Inscrits)
            .WithOne(e => e.ParcoursSuivi)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Parcours>()
            .HasMany(p => p.UesEnseignees)
            .WithMany(ue => ue.EnseigneeDans)
            .UsingEntity(j => 
            {
                j.ToTable("ParcoursUe");
                j.HasOne(typeof(Ue)).WithMany().OnDelete(DeleteBehavior.Cascade);
                j.HasOne(typeof(Parcours)).WithMany().OnDelete(DeleteBehavior.Cascade);
            });
        // ========== Fin Parcours ==========

        // ========== Propriétés de la table Ue ==========
        modelBuilder.Entity<Ue>()
            .HasKey(ue => ue.Id);
        
        modelBuilder.Entity<Ue>()
            .HasMany(ue => ue.Notes)
            .WithOne(n => n.Ue)
            .OnDelete(DeleteBehavior.Cascade);
        // ========== Fin Ue ==========

        // ========== Propriétés de la table Note ==========
        modelBuilder.Entity<Note>()
            .HasKey(n => new { n.EtudiantId, n.UeId });
        
        modelBuilder.Entity<Note>()
            .HasOne(n => n.Etudiant)
            .WithMany(e => e.NotesObtenues)
            .HasForeignKey(n => n.EtudiantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Note>()
            .HasOne(n => n.Ue)
            .WithMany(ue => ue.Notes)
            .HasForeignKey(n => n.UeId)
            .OnDelete(DeleteBehavior.Cascade);
        // ========== Fin Note ==========

        // ========== Propriétés de la table UniversiteUser ==========
        modelBuilder.Entity<UniversiteUser>()
            .HasOne(u => u.Etudiant)
            .WithOne()
            .HasForeignKey<UniversiteUser>(u => u.EtudiantId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<UniversiteUser>()
            .Navigation(user => user.Etudiant)
            .AutoInclude();
        // ========== Fin UniversiteUser ==========

        modelBuilder.Entity<UniversiteRole>();
    }

    public DbSet<Parcours>? Parcours { get; set; }
    public DbSet<Etudiant>? Etudiants { get; set; }
    public DbSet<Ue>? Ues { get; set; }
    public DbSet<Note>? Notes { get; set; }
    public DbSet<UniversiteUser>? UniversiteUsers { get; set; }
    public DbSet<UniversiteRole>? UniversiteRoles { get; set; }
}