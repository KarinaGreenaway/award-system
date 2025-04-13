using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.NomineeSummaryTests;

public class NomineeSummaryServiceTests
{
    private readonly Mock<INomineeSummaryRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly NomineeSummaryService _service;

    public NomineeSummaryServiceTests()
    {
        _repoMock = new Mock<INomineeSummaryRepository>();
        _mapperMock = new Mock<IMapper>();
        Mock<ILogger<NomineeSummaryService>> loggerMock = new();
        _service = new NomineeSummaryService(
            _repoMock.Object,
            loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task CreateNomineeSummaryAsync_NullDto_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateNomineeSummaryAsync(null));
    }

    [Fact]
    public async Task CreateNomineeSummaryAsync_ValidDto_AddsAndReturnsDto()
    {
        var createDto = new NomineeSummaryCreateDto
        {
            NomineeId = 5,
            CategoryId = 3,
            TotalNominations = 2
        };
        var entity = new NomineeSummary { Id = 8, NomineeId = 5, CategoryId = 3, TotalNominations = 2 };
        _mapperMock.Setup(m => m.Map<NomineeSummary>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.AddAsync(entity)).Returns(Task.CompletedTask);
        var responseDto = new NomineeSummaryResponseDto { Id = 8, NomineeId = 5, CategoryId = 3, TotalNominations = 2 };
        _mapperMock.Setup(m => m.Map<NomineeSummaryResponseDto>(entity)).Returns(responseDto);

        var result = await _service.CreateNomineeSummaryAsync(createDto);

        result.Match(
            onSuccess: dto =>
            {
                dto.Should().BeEquivalentTo(responseDto);
                _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
                return true;
            },
            onError: err =>
            {
                false.Should().BeTrue($"Expected success but got error: {err}");
                return false;
            }
        );
    }

    [Fact]
    public async Task GetNomineeSummaryAsync_WhenExists_ReturnsDto()
    {
        const int nomineeId = 5;
        const int categoryId = 3;
        var entity = new NomineeSummary { Id = 2, NomineeId = nomineeId, CategoryId = categoryId, TotalNominations = 4 };
        _repoMock.Setup(r => r.GetByNomineeIdAndCategoryIdAsync(nomineeId, categoryId)).ReturnsAsync(entity);
        var responseDto = new NomineeSummaryResponseDto { Id = 2, NomineeId = nomineeId, CategoryId = categoryId, TotalNominations = 4 };
        _mapperMock.Setup(m => m.Map<NomineeSummaryResponseDto>(entity)).Returns(responseDto);

        var result = await _service.GetNomineeSummaryAsync(nomineeId, categoryId);

        result.Match(
            onSuccess: dto =>
            {
                dto.Should().BeEquivalentTo(responseDto);
                return true;
            },
            onError: err =>
            {
                false.Should().BeTrue($"Expected success but got error: {err}");
                return false;
            }
        );
    }

    [Fact]
    public async Task GetNomineeSummaryAsync_WhenNotExists_ReturnsError()
    {
        _repoMock.Setup(r => r.GetByNomineeIdAndCategoryIdAsync(1, 2)).ReturnsAsync(null as NomineeSummary);

        var result = await _service.GetNomineeSummaryAsync(1, 2);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error");
                return true;
            },
            onError: err =>
            {
                err.Should().Be("NomineeSummary for NomineeId 1 and CategoryId 2 not found.");
                return false;
            }
        );
    }

    [Fact]
    public async Task GetAllNomineeSummariesAsync_ReturnsDtos()
    {
        var list = new List<NomineeSummary> { new NomineeSummary { Id = 1 }, new NomineeSummary { Id = 2 } };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(list);
        
        var dtos = list.Select(ns => new NomineeSummaryResponseDto { Id = ns.Id });
        dtos = dtos.ToArray();
        
        _mapperMock.Setup(m => m.Map<IEnumerable<NomineeSummaryResponseDto>>(list)).Returns(dtos);

        var result = await _service.GetAllNomineeSummariesAsync();

        result.Match(
            onSuccess: returned =>
            {
                returned.Should().BeEquivalentTo(dtos);
                return true;
            },
            onError: _ => false
        );
    }

    [Fact]
    public async Task GetAllNomineeSummariesByCategoryIdAsync_WhenEmpty_ReturnsError()
    {
        _repoMock.Setup(r => r.GetByCategoryIdAsync(7)).ReturnsAsync(new List<NomineeSummary>());

        var result = await _service.GetAllNomineeSummariesByCategoryIdAsync(7);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error");
                return true;
            },
            onError: err =>
            {
                err.Should().Be("NomineeSummary for CategoryId 7 not found.");
                return false;
            }
        );
    }

    [Fact]
    public async Task GetAllNomineeSummariesByCategoryIdAsync_WhenNotEmpty_ReturnsDtos()
    {
        var list = new List<NomineeSummary> { new NomineeSummary { Id = 9 } };
        _repoMock.Setup(r => r.GetByCategoryIdAsync(7)).ReturnsAsync(list);
        
        var dtos = list.Select(ns => new NomineeSummaryResponseDto { Id = ns.Id });
        dtos = dtos.ToArray();
        
        _mapperMock.Setup(m => m.Map<IEnumerable<NomineeSummaryResponseDto>>(list)).Returns(dtos);

        var result = await _service.GetAllNomineeSummariesByCategoryIdAsync(7);

        result.Match(
            onSuccess: returned =>
            {
                returned.Should().BeEquivalentTo(dtos);
                return true;
            },
            onError: _ => false
        );
    }

    [Fact]
    public async Task UpdateNomineeSummaryAsync_WhenNotExists_ReturnsError()
    {
        _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(null as NomineeSummary);

        var result = await _service.UpdateNomineeSummaryAsync(5, new NomineeSummaryUpdateDto());

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error");
                return true;
            },
            onError: err =>
            {
                err.Should().Be("NomineeSummary with ID 5 not found.");
                return false;
            }
        );
    }

    [Fact]
    public async Task UpdateNomineeSummaryAsync_WhenExists_UpdatesAndReturnsTrue()
    {
        var entity = new NomineeSummary { Id = 6, NomineeId = 2, CategoryId = 3, TotalNominations = 1 };
        _repoMock.Setup(r => r.GetByIdAsync(6)).ReturnsAsync(entity);
        _repoMock.Setup(r => r.CountIndividualNominationsForNomineeAsync(2, 3)).ReturnsAsync(5);
        _mapperMock.Setup(m => m.Map<NomineeSummaryUpdateDto>(entity)).Returns(new NomineeSummaryUpdateDto());
        _mapperMock.Setup(m => m.Map(It.IsAny<NomineeSummaryUpdateDto>(), entity));
        _repoMock.Setup(r => r.UpdateAsync(entity)).Returns(Task.CompletedTask);

        var result = await _service.UpdateNomineeSummaryAsync(6, new NomineeSummaryUpdateDto());

        result.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                _repoMock.Verify(r => r.UpdateAsync(entity), Times.Once);
                return true;
            },
            onError: _ => false
        );
    }

    [Fact]
    public async Task UpdateNomineeSummaryTotalNominationCountyAsync_WhenNotExists_ReturnsError()
    {
        _repoMock.Setup(r => r.GetByNomineeIdAndCategoryIdAsync(2,3)).ReturnsAsync(null as NomineeSummary);

        var result = await _service.UpdateNomineeSummaryTotalNominationCountyAsync(2,3);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error");
                return true;
            },
            onError: err =>
            {
                err.Should().Be("NomineeSummary with NomineeId 2 and CategoryId 3 not found.");
                return false;
            }
        );
    }

    [Fact]
    public async Task UpdateNomineeSummaryTotalNominationCountyAsync_WhenExists_UpdatesAndReturnsTrue()
    {
        var entity = new NomineeSummary { Id = 7, NomineeId = 2, CategoryId = 3, TotalNominations = 1 };
        _repoMock.Setup(r => r.GetByNomineeIdAndCategoryIdAsync(2,3)).ReturnsAsync(entity);
        _repoMock.Setup(r => r.CountIndividualNominationsForNomineeAsync(2,3)).ReturnsAsync(10);
        _mapperMock.Setup(m => m.Map<NomineeSummaryUpdateDto>(entity)).Returns(new NomineeSummaryUpdateDto());
        _mapperMock.Setup(m => m.Map(It.IsAny<NomineeSummaryUpdateDto>(), entity));
        _repoMock.Setup(r => r.UpdateAsync(entity)).Returns(Task.CompletedTask);

        var result = await _service.UpdateNomineeSummaryTotalNominationCountyAsync(2,3);

        result.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                _repoMock.Verify(r => r.UpdateAsync(entity), Times.Once);
                return true;
            },
            onError: _ => false
        );
    }

    [Fact]
    public async Task DeleteNomineeSummaryAsync_WhenNotExists_ReturnsError()
    {
        _repoMock.Setup(r => r.GetByNomineeIdAndCategoryIdAsync(4,5)).ReturnsAsync(null as NomineeSummary);

        var result = await _service.DeleteNomineeSummaryAsync(4,5);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error");
                return true;
            },
            onError: err =>
            {
                err.Should().Be("NomineeSummary for NomineeId 4 and CategoryId 5 not found for deletion.");
                return false;
            }
        );
    }

    [Fact]
    public async Task DeleteNomineeSummaryAsync_WhenExists_DeletesAndReturnsTrue()
    {
        var entity = new NomineeSummary { Id = 9, NomineeId = 4, CategoryId = 5 };
        _repoMock.Setup(r => r.GetByNomineeIdAndCategoryIdAsync(4,5)).ReturnsAsync(entity);
        _repoMock.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

        var result = await _service.DeleteNomineeSummaryAsync(4,5);

        result.Match(
            onSuccess: success =>
            {
                success.Should().BeTrue();
                _repoMock.Verify(r => r.DeleteAsync(entity), Times.Once);
                return true;
            },
            onError: _ => false
        );
    }
}