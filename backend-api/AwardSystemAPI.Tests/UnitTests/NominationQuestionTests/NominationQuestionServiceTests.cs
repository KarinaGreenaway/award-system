using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.NominationQuestionTests;

public class NominationQuestionServiceTests
{
    private readonly Mock<INominationQuestionRepository> _repoMock;
    private readonly Mock<IAwardCategoryRepository> _categoryRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<NominationQuestionService>> _loggerMock;
    private readonly NominationQuestionService _service;

    public NominationQuestionServiceTests()
    {
        _repoMock = new Mock<INominationQuestionRepository>();
        _categoryRepoMock = new Mock<IAwardCategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<NominationQuestionService>>();
        _service = new NominationQuestionService(
            _repoMock.Object,
            _categoryRepoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetQuestionsByCategoryAsync_ReturnsMappedDtos()
    {
        int categoryId = 5;
        var entities = new List<NominationQuestion>
        {
            new NominationQuestion { Id = 1, CategoryId = categoryId, QuestionText = "Q1" },
            new NominationQuestion { Id = 2, CategoryId = categoryId, QuestionText = "Q2" }
        };
        _repoMock.Setup(r => r.GetByCategoryIdAsync(categoryId)).ReturnsAsync(entities);

        var dtos = new List<NominationQuestionResponseDto>
        {
            new NominationQuestionResponseDto { Id = 1, CategoryId = categoryId, QuestionText = "Q1" },
            new NominationQuestionResponseDto { Id = 2, CategoryId = categoryId, QuestionText = "Q2" }
        };
        _mapperMock.Setup(m => m.Map<IEnumerable<NominationQuestionResponseDto>>(entities)).Returns(dtos);

        var result = await _service.GetQuestionsByCategoryAsync(categoryId);

        result.Match(
            onSuccess: returnedDtos =>
            {
                returnedDtos.Should().BeEquivalentTo(dtos);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue($"Expected success but got error: {error}");
                return false;
            }
        );
    }

    [Fact]
    public async Task GetQuestionByIdAsync_WhenExists_ReturnsDto()
    {
        const int id = 10;
        var entity = new NominationQuestion { Id = id, CategoryId = 3, QuestionText = "Sample?" };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        var dto = new NominationQuestionResponseDto { Id = id, CategoryId = 3, QuestionText = "Sample?" };
        _mapperMock.Setup(m => m.Map<NominationQuestionResponseDto>(entity)).Returns(dto);

        var result = await _service.GetQuestionByIdAsync(id);

        result.Match(
            onSuccess: returnedDto =>
            {
                returnedDto.Should().BeEquivalentTo(dto);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue($"Expected success but got error: {error}");
                return false;
            }
        );
    }

    [Fact]
    public async Task GetQuestionByIdAsync_WhenNotExists_ReturnsError()
    {
        const int id = 99;
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as NominationQuestion);

        var result = await _service.GetQuestionByIdAsync(id);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error for missing entity");
                return true;
            },
            onError: error =>
            {
                error.Should().Be($"NominationQuestion with ID {id} not found.");
                return false;
            }
        );
    }

    [Fact]
    public async Task CreateQuestionAsync_WhenDtoValid_AddsAndReturnsDto()
    {
        var createDto = new NominationQuestionCreateDto { CategoryId = 2, QuestionText = "New?" };
        var category = new AwardCategory
        {
            Id = 2,
            SponsorId = 42,
            Name = "Category 1",
            Type = "Individual",
            ProfileStatus = "published",
        };
        _categoryRepoMock.Setup(c => c.GetByIdAsync(2)).ReturnsAsync(category);

        var entity = new NominationQuestion { Id = 7, CategoryId = 2, QuestionText = "New?" };
        _mapperMock.Setup(m => m.Map<NominationQuestion>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.AddAsync(entity)).Returns(Task.CompletedTask);
        var responseDto = new NominationQuestionResponseDto { Id = 7, CategoryId = 2, QuestionText = "New?" };
        _mapperMock.Setup(m => m.Map<NominationQuestionResponseDto>(entity)).Returns(responseDto);

        var result = await _service.CreateQuestionAsync(createDto);

        result.Match(
            onSuccess: dto =>
            {
                dto.Should().BeEquivalentTo(responseDto);
                _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue($"Expected success but got error: {error}");
                return false;
            }
        );
    }
        
    [Fact]
    public async Task CreateQuestionAsync_WhenDtoNull_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateQuestionAsync(null));
    }

    [Fact]
    public async Task UpdateQuestionAsync_WhenExists_MapsAndUpdates()
    {
        const int id = 5;
        var updateDto = new NominationQuestionUpdateDto { QuestionText = "Updated?" };
        var entity = new NominationQuestion { Id = id, CategoryId = 4, QuestionText = "Old?" };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(entity)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map(updateDto, entity));

        var result = await _service.UpdateQuestionAsync(id, updateDto);

        result.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                _repoMock.Verify(r => r.UpdateAsync(entity), Times.Once);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue($"Expected success but got error: {error}");
                return false;
            }
        );
    }

    [Fact]
    public async Task UpdateQuestionAsync_WhenNotExists_ReturnsError()
    {
        const int id = 123;
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as NominationQuestion);
        var updateDto = new NominationQuestionUpdateDto { QuestionText = "X" };

        var result = await _service.UpdateQuestionAsync(id, updateDto);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error for non-existent entity");
                return true;
            },
            onError: error =>
            {
                error.Should().Be($"NominationQuestion with ID {id} not found.");
                return false;
            }
        );
    }

    [Fact]
    public async Task DeleteQuestionAsync_WhenExists_Deletes()
    {
        const int id = 8;
        var entity = new NominationQuestion { Id = id, CategoryId = 1, QuestionText = "Q" };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
        _repoMock.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

        var result = await _service.DeleteQuestionAsync(id);

        result.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                _repoMock.Verify(r => r.DeleteAsync(entity), Times.Once);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue($"Expected success but got error: {error}");
                return false;
            }
        );
    }

    [Fact]
    public async Task DeleteQuestionAsync_WhenNotExists_ReturnsError()
    {
        const int id = 99;
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as NominationQuestion);

        var result = await _service.DeleteQuestionAsync(id);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error for missing entity");
                return true;
            },
            onError: error =>
            {
                error.Should().Be($"NominationQuestion with ID {id} not found.");
                return false;
            }
        );
    }
}