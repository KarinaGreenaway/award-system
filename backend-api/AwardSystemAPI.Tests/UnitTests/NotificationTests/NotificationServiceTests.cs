using AutoMapper;
using AwardSystemAPI.Application.DTOs.NotificationDtos;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.NotificationTests;

public class NotificationServiceTests
{
    private readonly Mock<IGenericRepository<Notification>> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<NotificationService>> _loggerMock;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _repositoryMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<NotificationService>>();
        _service = new NotificationService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
    }


    [Fact]
    public async Task GetNotificationsAsync_ReturnsMappedDtos()
    {
        var notifications = new List<Notification>
        {
            new Notification { Id = 1, UserId = 1, Title = "Title 1", Description = "Desc 1", Read = false, CreatedAt = DateTime.UtcNow},
            new Notification { Id = 2, UserId = 1, Title = "Title 2", Description = "Desc 2", Read = true, CreatedAt = DateTime.UtcNow},
            new Notification { Id = 3, UserId = 2, Title = "Title 3", Description = "Desc 3", Read = false, CreatedAt = DateTime.UtcNow}
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(notifications);

        var expectedDtos = notifications
            .Where(n => n.UserId == 1)
            .Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Description = n.Description,
                Read = n.Read,
                CreatedAt = n.CreatedAt
            }).ToArray();

        _mapperMock.Setup(m => m.Map<IEnumerable<NotificationResponseDto>>(It.Is<IEnumerable<Notification>>(n => n.All(x => x.UserId == 1))))
            .Returns(expectedDtos);

        var response = await _service.GetNotificationsAsync(1);

        response.Match(
            onSuccess: dtos =>
            {
                dtos.Should().BeEquivalentTo(expectedDtos);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected success, but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task GetNotificationByIdAsync_WhenNotificationExists_ReturnsMappedDto()
    {
        const int id = 1;
        var notification = new Notification
        {
            Id = id,
            UserId = 1,
            Title = "Title 1",
            Description = "Desc 1",
            Read = false,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(notification);

        var expectedDto = new NotificationResponseDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Description = notification.Description,
            Read = notification.Read,
            CreatedAt = notification.CreatedAt
        };

        _mapperMock.Setup(m => m.Map<NotificationResponseDto>(notification))
            .Returns(expectedDto);

        var response = await _service.GetNotificationByIdAsync(id);

        response.Match(
            onSuccess: dto =>
            {
                dto.Should().BeEquivalentTo(expectedDto);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected valid NotificationResponseDto but received error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task GetNotificationByIdAsync_WhenNotificationDoesNotExist_ReturnsErrorString()
    {
        const int id = 1;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Notification)null);

        var response = await _service.GetNotificationByIdAsync(id);

        response.Match(
            onSuccess: dto =>
            {
                false.Should().BeTrue("Expected error message for non-existent notification.");
                return true;
            },
            onError: error =>
            {
                error.Should().Be($"Notification with ID {id} not found.");
                return false;
            });
    }

    [Fact]
    public async Task CreateNotificationAsync_WhenDtoIsNull_ThrowsArgumentNullException()
    {
        Func<Task> act = async () => await _service.CreateNotificationAsync(null);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CreateNotificationAsync_WhenValidDtoProvided_ReturnsMappedDtoAndAddsEntity()
    {
        var createDto = new NotificationCreateDto
        {
            Title = "New Notification",
            Description = "New Desc"
        };

        var mappedEntity = new Notification
        {
            Id = 10,
            Title = createDto.Title,
            Description = createDto.Description,
            // For testing, simulate that the repository expects UserId to already be set.
            UserId = 1,
            Read = false,
            CreatedAt = DateTime.UtcNow
        };

        _mapperMock.Setup(m => m.Map<Notification>(createDto)).Returns(mappedEntity);

        var expectedResponseDto = new NotificationResponseDto
        {
            Id = mappedEntity.Id,
            UserId = mappedEntity.UserId,
            Title = mappedEntity.Title,
            Description = mappedEntity.Description,
            Read = mappedEntity.Read,
            CreatedAt = mappedEntity.CreatedAt
        };

        _mapperMock.Setup(m => m.Map<NotificationResponseDto>(mappedEntity)).Returns(expectedResponseDto);
        _repositoryMock.Setup(r => r.AddAsync(mappedEntity)).Returns(Task.CompletedTask);

        var response = await _service.CreateNotificationAsync(createDto);

        response.Match(
            onSuccess: dto =>
            {
                _repositoryMock.Verify(r => r.AddAsync(mappedEntity), Times.Once);
                dto.Should().BeEquivalentTo(expectedResponseDto);
                return dto;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected NotificationResponseDto but received error: " + error);
                return new NotificationResponseDto();
            });
    }


    [Fact]
    public async Task MarkAsReadAsync_WhenNotificationExistsAndNotRead_UpdatesAndReturnsTrue()
    {
        const int id = 1;
        var notification = new Notification
        {
            Id = id,
            UserId = 1,
            Title = "Title",
            Description = "Desc",
            Read = false,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(notification);
        _repositoryMock.Setup(r => r.UpdateAsync(notification)).Returns(Task.CompletedTask);

        var response = await _service.MarkAsReadAsync(id);

        response.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                notification.Read.Should().BeTrue();
                _repositoryMock.Verify(r => r.UpdateAsync(notification), Times.Once);
                return success;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected successful update but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task MarkAsReadAsync_WhenNotificationAlreadyRead_ReturnsTrueWithoutUpdating()
    {
        const int id = 1;
        var notification = new Notification
        {
            Id = id,
            UserId = 1,
            Title = "Title",
            Description = "Desc",
            Read = true,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(notification);
        // UpdateAsync should not be called if the notification is already read.
        _repositoryMock.Setup(r => r.UpdateAsync(notification)).Returns(Task.CompletedTask);

        var response = await _service.MarkAsReadAsync(id);

        response.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Notification>()), Times.Never);
                return success;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected success but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task MarkAsReadAsync_WhenNotificationDoesNotExist_ReturnsErrorString()
    {
        const int id = 1;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Notification)null);

        var response = await _service.MarkAsReadAsync(id);

        response.Match(
            onSuccess: success =>
            {
                false.Should().BeTrue("Expected error message for non-existent notification.");
                return success;
            },
            onError: error =>
            {
                error.Should().Be($"Notification with ID {id} not found.");
                return false;
            });
    }

}