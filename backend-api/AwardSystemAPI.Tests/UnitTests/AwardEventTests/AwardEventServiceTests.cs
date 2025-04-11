using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.AwardEventTests;

public class AwardEventServiceTests
{
    private readonly Mock<IGenericRepository<AwardEvent>> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<AwardEventService>> _loggerMock;
    private readonly IAwardEventService _service;

    public AwardEventServiceTests()
    {
        _repositoryMock = new Mock<IGenericRepository<AwardEvent>>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<AwardEventService>>();
        _service = new AwardEventService(_repositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAwardEventsAsync_WhenEventsExist_ShouldReturnMappedDtos()
    {
        var events = new List<AwardEvent>
        {
            new AwardEvent { Id = 1, Name = "Event 1", Location = "Location 1", EventDateTime = DateTime.UtcNow.AddDays(2), Description = "Desc 1", Directions = "Directions 1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new AwardEvent { Id = 2, Name = "Event 2", Location = "Location 2", EventDateTime = DateTime.UtcNow.AddDays(3), Description = "Desc 2", Directions = "Directions 2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

        var expectedDtos = events.Select(e => new AwardEventResponseDto
        {
            Id = e.Id,
            Name = e.Name,
            Location = e.Location,
            EventDateTime = e.EventDateTime,
            Description = e.Description,
            Directions = e.Directions,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        }).ToArray();

        _mapperMock.Setup(m => m.Map<IEnumerable<AwardEventResponseDto>>(events)).Returns(expectedDtos);

        var response = await _service.GetAllAwardEventsAsync();

        response.Match(
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
    public async Task GetAwardEventByIdAsync_WhenEventExists_ShouldReturnMappedDto()
    {
        const int id = 1;
        var awardEvent = new AwardEvent
        {
            Id = id,
            Name = "Event 1",
            Location = "Location 1",
            EventDateTime = DateTime.UtcNow.AddDays(2),
            Description = "Desc 1",
            Directions = "Directions 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(awardEvent);

        var expectedDto = new AwardEventResponseDto
        {
            Id = awardEvent.Id,
            Name = awardEvent.Name,
            Location = awardEvent.Location,
            EventDateTime = awardEvent.EventDateTime,
            Description = awardEvent.Description,
            Directions = awardEvent.Directions,
            CreatedAt = awardEvent.CreatedAt,
            UpdatedAt = awardEvent.UpdatedAt
        };

        _mapperMock.Setup(m => m.Map<AwardEventResponseDto>(awardEvent)).Returns(expectedDto);

        var response = await _service.GetAwardEventByIdAsync(id);

        response.Match(
            onSuccess: dto =>
            {
                dto.Should().BeEquivalentTo(expectedDto);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected a valid AwardEventResponseDto but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task GetAwardEventByIdAsync_WhenEventDoesNotExist_ShouldReturnErrorString()
    {
        const int id = 1;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as AwardEvent);

        var response = await _service.GetAwardEventByIdAsync(id);

        response.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error message for non-existent AwardEvent.");
                return true;
            },
            onError: error =>
            {
                error.Should().Be($"AwardEvent with ID {id} not found.");
                return false;
            });
    }


    [Fact]
    public async Task CreateAwardEventAsync_WithNullDto_ThrowsArgumentNullException()
    {
        Func<Task> act = async () => await _service.CreateAwardEventAsync(null);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CreateAwardEventAsync_WithValidDto_ReturnsMappedDtoAndAddsEntity()
    {
        var createDto = new AwardEventCreateDto
        {
            Name = "Event 1",
            Location = "Location 1",
            EventDateTime = DateTime.UtcNow.AddDays(2),
            Description = "Desc 1",
            Directions = "Directions 1"
        };

        var mappedEntity = new AwardEvent
        {
            Id = 10,
            Name = createDto.Name,
            Location = createDto.Location,
            EventDateTime = createDto.EventDateTime,
            Description = createDto.Description,
            Directions = createDto.Directions,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mapperMock.Setup(m => m.Map<AwardEvent>(createDto)).Returns(mappedEntity);

        var expectedResponseDto = new AwardEventResponseDto
        {
            Id = mappedEntity.Id,
            Name = mappedEntity.Name,
            Location = mappedEntity.Location,
            EventDateTime = mappedEntity.EventDateTime,
            Description = mappedEntity.Description,
            Directions = mappedEntity.Directions,
            CreatedAt = mappedEntity.CreatedAt,
            UpdatedAt = mappedEntity.UpdatedAt
        };

        _mapperMock.Setup(m => m.Map<AwardEventResponseDto>(mappedEntity)).Returns(expectedResponseDto);
        _repositoryMock.Setup(r => r.AddAsync(mappedEntity)).Returns(Task.CompletedTask);

        var response = await _service.CreateAwardEventAsync(createDto);

        response.Match(
            onSuccess: dto =>
            {
                _repositoryMock.Verify(r => r.AddAsync(mappedEntity), Times.Once);
                dto.Should().BeEquivalentTo(expectedResponseDto);
                return dto;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected AwardEventResponseDto but got error: " + error);
                return new AwardEventResponseDto();
            });
    }


    [Fact]
    public async Task UpdateAwardEventAsync_WhenEventExists_ReturnsTrueAndUpdatesEntity()
    {
        const int id = 1;
        var awardEvent = new AwardEvent
        {
            Id = id,
            Name = "Old Event",
            Location = "Old Location",
            EventDateTime = DateTime.UtcNow.AddDays(2),
            Description = "Old Desc",
            Directions = "Old Directions",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(awardEvent);

        var updateDto = new AwardEventUpdateDto
        {
            Name = "Updated Event",
            Location = "Updated Location",
            EventDateTime = DateTime.UtcNow.AddDays(3),
            Description = "Updated Desc",
            Directions = "Updated Directions"
        };

        _mapperMock.Setup(m => m.Map(updateDto, awardEvent))
            .Callback<AwardEventUpdateDto, AwardEvent>((dto, e) =>
            {
                e.Name = dto.Name;
                e.Location = dto.Location;
                e.EventDateTime = dto.EventDateTime;
                e.Description = dto.Description;
                e.Directions = dto.Directions;
            });

        _repositoryMock.Setup(r => r.UpdateAsync(awardEvent)).Returns(Task.CompletedTask);

        var response = await _service.UpdateAwardEventAsync(id, updateDto);

        response.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                awardEvent.Name.Should().Be(updateDto.Name);
                awardEvent.Location.Should().Be(updateDto.Location);
                awardEvent.EventDateTime.Should().Be(updateDto.EventDateTime);
                awardEvent.Description.Should().Be(updateDto.Description);
                awardEvent.Directions.Should().Be(updateDto.Directions);
                _repositoryMock.Verify(r => r.UpdateAsync(awardEvent), Times.Once);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected successful update but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task UpdateAwardEventAsync_WhenEventDoesNotExist_ReturnsErrorString()
    {
        const int id = 999;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as AwardEvent);

        var updateDto = new AwardEventUpdateDto
        {
            Name = "Updated Event",
            Location = "Updated Location",
            EventDateTime = DateTime.UtcNow.AddDays(3),
            Description = "Updated Desc",
            Directions = "Updated Directions"
        };

        var response = await _service.UpdateAwardEventAsync(id, updateDto);

        response.Match(
            onSuccess: success =>
            {
                false.Should().BeTrue("Expected error message for updating a non-existent AwardEvent.");
                return success;
            },
            onError: error =>
            {
                error.Should().Be($"AwardEvent with ID {id} not found for update.");
                return false;
            });
    }


    [Fact]
    public async Task DeleteAwardEventAsync_WhenEventExists_ReturnsTrue()
    {
        const int id = 1;
        var awardEvent = new AwardEvent
        {
            Id = id,
            Name = "Event to Delete",
            Location = "Location",
            EventDateTime = DateTime.UtcNow.AddDays(2),
            Description = "Desc",
            Directions = "Directions",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(awardEvent);
        _repositoryMock.Setup(r => r.DeleteAsync(awardEvent)).Returns(Task.CompletedTask);

        var response = await _service.DeleteAwardEventAsync(id);

        response.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                _repositoryMock.Verify(r => r.DeleteAsync(awardEvent), Times.Once);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected successful deletion but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task DeleteAwardEventAsync_WhenEventDoesNotExist_ReturnsErrorString()
    {
        const int id = 1;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as AwardEvent);

        var response = await _service.DeleteAwardEventAsync(id);

        response.Match(
            onSuccess: success =>
            {
                false.Should().BeTrue("Expected error message for deletion of non-existent AwardEvent.");
                return success;
            },
            onError: error =>
            {
                error.Should().Be($"AwardEvent with ID {id} not found for deletion.");
                return false;
            });
    }
}