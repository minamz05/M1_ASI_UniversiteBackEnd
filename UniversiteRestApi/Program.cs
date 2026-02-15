using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.JeuxDeDonnees;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.RepositoryFactories;
using UniversiteEFDataProvider.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// ========== Swagger avec support JWT ==========
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Projet Universite", Version = "v1" });

    // Add Bearer Token Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
// ========== Fin config Swagger ==========

// ========== Config système de log en console ==========
builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.AddConsole();
});
// ========== Fin Log ==========

// ========== Configuration des connexions à MySql ==========
String connectionString = builder.Configuration.GetConnectionString("MySqlConnection") ?? throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");

builder.Services.AddDbContext<UniversiteDbContext>(options => options.UseMySQL(connectionString));
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
// ========== Fin connexion BD ==========

// ========== Sécurisation - Identity et JWT ==========
builder.Services.AddIdentityCore<UniversiteUser>(options =>
{
    // A modifier en prod pour renforcer la sécurité!!!
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddRoles<UniversiteRole>()
    .AddEntityFrameworkStores<UniversiteDbContext>()
    .AddApiEndpoints()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
// ========== Fin Sécurisation ==========

var app = builder.Build();

// ========== Configuration du serveur Web ==========
app.UseHttpsRedirection();

// ========== Swagger ==========
app.UseSwagger();
app.UseSwaggerUI();
// ========== Fin Swagger ==========

// ========== Sécurisation ==========
app.UseAuthentication();
app.UseAuthorization();
// ========== Fin Sécurisation ==========

app.MapControllers();

// ========== Initialisation de la base de données ==========
ILogger logger = app.Services.GetRequiredService<ILogger<BdBuilder>>();
logger.LogInformation("Chargement des données de test");
using(var scope = app.Services.CreateScope())
{
    UniversiteDbContext context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    IRepositoryFactory repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
    UserManager<UniversiteUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<UniversiteUser>>();
    RoleManager<UniversiteRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UniversiteRole>>();
    
    BdBuilder seedBD = new BasicBdBuilder(repositoryFactory);
    await seedBD.BuildUniversiteBdAsync();
}
// ========== Fin Initialisation ==========

// Exécution de l'application
app.Run();