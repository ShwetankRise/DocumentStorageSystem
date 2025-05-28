using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocumentStorageSystem.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace DocumentStorageSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly DocumentService _documentService;

        public FilesController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] string? name)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid user ID");

            var fileName = name ?? file.FileName;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var content = memoryStream.ToArray();

            var version = await _documentService.UploadDocumentAsync(userId, fileName, content);
            return Ok(new { message = "File uploaded successfully", version });
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetFile(string fileName, [FromQuery] int? revision)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid user ID");

            try
            {
                // If revision is null, it fetches the latest version (as implemented in service)
                var content = await _documentService.GetDocumentAsync(userId, fileName, revision);

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(fileName, out var contentType))
                    contentType = "application/octet-stream";

                // Return file with the correct content type and original filename
                return File(content, contentType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid user ID");

            var files = await _documentService.GetUserDocumentsAsync(userId);
            return Ok(new
            {
                files = files.Select(f => new
                {
                    f.Name,
                    f.Version,
                    f.UploadDate
                })
            });
        }
    }
}
