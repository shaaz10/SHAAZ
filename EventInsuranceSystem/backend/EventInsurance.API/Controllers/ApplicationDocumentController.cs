using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventInsurance.API.Controllers
{
    public class UploadDocumentRequestForm
    {
        public string DocumentType { get; set; } = string.Empty;
        public IFormFile? File { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationDocumentController : ControllerBase
    {
        private readonly IApplicationDocumentService _service;
        private readonly IAIDocumentValidationService _validationService;

        public ApplicationDocumentController(
            IApplicationDocumentService service,
            IAIDocumentValidationService validationService)
        {
            _service = service;
            _validationService = validationService;
        }

        /// <summary>
        /// Handles the secure uploading of documents for a specific policy application.
        /// Saves the file locally, updates database records, and triggers AI validation automatically.
        /// </summary>
        // ==========================================
        // FLOW 1: FROM REQUEST TO ACTIVE POLICY
        // Step 5: Document Upload & AI Validation
        // The customer uploads required documents (ID, Event Permits), which are processed and AI-validated for authenticity.
        // ==========================================
        [Authorize(Roles = "Customer")]
        [HttpPost("upload/{applicationId}")]
        public async Task<IActionResult> UploadDocument(
            int applicationId,
            [FromForm] UploadDocumentRequestForm request)
        {
            var file = request.File;
            var documentType = request.DocumentType;

            // Validate that a file was actually provided
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            // Ensure the local uploads directory exists
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            
            // Generate a unique filename to prevent collisions and save the file
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Create a DTO with file details and pass to the service layer for database persistence
            var dto = new UploadDocumentDto 
            {
                FileName = file.FileName,
                FilePath = "/uploads/" + fileName,
                DocumentType = documentType 
            };

            var documentId = await _service.UploadDocumentAsync(applicationId, dto);

            // Automatically trigger AI validation after upload to process the newly added document
            var validationResult = await _validationService.ValidateAndScoreAsync(applicationId);

            // Return the new document's ID and the results of the immediate AI validation
            return Ok(new { 
                DocumentId = documentId, 
                ValidationResult = validationResult 
            });
        }

        /// <summary>
        /// Retrieves a list of all documents uploaded for a given policy application.
        /// Primarily used by customers to see their upload history and status.
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetDocuments(
            int applicationId)
        {
            // Queries the document service for all files associated with the applicationId
            var documents = await _service
                .GetApplicationDocumentsAsync(applicationId);

            // Returns the collection of document metadata
            return Ok(documents);
        }
    }
}
