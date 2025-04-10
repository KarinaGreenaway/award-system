using AutoMapper;
using AwardSystemAPI.Application.DTOs.AwardCategoryDtos;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.AwardCategoryTests;

public class AwardCategoryServiceTests
{
    private readonly Mock<IAwardCategoryRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<AwardCategoryService>> _loggerMock;
    private readonly AwardCategoryService _service;

    public AwardCategoryServiceTests()
    {
        _repositoryMock = new Mock<IAwardCategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<AwardCategoryService>>();
        _service = new AwardCategoryService(_repositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAwardCategoriesAsync_ReturnsMappedDtos()
    {
        var categories = new List<AwardCategory>
        {
            new AwardCategory { Id = 1, Name = "Cat1", Type="individual", SponsorId = 1, ProfileStatus = "draft", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new AwardCategory { Id = 2, Name = "Cat2", Type="team", SponsorId = 1, ProfileStatus = "published", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        var expectedDtos = categories.Select(c => new AwardCategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Type = c.Type,
            SponsorId = c.SponsorId,
            ProfileStatus = c.ProfileStatus,
            IntroductionVideo = null,
            IntroductionParagraph = null,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        _mapperMock.Setup(m => m.Map<IEnumerable<AwardCategoryResponseDto>>(categories))
            .Returns(expectedDtos);

        var result = await _service.GetAllAwardCategoriesAsync();

        result.Match(
            onSuccess: dtos =>
            {
                dtos.Should().BeEquivalentTo(expectedDtos);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected success but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task GetAwardCategoryByIdAsync_WhenCategoryExists_ReturnsMappedDto()
    {
        const int id = 1;
        var category = new AwardCategory
        {
            Id = id,
            Name = "Cat1",
            Type = "individual",
            SponsorId = 1,
            ProfileStatus = "draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        var expectedDto = new AwardCategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,
            SponsorId = category.SponsorId,
            ProfileStatus = category.ProfileStatus,
            IntroductionVideo = null,
            IntroductionParagraph = null,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };

        _mapperMock.Setup(m => m.Map<AwardCategoryResponseDto>(category)).Returns(expectedDto);

        var result = await _service.GetAwardCategoryByIdAsync(id);

        result.Match(
            onSuccess: dto =>
            {
                dto.Should().BeEquivalentTo(expectedDto);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected valid AwardCategoryResponseDto but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task GetAwardCategoryByIdAsync_WhenCategoryDoesNotExist_ReturnsErrorString()
    {
        const int id = 1;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as AwardCategory);

        var result = await _service.GetAwardCategoryByIdAsync(id);

        result.Match(
            onSuccess: dto =>
            {
                false.Should().BeTrue("Expected error message for non-existent category.");
                return true;
            },
            onError: error =>
            {
                error.Should().Be($"AwardCategory with ID {id} not found.");
                return false;
            });
    }

    [Fact]
    public async Task CreateAwardCategoryAsync_WhenValidDtoProvided_ReturnsMappedDtoAndAddsEntity()
    {
        var createDto = new AwardCategoryCreateDto
        {
            Name = "New Category",
            Type = "team",
            SponsorId = 1,
            ProfileStatus = "draft",
            IntroductionVideo = "video.mp4",
            IntroductionParagraph = "Intro text"
        };

        var mappedEntity = new AwardCategory
        {
            Id = 10, // assume repository sets an ID
            Name = createDto.Name,
            Type = createDto.Type,
            SponsorId = createDto.SponsorId,
            ProfileStatus = createDto.ProfileStatus,
            IntroductionVideo = createDto.IntroductionVideo,
            IntroductionParagraph = createDto.IntroductionParagraph,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mapperMock.Setup(m => m.Map<AwardCategory>(createDto)).Returns(mappedEntity);

        var expectedResponseDto = new AwardCategoryResponseDto
        {
            Id = mappedEntity.Id,
            Name = mappedEntity.Name,
            Type = mappedEntity.Type,
            SponsorId = mappedEntity.SponsorId,
            ProfileStatus = mappedEntity.ProfileStatus,
            IntroductionVideo = mappedEntity.IntroductionVideo,
            IntroductionParagraph = mappedEntity.IntroductionParagraph,
            CreatedAt = mappedEntity.CreatedAt,
            UpdatedAt = mappedEntity.UpdatedAt
        };

        _mapperMock.Setup(m => m.Map<AwardCategoryResponseDto>(mappedEntity)).Returns(expectedResponseDto);
        _repositoryMock.Setup(r => r.AddAsync(mappedEntity)).Returns(Task.CompletedTask);

        var result = await _service.CreateAwardCategoryAsync(createDto);

        result.Match(
            onSuccess: dto =>
            {
                _repositoryMock.Verify(r => r.AddAsync(mappedEntity), Times.Once);
                dto.Should().BeEquivalentTo(expectedResponseDto);
                return dto;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected AwardCategoryResponseDto but got error: " + error);
                return new AwardCategoryResponseDto();
            });
    }

    [Fact]
    public async Task UpdateAwardCategoryAsync_WhenCategoryExists_ReturnsTrueAndUpdatesEntity()
    {
        const int id = 1;
        var category = new AwardCategory
        {
            Id = id,
            Name = "Old Name",
            Type = "individual",
            SponsorId = 1,
            ProfileStatus = "draft",
            IntroductionVideo = "oldvideo.mp4",
            IntroductionParagraph = "Old intro",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        var updateDto = new AwardCategoryUpdateDto
        {
            Name = "Updated Name",
            Type = "team",
            SponsorId = 1,  // usually sponsor id remains unchanged
            ProfileStatus = "published",
            IntroductionVideo = "newvideo.mp4",
            IntroductionParagraph = "New intro"
        };

        _mapperMock.Setup(m => m.Map(updateDto, category))
            .Callback<AwardCategoryUpdateDto, AwardCategory>((dto, cat) =>
            {
                cat.Name = dto.Name;
                cat.Type = dto.Type;
                cat.ProfileStatus = dto.ProfileStatus;
                cat.IntroductionVideo = dto.IntroductionVideo;
                cat.IntroductionParagraph = dto.IntroductionParagraph;
            });

        _repositoryMock.Setup(r => r.UpdateAsync(category)).Returns(Task.CompletedTask);

        var result = await _service.UpdateAwardCategoryAsync(id, updateDto);

        result.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                category.Name.Should().Be(updateDto.Name);
                category.Type.Should().Be(updateDto.Type);
                category.ProfileStatus.Should().Be(updateDto.ProfileStatus);
                category.IntroductionVideo.Should().Be(updateDto.IntroductionVideo);
                category.IntroductionParagraph.Should().Be(updateDto.IntroductionParagraph);
                _repositoryMock.Verify(r => r.UpdateAsync(category), Times.Once);
                return success;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected successful update but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task UpdateAwardCategoryAsync_WhenCategoryDoesNotExist_ReturnsErrorString()
    {
        const int id = 999;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AwardCategory)null);

        var updateDto = new AwardCategoryUpdateDto
        {
            Name = "Updated Name",
            Type = "individual",
            SponsorId = 1,
            ProfileStatus = "published",
            IntroductionVideo = "newvideo.mp4",
            IntroductionParagraph = "New intro"
        };

        var result = await _service.UpdateAwardCategoryAsync(id, updateDto);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error message for updating a non-existent AwardCategory.");
                return true;
            },
            onError: error =>
            {
                error.Should().Be($"AwardCategory with ID {id} not found for update.");
                return false;
            });
    }

    [Fact]
    public async Task DeleteAwardCategoryAsync_WhenCategoryExists_ReturnsTrue()
    {
        const int id = 1;
        var category = new AwardCategory
        {
            Id = id,
            Name = "Category",
            Type = "team",
            SponsorId = 1,
            ProfileStatus = "draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);
        _repositoryMock.Setup(r => r.DeleteAsync(category)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAwardCategoryAsync(id);

        result.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                _repositoryMock.Verify(r => r.DeleteAsync(category), Times.Once);
                return success;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected successful deletion but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task DeleteAwardCategoryAsync_WhenCategoryDoesNotExist_ReturnsErrorString()
    {
        const int id = 1;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as AwardCategory);

        var result = await _service.DeleteAwardCategoryAsync(id);

        result.Match(
            onSuccess: success =>
            {
                false.Should().BeTrue("Expected error message for deleting non-existent AwardCategory.");
                return success;
            },
            onError: error =>
            {
                error.Should().Be($"AwardCategory with ID {id} not found for deletion.");
                return false;
            });
    }

    [Fact]
    public async Task GetAwardCategoriesBySponsorIdAsync_ReturnsMappedDtos()
    {
        const int sponsorId = 1;
        var categories = new List<AwardCategory>
        {
            new AwardCategory { Id = 1, Name = "Category1", Type = "individual", SponsorId = sponsorId, ProfileStatus = "draft", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new AwardCategory { Id = 2, Name = "Category2", Type = "team", SponsorId = sponsorId, ProfileStatus = "published", IntroductionVideo = "video.mp4", IntroductionParagraph = "Intro", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _repositoryMock.Setup(r => r.GetBySponsorIdAsync(sponsorId)).ReturnsAsync(categories);

        var expectedDtos = categories.Select(c => new AwardCategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Type = c.Type,
            SponsorId = c.SponsorId,
            ProfileStatus = c.ProfileStatus,
            IntroductionVideo = c.IntroductionVideo,
            IntroductionParagraph = c.IntroductionParagraph,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        _mapperMock.Setup(m => m.Map<IEnumerable<AwardCategoryResponseDto>>(categories)).Returns(expectedDtos);

        var result = await _service.GetAwardCategoriesBySponsorIdAsync(sponsorId);

        result.Match(
            onSuccess: dtos =>
            {
                dtos.Should().BeEquivalentTo(expectedDtos);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected success but got error: " + error);
                return false;
            });
    }
}