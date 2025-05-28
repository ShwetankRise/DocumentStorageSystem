using Microsoft.EntityFrameworkCore;
using DocumentStorageSystem.Data;
using DocumentStorageSystem.Services;
using DocumentStorageSystem.Models;
using Xunit;
using System.Threading.Tasks;
using System.Linq;

namespace DocumentStorageSystem.Tests
{
    public class DocumentServiceTests
    {
        private readonly AppDbContext _context;
        private readonly DocumentService _service;

        public DocumentServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);
            _service = new DocumentService(_context);
        }

        [Fact]
        public async Task UploadDocumentAsync_StoresNewDocument()
        {
            // Arrange
            var content = new byte[] { 1, 2, 3 };
            var fileName = "test.pdf";
            var userId = 1;

            // Act
            var version = await _service.UploadDocumentAsync(userId, fileName, content);

            // Assert
            var document = await _context.Documents.FirstOrDefaultAsync(d => d.Name == fileName && d.UserId == userId);
            Assert.NotNull(document);
            Assert.Equal(0, version);
            Assert.Equal(content, document.Content);
            Assert.Equal(userId, document.UserId);
        }

        [Fact]
        public async Task UploadDocumentAsync_IncrementsVersion()
        {
            // Arrange
            var content1 = new byte[] { 1, 2, 3 };
            var content2 = new byte[] { 4, 5, 6 };
            var fileName = "test.pdf";
            var userId = 1;

            // Act
            await _service.UploadDocumentAsync(userId, fileName, content1);
            var version = await _service.UploadDocumentAsync(userId, fileName, content2);

            // Assert
            var documents = await _context.Documents.Where(d => d.Name == fileName && d.UserId == userId).ToListAsync();
            Assert.Equal(2, documents.Count);
            Assert.Equal(1, version);
        }

        [Fact]
        public async Task GetDocumentAsync_ReturnsLatestVersion()
        {
            // Arrange
            var content1 = new byte[] { 1, 2, 3 };
            var content2 = new byte[] { 4, 5, 6 };
            var fileName = "test.pdf";
            var userId = 1;
            await _service.UploadDocumentAsync(userId, fileName, content1);
            await _service.UploadDocumentAsync(userId, fileName, content2);

            // Act
            var result = await _service.GetDocumentAsync(userId, fileName, null);

            // Assert
            Assert.Equal(content2, result);
        }

        [Fact]
        public async Task GetDocumentAsync_ReturnsSpecificRevision()
        {
            // Arrange
            var content1 = new byte[] { 1, 2, 3 };
            var content2 = new byte[] { 4, 5, 6 };
            var fileName = "test.pdf";
            var userId = 1;
            await _service.UploadDocumentAsync(userId, fileName, content1);
            await _service.UploadDocumentAsync(userId, fileName, content2);

            // Act
            var result = await _service.GetDocumentAsync(userId, fileName, 0);

            // Assert
            Assert.Equal(content1, result);
        }

        [Fact]
        public async Task GetDocumentAsync_UnauthorizedAccess_ThrowsException()
        {
            // Arrange
            var content = new byte[] { 1, 2, 3 };
            var fileName = "test.pdf";
            var userId = 1;
            await _service.UploadDocumentAsync(userId, fileName, content);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetDocumentAsync(2, fileName, null));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}