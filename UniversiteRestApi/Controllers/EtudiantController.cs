using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.UseCases.EtudiantUseCases.Delete;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.SecurityUseCases.Create;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteEFDataProvider.Entities;

namespace UniversiteWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // ========== GET ALL - Retourner tous les étudiants ==========
        [HttpGet]
        public async Task<ActionResult<List<EtudiantDto>>> GetAsync()
        {
            // Identification et authentification
            string role = "";
            string email = "";
            IUniversiteUser user = null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            try
            {
                List<Etudiant> etudiants = await repositoryFactory.EtudiantRepository().FindAllAsync();
                List<EtudiantDto> dtos = etudiants.ConvertAll(e => new EtudiantDto().ToDto(e));
                return Ok(dtos);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        // ========== GET ALL WITH NOTES - Retourner tous les étudiants avec leurs notes ==========
        [HttpGet("withnotes")]
        public async Task<ActionResult<List<EtudiantCompletDto>>> GetWithNotesAsync()
        {
            // Identification et authentification
            string role = "";
            string email = "";
            IUniversiteUser user = null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            try
            {
                // ✅ CHANGEZ CETTE LIGNE
                List<Etudiant> etudiants = await repositoryFactory.EtudiantRepository().FindAllEtudiantsCompletAsync();
                List<EtudiantCompletDto> dtos = etudiants.ConvertAll(e => new EtudiantCompletDto().ToDto(e));
                return Ok(dtos);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        // ========== GET ONE - Retourner un étudiant par ID ==========
        [HttpGet("{id}")]
        public async Task<ActionResult<EtudiantDto>> GetUnEtudiantAsync(long id)
        {
            // Identification et authentification
            string role = "";
            string email = "";
            IUniversiteUser user = null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            try
            {
                Etudiant? etud = await repositoryFactory.EtudiantRepository().FindAsync(id);
                if (etud == null) return NotFound();
                
                EtudiantDto dto = new EtudiantDto().ToDto(etud);
                return Ok(dto);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        // ========== GET COMPLET - Retourner un étudiant avec ses notes ==========
        [HttpGet("complet/{id}")]
        public async Task<ActionResult<EtudiantCompletDto>> GetUnEtudiantCompletAsync(long id)
        {
            // Identification et authentification
            string role = "";
            string email = "";
            IUniversiteUser user = null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            GetEtudiantCompletUseCase uc = new GetEtudiantCompletUseCase(repositoryFactory);
            
            // Autorisation
            // On vérifie si l'utilisateur connecté a le droit d'accéder à la ressource
            if (!uc.IsAuthorized(role, user, id)) return Unauthorized();
            
            Etudiant? etud;
            try
            {
                etud = await uc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            if (etud == null) return NotFound();
            return new EtudiantCompletDto().ToDto(etud);
        }

        // ========== POST - Créer un nouvel étudiant ==========
        [HttpPost]
        public async Task<ActionResult<EtudiantDto>> PostAsync([FromBody] EtudiantDto etudiantDto)
        {
            CreateEtudiantUseCase createEtudiantUc = new CreateEtudiantUseCase(repositoryFactory);
            CreateUniversiteUserUseCase createUserUc = new CreateUniversiteUserUseCase(repositoryFactory);

            string role = "";
            string email = "";
            IUniversiteUser user = null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            if (!createEtudiantUc.IsAuthorized(role) || !createUserUc.IsAuthorized(role)) 
                return Unauthorized();
            
            Etudiant etud = etudiantDto.ToEntity();
            
            try
            {
                etud = await createEtudiantUc.ExecuteAsync(etud);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            try
            {
                // Création du user associé
                user = new UniversiteUser { UserName = etudiantDto.Email, Email = etudiantDto.Email, Etudiant = etud };
                // On crée l'utilisateur avec un mot de passe par défaut et un rôle étudiant
                await createUserUc.ExecuteAsync(etud.Email, etud.Email, "Miage2025#", Roles.Etudiant, etud); 
            }
            catch (Exception e)
            {
                // On supprime l'étudiant que l'on vient de créer. Sinon on a un étudiant mais pas de user associé
                await new DeleteEtudiantUseCase(repositoryFactory).ExecuteAsync(etud.Id);
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            EtudiantDto dto = new EtudiantDto().ToDto(etud);
            return CreatedAtAction(nameof(GetUnEtudiantAsync), new { id = dto.Id }, dto);
        }

        // ========== PUT - Modifier un étudiant ==========
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(long id, [FromBody] EtudiantDto etudiantDto)
        {
            // Identification et authentification
            string role = "";
            string email = "";
            IUniversiteUser user = null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            if (id != etudiantDto.Id)
                return BadRequest("L'ID de l'étudiant ne correspond pas");

            try
            {
                Etudiant etud = etudiantDto.ToEntity();
                await repositoryFactory.EtudiantRepository().UpdateAsync(etud);
                await repositoryFactory.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        // ========== DELETE - Supprimer un étudiant ==========
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            // Identification et authentification
            string role = "";
            string email = "";
            IUniversiteUser user = null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            try
            {
                await new DeleteEtudiantUseCase(repositoryFactory).ExecuteAsync(id);
                return NoContent();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        // ========== MÉTHODE PRIVÉE - Vérification de sécurité ==========
        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = "";
            email = "";
            user = null;

            // Récupération des informations de connexion dans la requête http entrante
            ClaimsPrincipal claims = HttpContext.User;
            
            // Faisons nos tests pour savoir si la personne est bien connectée
            if (claims.Identity?.IsAuthenticated != true) 
                throw new UnauthorizedAccessException();
            
            // Récupérons le email de la personne connectée
            if (claims.FindFirst(ClaimTypes.Email) == null) 
                throw new UnauthorizedAccessException();
            
            email = claims.FindFirst(ClaimTypes.Email).Value;
            if (email == null) 
                throw new UnauthorizedAccessException();
            
            // Vérifions qu'il est bien associé à un utilisateur référencé
            user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
            if (user == null) 
                throw new UnauthorizedAccessException();
            
            // Vérifions qu'un rôle a bien été défini
            if (claims.FindFirst(ClaimTypes.Role) == null) 
                throw new UnauthorizedAccessException();
            
            // Récupérons le rôle de l'utilisateur
            var ident = claims.Identities.FirstOrDefault();
            if (ident == null)
                throw new UnauthorizedAccessException();
            
            role = ident.FindFirst(ClaimTypes.Role).Value;
            if (role == null) 
                throw new UnauthorizedAccessException();
            
            // Vérifions que le user a bien le role envoyé via http
            bool isInRole = new IsInRoleUseCase(repositoryFactory).ExecuteAsync(email, role).Result; 
            if (!isInRole) 
                throw new UnauthorizedAccessException();
            
            // Si tout est passé sans renvoyer d'exception, le user est authentifié et connecté
        }
    }
}