using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.RepositoryTests
{
    public class GenericRepositoryTests : IDisposable
    {
        private AppDbContext _context;
        private GenericRepository<AwardProcess> _repository;
        private readonly ILogger<GenericRepository<AwardProcess>> _logger;

        public GenericRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _logger = new LoggerFactory().CreateLogger<GenericRepository<AwardProcess>>();
            _repository = new GenericRepository<AwardProcess>(_context, _logger);
        }

        public void Dispose()
        {
            try
            {
                _context?.Database.EnsureDeleted();
            }
            catch (ObjectDisposedException)
            {
                // Already disposed so ignore.
            }
            _context?.Dispose();
            _context = null;
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            var process1 = new AwardProcess
            {
                AwardsName = "Award 1",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var process2 = new AwardProcess
            {
                AwardsName = "Award 2",
                StartDate = DateTime.UtcNow,
                Status = "completed",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.AwardProcesses.AddRange(process1, process2);
            await _context.SaveChangesAsync();

            IEnumerable<AwardProcess> results = await _repository.GetAllAsync();

            results.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnEntity()
        {
            var process = new AwardProcess
            {
                AwardsName = "Award 1",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.AwardProcesses.Add(process);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(process.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(process.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldThrowArgumentException()
        {
            const int invalidId = 0;

            Func<Task> act = async () => await _repository.GetByIdAsync(invalidId);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("ID must be greater than zero.*");
        }

        [Fact]
        public async Task AddAsync_WithValidEntity_ShouldAddEntity()
        {
            var process = new AwardProcess
            {
                AwardsName = "New Award",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(process);
            var allEntities = await _repository.GetAllAsync();

            allEntities.Should().ContainSingle(x => x.AwardsName == "New Award");
        }

        [Fact]
        public async Task AddAsync_WithNullEntity_ShouldThrowArgumentNullException()
        {
            Func<Task> act = async () => await _repository.AddAsync(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateAsync_WithValidEntity_ShouldUpdateEntity()
        {
            var process = new AwardProcess
            {
                AwardsName = "Initial Award",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.AwardProcesses.Add(process);
            await _context.SaveChangesAsync();

            process.AwardsName = "Updated Award";
            await _repository.UpdateAsync(process);
            var updated = await _repository.GetByIdAsync(process.Id);

            updated?.AwardsName.Should().Be("Updated Award");
        }

        [Fact]
        public async Task UpdateAsync_WithNullEntity_ShouldThrowArgumentNullException()
        {
            Func<Task> act = async () => await _repository.UpdateAsync(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task DeleteAsync_WithValidEntity_ShouldDeleteEntity()
        {
            var process = new AwardProcess
            {
                AwardsName = "To Delete",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.AwardProcesses.Add(process);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(process);
            var allEntities = await _repository.GetAllAsync();

            allEntities.Should().NotContain(x => x.Id == process.Id);
        }

        [Fact]
        public async Task DeleteAsync_WithNullEntity_ShouldThrowArgumentNullException()
        {
            Func<Task> act = async () => await _repository.DeleteAsync(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task SaveAsync_ShouldPersistChanges()
        {
            var process = new AwardProcess
            {
                AwardsName = "Persistent Award",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.AwardProcesses.Add(process);

            await _repository.SaveAsync();
            var retrieved = await _repository.GetByIdAsync(process.Id);

            retrieved.Should().NotBeNull();
            retrieved.AwardsName.Should().Be("Persistent Award");
        }

        [Fact]
        public async Task GetAllAsync_WhenContextDisposed_ShouldThrowCustomException()
        {
            _context.AwardProcesses.Add(new AwardProcess
            {
                AwardsName = "Test Award",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // Dispose the context to force an exception.
            _context.Dispose();

            Func<Task> act = async () => await _repository.GetAllAsync();

            var exception = await act.Should().ThrowAsync<Exception>();
            exception.Which.Message.Should().Be("An error occurred while retrieving all entities.");
        }

        [Fact]
        public async Task GetByIdAsync_WhenContextDisposed_ShouldThrowCustomException()
        {
            var process = new AwardProcess
            {
                AwardsName = "Test Award",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.AwardProcesses.Add(process);
            await _context.SaveChangesAsync();

            _context.Dispose();

            Func<Task> act = async () => await _repository.GetByIdAsync(process.Id);

            var exception = await act.Should().ThrowAsync<Exception>();
            exception.Which.Message.Should().Be($"An error occurred while retrieving the entity with ID {process.Id}.");
        }

        [Fact]
        public async Task AddAsync_WhenContextDisposed_ShouldThrowCustomException()
        {
            var process = new AwardProcess
            {
                AwardsName = "Test Add",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Dispose();

            Func<Task> act = async () => await _repository.AddAsync(process);

            var exception = await act.Should().ThrowAsync<Exception>();
            exception.Which.Message.Should().Be("An error occurred while adding the entity.");
        }

        [Fact]
        public async Task UpdateAsync_WhenContextDisposed_ShouldThrowCustomException()
        {
            var process = new AwardProcess
            {
                AwardsName = "Test Update",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.AwardProcesses.Add(process);
            await _context.SaveChangesAsync();

            _context.Dispose();

            process.AwardsName = "Updated Name";
            Func<Task> act = async () => await _repository.UpdateAsync(process);

            var exception = await act.Should().ThrowAsync<Exception>();
            exception.Which.Message.Should().Be("An error occurred while updating the entity.");
        }

        [Fact]
        public async Task DeleteAsync_WhenContextDisposed_ShouldThrowCustomException()
        {
            var process = new AwardProcess
            {
                AwardsName = "Test Delete",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.AwardProcesses.Add(process);
            await _context.SaveChangesAsync();

            _context.Dispose();

            Func<Task> act = async () => await _repository.DeleteAsync(process);

            var exception = await act.Should().ThrowAsync<Exception>();
            exception.Which.Message.Should().Be("An error occurred while deleting the entity.");
        }

        [Fact]
        public async Task SaveAsync_WhenContextDisposed_ShouldThrowCustomException()
        {
            var process = new AwardProcess
            {
                AwardsName = "Test Save",
                StartDate = DateTime.UtcNow,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.AwardProcesses.Add(process);

            _context.Dispose();

            Func<Task> act = async () => await _repository.SaveAsync();

            var exception = await act.Should().ThrowAsync<Exception>();
            exception.Which.Message.Should().Be("An error occurred while saving changes to the database.");
        }
    }
}
