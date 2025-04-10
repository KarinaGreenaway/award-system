using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.JudgingRoundTests;

public class JudgingRoundServiceTests
{
    private readonly Mock<IGenericRepository<JudgingRound>> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<JudgingRoundService>> _loggerMock;
    private readonly IJudgingRoundService _service;

    public JudgingRoundServiceTests()
    {
        _repositoryMock = new Mock<IGenericRepository<JudgingRound>>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<JudgingRoundService>>();
        _service = new JudgingRoundService(_repositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateJudgingRoundAsync_WithNullDto_ThrowsArgumentNullException()
    {
        Func<Task> act = async () => await _service.CreateJudgingRoundAsync(null);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CreateJudgingRoundAsync_WithValidDto_ReturnsMappedDtoAndAddsEntity()
    {
        var createDto = new JudgingRoundCreateDto
        {
            AwardProcessId = 1,
            RoundName = "Round 1",
            StartDate = DateTime.UtcNow.AddDays(1),
            Deadline = DateTime.UtcNow.AddDays(2),
            CandidateCount = 5
        };

        var mappedEntity = new JudgingRound
        {
            Id = 10,
            AwardProcessId = createDto.AwardProcessId,
            RoundName = createDto.RoundName,
            StartDate = createDto.StartDate,
            Deadline = createDto.Deadline,
            CandidateCount = createDto.CandidateCount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mapperMock.Setup(m => m.Map<JudgingRound>(createDto)).Returns(mappedEntity);

        var expectedResponseDto = new JudgingRoundResponseDto
        {
            Id = mappedEntity.Id,
            AwardProcessId = mappedEntity.AwardProcessId,
            RoundName = mappedEntity.RoundName,
            StartDate = mappedEntity.StartDate,
            Deadline = mappedEntity.Deadline,
            CandidateCount = mappedEntity.CandidateCount,
            CreatedAt = mappedEntity.CreatedAt,
            UpdatedAt = mappedEntity.UpdatedAt
        };

        _mapperMock.Setup(m => m.Map<JudgingRoundResponseDto>(mappedEntity)).Returns(expectedResponseDto);
        _repositoryMock.Setup(r => r.AddAsync(mappedEntity)).Returns(Task.CompletedTask);

        var response = await _service.CreateJudgingRoundAsync(createDto);

        response.Match(
            onSuccess: dto =>
            {
                _repositoryMock.Verify(r => r.AddAsync(mappedEntity), Times.Once);
                dto.Should().BeEquivalentTo(expectedResponseDto);
                return dto;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected successful creation but got error: " + error);
                return new JudgingRoundResponseDto();
            });
    }


    [Fact]
    public async Task GetJudgingRoundByIdAsync_WhenEntityExists_ReturnsMappedDto()
    {
        const int id = 1;
        var entity = new JudgingRound
        {
            Id = id,
            AwardProcessId = 1,
            RoundName = "Round A",
            StartDate = DateTime.UtcNow.AddDays(1),
            Deadline = DateTime.UtcNow.AddDays(2),
            CandidateCount = 3,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        var expectedDto = new JudgingRoundResponseDto
        {
            Id = entity.Id,
            AwardProcessId = entity.AwardProcessId,
            RoundName = entity.RoundName,
            StartDate = entity.StartDate,
            Deadline = entity.Deadline,
            CandidateCount = entity.CandidateCount,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        _mapperMock.Setup(m => m.Map<JudgingRoundResponseDto>(entity)).Returns(expectedDto);

        var response = await _service.GetJudgingRoundByIdAsync(id);

        response.Match(
            onSuccess: dto =>
            {
                dto.Should().BeEquivalentTo(expectedDto);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected a valid JudgingRoundResponseDto but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task GetJudgingRoundByIdAsync_WhenEntityDoesNotExist_ReturnsErrorMessage()
    {
        const int id = 1;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as JudgingRound);

        var response = await _service.GetJudgingRoundByIdAsync(id);

        response.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected an error message for non-existent JudgingRound.");
                return true;
            },
            onError: error =>
            {
                error.Should().Be($"JudgingRound with ID {id} not found.");
                return false;
            });
    }


    [Fact]
    public async Task UpdateJudgingRoundAsync_WhenEntityExists_ReturnsTrueAndUpdatesEntity()
    {
        const int id = 1;
        var entity = new JudgingRound
        {
            Id = id,
            AwardProcessId = 1,
            RoundName = "Old Round",
            StartDate = DateTime.UtcNow.AddDays(1),
            Deadline = DateTime.UtcNow.AddDays(2),
            CandidateCount = 4,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        var updateDto = new JudgingRoundUpdateDto
        {
            RoundName = "Updated Round",
            StartDate = DateTime.UtcNow.AddDays(3),
            Deadline = DateTime.UtcNow.AddDays(4),
            CandidateCount = 5
        };

        _mapperMock.Setup(m => m.Map(updateDto, entity))
            .Callback<JudgingRoundUpdateDto, JudgingRound>((dto, e) =>
            {
                e.RoundName = dto.RoundName;
                e.StartDate = dto.StartDate;
                e.Deadline = dto.Deadline;
                e.CandidateCount = dto.CandidateCount;
            });

        _repositoryMock.Setup(r => r.UpdateAsync(entity)).Returns(Task.CompletedTask);

        var response = await _service.UpdateJudgingRoundAsync(id, updateDto);

        response.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                entity.RoundName.Should().Be(updateDto.RoundName);
                entity.StartDate.Should().Be(updateDto.StartDate);
                entity.Deadline.Should().Be(updateDto.Deadline);
                entity.CandidateCount.Should().Be(updateDto.CandidateCount);
                _repositoryMock.Verify(r => r.UpdateAsync(entity), Times.Once);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected successful update, but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task UpdateJudgingRoundAsync_WhenEntityDoesNotExist_ReturnsErrorMessage()
    {
        const int id = 999;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as JudgingRound);

        var updateDto = new JudgingRoundUpdateDto
        {
            RoundName = "Updated Round",
            StartDate = DateTime.UtcNow.AddDays(3),
            Deadline = DateTime.UtcNow.AddDays(4),
            CandidateCount = 5
        };

        var response = await _service.UpdateJudgingRoundAsync(id, updateDto);

        response.Match(
            onSuccess: success =>
            {
                false.Should().BeTrue("Expected an error message when updating a non-existent JudgingRound.");
                return success;
            },
            onError: error =>
            {
                error.Should().Be($"JudgingRound with ID {id} not found for update.");
                return false;
            });
    }


    [Fact]
    public async Task DeleteJudgingRoundAsync_WhenEntityExists_ReturnsTrue()
    {
        const int id = 1;
        var entity = new JudgingRound
        {
            Id = id,
            AwardProcessId = 1,
            RoundName = "To be deleted",
            StartDate = DateTime.UtcNow.AddDays(1),
            Deadline = DateTime.UtcNow.AddDays(2),
            CandidateCount = 3,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

        var response = await _service.DeleteJudgingRoundAsync(id);

        response.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                _repositoryMock.Verify(r => r.DeleteAsync(entity), Times.Once);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue("Expected successful deletion but got error: " + error);
                return false;
            });
    }

    [Fact]
    public async Task DeleteJudgingRoundAsync_WhenEntityDoesNotExist_ReturnsErrorMessage()
    {
        const int id = 1;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as JudgingRound);

        var response = await _service.DeleteJudgingRoundAsync(id);

        response.Match(
            onSuccess: success =>
            {
                false.Should().BeTrue("Expected an error message for deletion of a non-existent JudgingRound.");
                return success;
            },
            onError: error =>
            {
                error.Should().Be($"JudgingRound with ID {id} not found for deletion.");
                return false;
            });
    }


    [Fact]
    public async Task GetJudgingRoundsByAwardProcessIdAsync_ReturnsMappedDtos()
    {
        const int awardProcessId = 1;
        var rounds = new List<JudgingRound>
        {
            new JudgingRound
            {
                Id = 1,
                AwardProcessId = awardProcessId,
                RoundName = "Round 1",
                StartDate = DateTime.UtcNow.AddDays(1),
                Deadline = DateTime.UtcNow.AddDays(2),
                CandidateCount = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JudgingRound
            {
                Id = 2,
                AwardProcessId = awardProcessId,
                RoundName = "Round 2",
                StartDate = DateTime.UtcNow.AddDays(3),
                Deadline = DateTime.UtcNow.AddDays(4),
                CandidateCount = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(rounds);

        var expectedDtos = rounds.Select(r => new JudgingRoundResponseDto
        {
            Id = r.Id,
            AwardProcessId = r.AwardProcessId,
            RoundName = r.RoundName,
            StartDate = r.StartDate,
            Deadline = r.Deadline,
            CandidateCount = r.CandidateCount,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToArray();

        _mapperMock.Setup(m => m.Map<IEnumerable<JudgingRoundResponseDto>>(rounds)).Returns(expectedDtos);

        var response = await _service.GetJudgingRoundsByAwardProcessIdAsync(awardProcessId);

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
}