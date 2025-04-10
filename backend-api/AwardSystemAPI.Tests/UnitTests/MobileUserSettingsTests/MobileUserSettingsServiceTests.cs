using AutoMapper;
using AwardSystemAPI.Application.DTOs.MobileUserSettingsDtos;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.MobileUserSettingsTests;

public class MobileUserSettingsServiceTests
{
    private readonly Mock<IGenericRepository<MobileUserSettings>> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<MobileUserSettingsService>> _loggerMock;
    private readonly MobileUserSettingsService _service;

    public MobileUserSettingsServiceTests()
    {
        _repositoryMock = new Mock<IGenericRepository<MobileUserSettings>>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<MobileUserSettingsService>>();
        _service = new MobileUserSettingsService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
    }


    [Fact]
    public async Task GetSettingsAsync_WhenSettingsExist_ReturnsMappedDto()
    {
        const int userId = 1;
        var settings = new MobileUserSettings
        {
            Id = 10,
            UserId = userId,
            PushNotifications = false,
            AiFunctionality = true
        };

        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<MobileUserSettings> { settings });

        var expectedDto = new MobileUserSettingsResponseDto
        {
            Id = settings.Id,
            UserId = settings.UserId,
            PushNotifications = settings.PushNotifications,
            AiFunctionality = settings.AiFunctionality
        };

        _mapperMock.Setup(m => m.Map<MobileUserSettingsResponseDto>(settings))
            .Returns(expectedDto);

        var response = await _service.GetSettingsAsync(userId);

        response.Match(
            onSuccess: dto =>
            {
                dto.Should().BeEquivalentTo(expectedDto);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected settings but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task GetSettingsAsync_WhenSettingsDoNotExist_ReturnsErrorString()
    {
        const int userId = 1;
        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<MobileUserSettings>());

        var response = await _service.GetSettingsAsync(userId);

        response.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error message because settings do not exist.");
                return true;
            },
            onError: error =>
            {
                error.Should().Be($"Settings not found for user with ID {userId}.");
                return false;
            });
    }


    [Fact]
    public async Task UpdateSettingsAsync_WhenSettingsExist_ReturnsUpdatedMappedDto()
    {
        const int userId = 1;
        var settings = new MobileUserSettings
        {
            Id = 10,
            UserId = userId,
            PushNotifications = true,
            AiFunctionality = false
        };

        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<MobileUserSettings> { settings });

        var updateDto = new MobileUserSettingsUpdateDto
        {
            PushNotifications = false,
            AiFunctionality = true
        };

        settings.PushNotifications = updateDto.PushNotifications;
        settings.AiFunctionality = updateDto.AiFunctionality;
        _repositoryMock.Setup(r => r.UpdateAsync(settings)).Returns(Task.CompletedTask);

        var expectedResponseDto = new MobileUserSettingsResponseDto
        {
            Id = settings.Id,
            UserId = settings.UserId,
            PushNotifications = updateDto.PushNotifications,
            AiFunctionality = updateDto.AiFunctionality
        };

        _mapperMock.Setup(m => m.Map<MobileUserSettingsResponseDto>(settings))
            .Returns(expectedResponseDto);

        var response = await _service.UpdateSettingsAsync(userId, updateDto);

        response.Match(
            onSuccess: dto =>
            {
                dto.Should().BeEquivalentTo(expectedResponseDto);
                _repositoryMock.Verify(r => r.UpdateAsync(settings), Times.Once);
                return dto;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected update success but got error: " + error);
                return new MobileUserSettingsResponseDto();
            });
    }

    [Fact]
    public async Task UpdateSettingsAsync_WhenSettingsDoNotExist_ReturnsErrorString()
    {
        const int userId = 1;
        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<MobileUserSettings>());

        var updateDto = new MobileUserSettingsUpdateDto
        {
            PushNotifications = false,
            AiFunctionality = true
        };

        var response = await _service.UpdateSettingsAsync(userId, updateDto);

        response.Match<MobileUserSettingsResponseDto>(
            onSuccess: dto =>
            {
                false.Should().BeTrue("Expected error message because settings do not exist.");
                return dto;
            },
            onError: error =>
            {
                error.Should().Be($"Settings not found for user with ID {userId}.");
                return new MobileUserSettingsResponseDto();
            });
    }
}