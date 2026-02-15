using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases;
using UniversiteDomain.UseCases.NoteUseCases.ImportFromCsv;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteEFDataProvider.Entities;

namespace UniversiteWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // ========== POST - Importer les notes à partir d'un fichier CSV ==========
        [HttpPost("import-csv/{ueId}")]
        public async Task<IActionResult> ImportNotesFromCsvAsync(long ueId, IFormFile file)
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

            // Vérification de l'autorisation - Seule la Scolarité peut importer
            var uc = new ImportNotesFromCsvUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role))
                return Unauthorized(new { message = "Seule la Scolarité peut importer les notes" });

            // Validation du fichier
            if (file == null || file.Length == 0)
                return BadRequest("Le fichier CSV est vide ou manquant");

            if (!file.FileName.EndsWith(".csv"))
                return BadRequest("Le fichier doit être au format CSV");

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var (successCount, errors) = await uc.ExecuteAsync(stream, ueId);

                    // Construire la réponse
                    var response = new
                    {
                        success = true,
                        successCount = successCount,
                        errorCount = errors.Count,
                        errors = errors
                    };

                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    success = false,
                    message = e.Message
                });
            }
        }

        // ========== GET - Récupérer toutes les notes ==========
        [HttpGet]
        public async Task<IActionResult> GetAllNotesAsync()
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
                var notes = await repositoryFactory.NoteRepository().FindAllAsync();
                return Ok(notes);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // ========== GET - Récupérer les notes d'une UE ==========
        [HttpGet("ue/{ueId}")]
        public async Task<IActionResult> GetNotesByUeAsync(long ueId)
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
                var notes = await repositoryFactory.NoteRepository().FindAllAsync();
                var notesByUe = notes.Where(n => n.UeId == ueId).ToList();
                return Ok(notesByUe);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // ========== MÉTHODE PRIVÉE - Vérification de sécurité ==========
        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = "";
            email = "";
            user = null;

            ClaimsPrincipal claims = HttpContext.User;

            if (claims.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException();

            if (claims.FindFirst(ClaimTypes.Email) == null)
                throw new UnauthorizedAccessException();

            email = claims.FindFirst(ClaimTypes.Email).Value;
            if (email == null)
                throw new UnauthorizedAccessException();

            user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
            if (user == null)
                throw new UnauthorizedAccessException();

            if (claims.FindFirst(ClaimTypes.Role) == null)
                throw new UnauthorizedAccessException();

            var ident = claims.Identities.FirstOrDefault();
            if (ident == null)
                throw new UnauthorizedAccessException();

            role = ident.FindFirst(ClaimTypes.Role).Value;
            if (role == null)
                throw new UnauthorizedAccessException();

            bool isInRole = new IsInRoleUseCase(repositoryFactory).ExecuteAsync(email, role).Result;
            if (!isInRole)
                throw new UnauthorizedAccessException();
        }
    }
}