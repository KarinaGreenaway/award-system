using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.AwardCategoryTests;

public class AwardCategoryRepositoryTests : IDisposable
{
    private AppDbContext _context;
    private AwardCategoryRepository _repository;
    private readonly ILogger<AwardCategoryRepository> _logger;

    public AwardCategoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _logger = new LoggerFactory().CreateLogger<AwardCategoryRepository>();
        _repository = new AwardCategoryRepository(_context, _logger);
    }

    public void Dispose()
    {
        try
        {
            _context?.Database.EnsureDeleted();
        }
        catch (ObjectDisposedException) { }
        _context?.Dispose();
        _context = null;
    }

    [Fact]
    public async Task GetBySponsorIdAsync_WithValidSponsorId_ShouldReturnMatchingCategories()
    {
        var category1 = new AwardCategory
        {
            Name = "Cat1",
            Type = "individual",
            SponsorId = 1,
            ProfileStatus = "published",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var category2 = new AwardCategory
        {
            Name = "Cat2",
            Type = "team",
            SponsorId = 1,
            ProfileStatus = "draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var category3 = new AwardCategory
        {
            Name = "Cat3",
            Type = "individual",
            SponsorId = 2,
            ProfileStatus = "published",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.AwardCategories.AddRange(category1, category2, category3);
        await _context.SaveChangesAsync();

        IEnumerable<AwardCategory> results = await _repository.GetBySponsorIdAsync(1);

        results = results.ToList();
        results.Should().HaveCount(2);
        results.All(c => c.SponsorId == 1).Should().BeTrue();
    }

    [Fact]
    public async Task GetBySponsorIdAsync_WithNonExistingSponsorId_ShouldReturnEmptyList()
    {
        var category1 = new AwardCategory
        {
            Name = "Cat1",
            Type = "individual",
            SponsorId = 1,
            ProfileStatus = "published",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.AwardCategories.Add(category1);
        await _context.SaveChangesAsync();

        IEnumerable<AwardCategory> results = await _repository.GetBySponsorIdAsync(99);

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBySponsorIdAsync_WhenContextDisposed_ShouldThrowCustomException()
    {
        var category = new AwardCategory
        {
            Name = "Test Category",
            Type = "team",
            SponsorId = 1,
            ProfileStatus = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.AwardCategories.Add(category);
        await _context.SaveChangesAsync();

        // Dispose the context to simulate an error.
        _context.Dispose();

        Func<Task> act = async () => await _repository.GetBySponsorIdAsync(1);

        var exception = await act.Should().ThrowAsync<Exception>();
        exception.Which.Message.Should().Be("An error occurred while retrieving AwardCategory records by sponsor id.");
    }
}