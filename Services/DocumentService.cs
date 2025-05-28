using Microsoft.EntityFrameworkCore;
using DocumentStorageSystem.Data;
using DocumentStorageSystem.Models;
using System.IO;

namespace DocumentStorageSystem.Services
{
    public class DocumentService
    {
        private readonly AppDbContext _context;

        public DocumentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> UploadDocumentAsync(int userId, string fileName, byte[] content)
        {
            var latestVersion = await _context.Documents
                .Where(d => d.Name.ToLower() == fileName.ToLower() && d.UserId == userId)
                .MaxAsync(d => (int?)d.Version) ?? -1;

            var document = new Document
            {
                Name = fileName,
                Content = content,
                Version = latestVersion + 1,
                UploadDate = DateTime.UtcNow,
                UserId = userId
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document.Version;
        }

        public async Task<byte[]> GetDocumentAsync(int userId, string fileName, int? revision = null)
        {
            var query = _context.Documents
                .Where(d => d.Name.ToLower() == fileName.ToLower() && d.UserId == userId);

            var document = revision.HasValue
                ? await query.FirstOrDefaultAsync(d => d.Version == revision)
                : await query.OrderByDescending(d => d.Version).FirstOrDefaultAsync();

            if (document == null)
                throw new FileNotFoundException("Document not found or access denied");

            return document.Content;
        }

        public async Task<List<Document>> GetUserDocumentsAsync(int userId)
        {
            return await _context.Documents
                .Where(d => d.UserId == userId)
                .GroupBy(d => d.Name.ToLower())
                .Select(g => g.OrderByDescending(d => d.Version).First())
                .ToListAsync();
        }
    }
}
