using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Domain.Enums;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.AnnouncementTests;

public class AnnouncementServiceTests
{
    private readonly Mock<IAnnouncementRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<AnnouncementService>> _loggerMock;
    private readonly AnnouncementService _service;

    public AnnouncementServiceTests()
    {
        _repoMock   = new Mock<IAnnouncementRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<AnnouncementService>>();
        _service    = new AnnouncementService(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAnnouncementAsync_WhenDtoIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.CreateAnnouncementAsync(null!, userId: 123));
    }

    [Fact]
    public async Task CreateAnnouncementAsync_WhenValidDto_AddsAndReturnsDto()
    {
        // Arrange
        var createDto = new AnnouncementCreateDto { Title = "T", Description = "D", Audience = TargetAudience.MobileUsers, Status = "draft" };
        var entity = new Announcement { Id = 77, Title = "T", Description = "D", Audience = TargetAudience.MobileUsers, Status = "draft" };
        var responseDto = new AnnouncementResponseDto { Id = 77, Title = "T", Description = "D", Audience = TargetAudience.MobileUsers, Status = "draft" };

        _mapperMock
            .Setup(m => m.Map<Announcement>(createDto))
            .Returns(entity);
        _repoMock
            .Setup(r => r.AddAsync(entity))
            .Returns(Task.CompletedTask);
        _mapperMock
            .Setup(m => m.Map<AnnouncementResponseDto>(entity))
            .Returns(responseDto);

        // Act
        var result = await _service.CreateAnnouncementAsync(createDto, userId: 42);

        // Assert
        result.Match(
            onSuccess: dto =>
            {
                dto.Should().BeEquivalentTo(responseDto);
                _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
                dto.Id.Should().Be(77);
                return true;
            },
            onError: err =>
            {
                false.Should().BeTrue($"Expected success but got error {err}");
                return false;
            });
    }

    [Fact]
    public async Task UpdateAnnouncementAsync_WhenNotFound_ReturnsError()
    {
        _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Announcement?)null);

        var updateDto = new AnnouncementUpdateDto { Title = "X" };
        var response = await _service.UpdateAnnouncementAsync(5, updateDto, userId: 1);

        response.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error but got success");
                return true;
            },
            onError: err =>
            {
                err.Should().Be("Announcement with ID 5 not found.");
                return false;
            });
    }

    [Fact]
    public async Task UpdateAnnouncementAsync_WhenFound_MapsUpdatesAndReturnsTrue()
    {
        var entity = new Announcement { Id = 9, Title = "Old", Description = "D", Audience = TargetAudience.Sponsors, Status = "draft" };
        _repoMock.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map(It.IsAny<AnnouncementUpdateDto>(), entity));
        _repoMock.Setup(r => r.UpdateAsync(entity)).Returns(Task.CompletedTask);

        var updateDto = new AnnouncementUpdateDto { Title = "New", Description = "D2", Audience = TargetAudience.Sponsors, Status = "published" };
        var response = await _service.UpdateAnnouncementAsync(9, updateDto, userId: 7);

        response.Match(
            onSuccess: ok =>
            {
                ok.Should().BeTrue();
                _repoMock.Verify(r => r.UpdateAsync(entity), Times.Once);
                return true;
            },
            onError: err =>
            {
                false.Should().BeTrue($"Expected success but got error {err}");
                return false;
            });
    }

    [Fact]
    public async Task DeleteAnnouncementAsync_WhenNotFound_ReturnsError()
    {
        _repoMock.Setup(r => r.GetByIdAsync(123)).ReturnsAsync((Announcement?)null);

        var resp = await _service.DeleteAnnouncementAsync(123, userId: 1);

        resp.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error but got success");
                return true;
            },
            onError: err =>
            {
                err.Should().Be("Announcement with ID 123 not found.");
                return false;
            });
    }

    [Fact]
    public async Task DeleteAnnouncementAsync_WhenFound_DeletesAndReturnsTrue()
    {
        var entity = new Announcement
        {
            Id = 10,
            Status = "draft",
        };
        _repoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(entity);
        _repoMock.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

        var resp = await _service.DeleteAnnouncementAsync(10, userId: 5);

        resp.Match(
            onSuccess: ok =>
            {
                ok.Should().BeTrue();
                _repoMock.Verify(r => r.DeleteAsync(entity), Times.Once);
                return true;
            },
            onError: err =>
            {
                false.Should().BeTrue($"Expected success but got error {err}");
                return false;
            });
    }

    [Fact]
    public async Task GetMyCategoryAnnouncementsAsync_ReturnsMappedList()
    {
        var items = new List<Announcement>
        {
            new Announcement
            {
                Id = 1,
                Status = "draft"
            },
            new Announcement
            {
                Id = 2,
                Status = "published"
            }
        };
        var dtos = items.Select(a => new AnnouncementResponseDto { Id = a.Id }).ToList();

        _repoMock.Setup(r => r.GetByCreatedByAsync(99)).ReturnsAsync(items);
        _mapperMock.Setup(m => m.Map<IEnumerable<AnnouncementResponseDto>>(items)).Returns(dtos);

        var result = await _service.GetMyCategoryAnnouncementsAsync(99);

        result.Match(
            onSuccess: list =>
            {
                list.Should().BeEquivalentTo(dtos);
                return true;
            },
            onError: err =>
            {
                false.Should().BeTrue($"Expected success but got error {err}");
                return false;
            });
    }

    [Fact]
    public async Task GetAllAnnouncementsAsync_ReturnsMappedList()
    {
        var items = new List<Announcement>
        {
            new Announcement
            {
                Id = 3,
                Status = "draft"
            },
            new Announcement
            {
                Id = 4,
                Status = "published"
            }
        };
        var dtos = items.Select(a => new AnnouncementResponseDto { Id = a.Id }).ToList();

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(items);
        _mapperMock.Setup(m => m.Map<IEnumerable<AnnouncementResponseDto>>(items)).Returns(dtos);

        var result = await _service.GetAllAnnouncementsAsync();

        result.Match(
            onSuccess: list =>
            {
                list.Should().BeEquivalentTo(dtos);
                return true;
            },
            onError: err =>
            {
                false.Should().BeTrue($"Expected success but got error {err}");
                return false;
            });
    }

    [Fact]
    public async Task GetPublishedForAudienceAsync_ReturnsMappedList()
    {
        var items = new List<Announcement>
        {
            new Announcement
            {
                Id = 5,
                Status = "published"
            },
            new Announcement
            {
                Id = 6,
                Status = "published"
            }
        };
        var dtos = items.Select(a => new AnnouncementResponseDto { Id = a.Id }).ToList();

        _repoMock
            .Setup(r => r.GetByAudienceAsync(TargetAudience.MobileUsers, "published"))
            .ReturnsAsync(items);
        _mapperMock
            .Setup(m => m.Map<IEnumerable<AnnouncementResponseDto>>(items))
            .Returns(dtos);

        var result = await _service.GetPublishedForAudienceAsync(TargetAudience.MobileUsers);

        result.Match(
            onSuccess: list =>
            {
                list.Should().BeEquivalentTo(dtos);
                return true;
            },
            onError: err =>
            {
                false.Should().BeTrue($"Expected success but got error {err}");
                return false;
            });
    }

    [Fact]
    public async Task GetAnnouncementsByCreatorIdAsync_ReturnsMappedList()
    {
        var items = new List<Announcement>
        {
            new Announcement
            {
                Id = 7,
                Status = "draft"
            },
            new Announcement
            {
                Id = 8,
                Status = "published"
            }
        };
        var dtos = items.Select(a => new AnnouncementResponseDto { Id = a.Id }).ToList();

        _repoMock.Setup(r => r.GetByCreatedByAsync(55)).ReturnsAsync(items);
        _mapperMock.Setup(m => m.Map<IEnumerable<AnnouncementResponseDto>>(items)).Returns(dtos);

        var result = await _service.GetAnnouncementsByCreatorIdAsync(55);

        result.Match(
            onSuccess: list =>
            {
                list.Should().BeEquivalentTo(dtos);
                return true;
            },
            onError: err =>
            {
                false.Should().BeTrue($"Expected success but got error {err}");
                return false;
            });
    }

    [Fact]
    public async Task GetAnnouncementByIdAsync_WhenNotFound_ReturnsError()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1234)).ReturnsAsync((Announcement?)null);

        var result = await _service.GetAnnouncementByIdAsync(1234);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error but got success");
                return true;
            },
            onError: err =>
            {
                err.Should().Be("Announcement with ID 1234 not found.");
                return false;
            });
    }

    [Fact]
    public async Task GetAnnouncementByIdAsync_WhenFound_ReturnsDto()
    {
        var entity = new Announcement
        {
            Id = 99,
            Title = "TT",
            Status = "draft",
        };
        var dto = new AnnouncementResponseDto { Id = 99, Title = "TT" };

        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<AnnouncementResponseDto>(entity)).Returns(dto);

        var result = await _service.GetAnnouncementByIdAsync(99);

        result.Match(
            onSuccess: returnedDto =>
            {
                returnedDto.Should().BeEquivalentTo(dto);
                return true;
            },
            onError: err =>
            {
                false.Should().BeTrue($"Expected success but got error {err}");
                return false;
            });
    }
}