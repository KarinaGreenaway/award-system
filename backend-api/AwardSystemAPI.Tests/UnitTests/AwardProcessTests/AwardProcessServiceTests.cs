using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.AwardProcessTests;

public class AwardProcessServiceTests
{
    private readonly Mock<IGenericRepository<AwardProcess>> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<AwardProcessService>> _loggerMock;
    private readonly AwardProcessService _service;
    
    public AwardProcessServiceTests()
    {
        _repositoryMock = new Mock<IGenericRepository<AwardProcess>>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<AwardProcessService>>();
        _service = new AwardProcessService(_repositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAwardProcessesAsync_WhenAwardProcessesExist_ShouldReturnMappedDtos()
    {
        var now = DateTime.UtcNow;
        var awardProcesses = new List<AwardProcess>
        {
            new AwardProcess { Id = 1, AwardsName = "Award 1", StartDate = now, Status = "active", CreatedAt = now, UpdatedAt = now },
            new AwardProcess { Id = 2, AwardsName = "Award 2", StartDate = now, Status = "completed", CreatedAt = now, UpdatedAt = now }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(awardProcesses);

        var expectedDtos = awardProcesses.Select(ap => new AwardProcessResponseDto
        {
            Id = ap.Id,
            AwardsName = ap.AwardsName,
            StartDate = ap.StartDate,
            EndDate = ap.EndDate,
            Status = ap.Status,
            CreatedAt = ap.CreatedAt,
        }).ToList();

        _mapperMock.Setup(m => m.Map<IEnumerable<AwardProcessResponseDto>>(awardProcesses))
            .Returns(expectedDtos);

        ApiResponse<IEnumerable<AwardProcessResponseDto>, string> result = await _service.GetAllAwardProcessesAsync();

        result.Match(
            onSuccess: dtos =>
            {
                dtos.Should().NotBeNull()
                    .And.HaveCount(expectedDtos.Count)
                    .And.BeEquivalentTo(expectedDtos, options => options.ComparingByMembers<AwardProcessResponseDto>());
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected success but got error: {0}", error);
                return false;
            });
    }

    [Fact]
    public async Task GetAwardProcessByIdAsync_WhenAwardProcessExists_ShouldReturnMappedDto()
    {
        var now = DateTime.UtcNow;
        const int id = 1;
        var awardProcess = new AwardProcess
        {
            Id = id,
            AwardsName = "Award",
            StartDate = now,
            Status = "active",
            CreatedAt = now,
            UpdatedAt = now
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(awardProcess);

        var expectedDto = new AwardProcessResponseDto
        {
            Id = awardProcess.Id,
            AwardsName = awardProcess.AwardsName,
            StartDate = awardProcess.StartDate,
            EndDate = awardProcess.EndDate,
            Status = awardProcess.Status,
            CreatedAt = awardProcess.CreatedAt,
        };

        _mapperMock.Setup(m => m.Map<AwardProcessResponseDto>(awardProcess))
            .Returns(expectedDto);

        ApiResponse<AwardProcessResponseDto?, string> result = await _service.GetAwardProcessByIdAsync(id);

        result.Match(
            onSuccess: dto =>
            {
                dto.Should().NotBeNull().And.BeEquivalentTo(expectedDto, options => options.ComparingByMembers<AwardProcessResponseDto>());
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected valid AwardProcessResponseDto but received error: {0}", error);
                return false;
            });
    }

    [Fact]
    public async Task GetAwardProcessByIdAsync_WhenAwardProcessDoesNotExist_ShouldReturnErrorString()
    {
        const int id = 42;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as AwardProcess);

        ApiResponse<AwardProcessResponseDto?, string> result = await _service.GetAwardProcessByIdAsync(id);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error message for non-existent AwardProcess.");
                return true;
            },
            onError: error =>
            {
                error.Should().Be($"AwardProcess with ID {id} not found.");
                return false;
            });
    }

    [Fact]
    public async Task CreateAwardProcessAsync_WhenValidDtoIsProvided_ShouldReturnMappedDtoAndAddEntity()
    {
        var now = DateTime.UtcNow;
        var createDto = new AwardProcessCreateDto
        {
            AwardsName = "New Award",
            StartDate = now,
            EndDate = now.AddDays(1),
            Status = "active"
        };

        var mappedEntity = new AwardProcess
        {
            Id = 10,
            AwardsName = createDto.AwardsName,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            Status = createDto.Status,
            CreatedAt = now,
            UpdatedAt = now
        };

        _mapperMock.Setup(m => m.Map<AwardProcess>(createDto)).Returns(mappedEntity);

        var expectedResponseDto = new AwardProcessResponseDto
        {
            Id = mappedEntity.Id,
            AwardsName = mappedEntity.AwardsName,
            StartDate = mappedEntity.StartDate,
            EndDate = mappedEntity.EndDate,
            Status = mappedEntity.Status,
            CreatedAt = mappedEntity.CreatedAt,
        };

        _mapperMock.Setup(m => m.Map<AwardProcessResponseDto>(mappedEntity)).Returns(expectedResponseDto);
        _repositoryMock.Setup(r => r.AddAsync(mappedEntity)).Returns(Task.CompletedTask);

        ApiResponse<AwardProcessResponseDto, string> result = await _service.CreateAwardProcessAsync(createDto);

        result.Match(
            onSuccess: dto =>
            {
                _repositoryMock.Verify(r => r.AddAsync(mappedEntity), Times.Once);
                dto.Should().BeEquivalentTo(expectedResponseDto, options => options.ComparingByMembers<AwardProcessResponseDto>());
                return dto;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected AwardProcessResponseDto but received error: {0}", error);
                return new AwardProcessResponseDto();
            });
    }

    [Fact]
    public async Task UpdateAwardProcessAsync_WhenAwardProcessExists_ShouldReturnTrueAndUpdateEntity()
    {
        var now = DateTime.UtcNow;
        const int id = 5;
        var existingEntity = new AwardProcess
        {
            Id = id,
            AwardsName = "Original Award",
            StartDate = now,
            Status = "active",
            CreatedAt = now,
            UpdatedAt = now
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEntity);

        var updateDto = new AwardProcessUpdateDto
        {
            AwardsName = "Updated Award",
            StartDate = now,
            EndDate = now.AddDays(2),
            Status = "completed"
        };

        _mapperMock.Setup(m => m.Map(updateDto, existingEntity))
            .Callback<AwardProcessUpdateDto, AwardProcess>((dto, process) =>
            {
                process.AwardsName = dto.AwardsName;
                process.StartDate = dto.StartDate;
                process.EndDate = dto.EndDate;
                process.Status = dto.Status;
            });
            
        _repositoryMock.Setup(r => r.UpdateAsync(existingEntity)).Returns(Task.CompletedTask);

        ApiResponse<bool, string> result = await _service.UpdateAwardProcessAsync(id, updateDto);

        result.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                existingEntity.AwardsName.Should().Be(updateDto.AwardsName);
                existingEntity.Status.Should().Be(updateDto.Status);
                return success;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected successful update but received error: {0}", error);
                return false;
            });
    }

    [Fact]
    public async Task UpdateAwardProcessAsync_WhenAwardProcessDoesNotExist_ShouldReturnErrorString()
    {
        const int id = 999;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as AwardProcess);

        var updateDto = new AwardProcessUpdateDto
        {
            AwardsName = "Updated Award",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2),
            Status = "completed"
        };

        ApiResponse<bool, string> result = await _service.UpdateAwardProcessAsync(id, updateDto);

        result.Match(
            onSuccess: success =>
            {
                false.Should().BeTrue("Expected error message for updating a non-existent AwardProcess.");
                return success;
            },
            onError: error =>
            {
                error.Should().Be($"AwardProcess with ID {id} not found for update.");
                return false;
            });
    }

    [Fact]
    public async Task DeleteAwardProcessAsync_WhenAwardProcessExists_ShouldReturnTrueAndDeleteEntity()
    {
        const int id = 3;
        var now = DateTime.UtcNow;
        var existingEntity = new AwardProcess
        {
            Id = id,
            AwardsName = "Award to delete",
            StartDate = now,
            Status = "active",
            CreatedAt = now,
            UpdatedAt = now
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEntity);
        _repositoryMock.Setup(r => r.DeleteAsync(existingEntity)).Returns(Task.CompletedTask);

        ApiResponse<bool, string> result = await _service.DeleteAwardProcessAsync(id);

        result.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                _repositoryMock.Verify(r => r.DeleteAsync(existingEntity), Times.Once);
                return success;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected successful deletion but received error: {0}", error);
                return false;
            });
    }

    [Fact]
    public async Task DeleteAwardProcessAsync_WhenAwardProcessDoesNotExist_ShouldReturnErrorString()
    {
        const int id = 1000;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as AwardProcess);

        ApiResponse<bool, string> result = await _service.DeleteAwardProcessAsync(id);

        result.Match(
            onSuccess: success =>
            {
                false.Should().BeTrue("Expected error message for deleting non-existent AwardProcess.");
                return success;
            },
            onError: error =>
            {
                error.Should().Be($"AwardProcess with ID {id} not found for deletion.");
                return false;
            });
    }
}