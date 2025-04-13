using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AwardSystemAPI.Tests.UnitTests.NominationTests;

public class NominationServiceTests
{
    private readonly Mock<INominationRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAiSummaryService> _aiMock;
    private readonly Mock<INomineeSummaryService> _summaryMock;
    private readonly Mock<ILogger<NominationService>> _loggerMock;
    private readonly NominationService _service;

    public NominationServiceTests()
    {
        _repoMock = new Mock<INominationRepository>();
        _mapperMock = new Mock<IMapper>();
        _aiMock = new Mock<IAiSummaryService>();
        _summaryMock = new Mock<INomineeSummaryService>();
        _loggerMock = new Mock<ILogger<NominationService>>();
        _service = new NominationService(
            _repoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _aiMock.Object,
            _summaryMock.Object
        );
    }

    [Fact]
    public async Task CreateNominationAsync_WhenDtoNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateNominationAsync(null!, 1));
    }

    [Fact]
    public async Task CreateNominationAsync_MapsAndSavesAndReturnsDto()
    {
        var dto = new NominationCreateDto
        {
            CategoryId = 3,
            NomineeId = 5,
            Answers = [],
            TeamMembers = null
        };
        var entity = new Nomination { Id = 10, CategoryId = 3, NomineeId = 5 };
        _mapperMock.Setup(m => m.Map<Nomination>(dto)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IEnumerable<NominationAnswer>>(dto.Answers)).Returns(new List<NominationAnswer>());
        _aiMock.Setup(a => a.GenerateNominationSummaryAsync(entity, It.IsAny<IEnumerable<NominationAnswer>>()))
            .ReturnsAsync("summary");
        _repoMock.Setup(r => r.AddAsync(entity)).Returns(Task.CompletedTask);
        var responseDto = new NominationResponseDto { Id = 10, CategoryId = 3, AiSummary = "summary" };
        _mapperMock.Setup(m => m.Map<NominationResponseDto>(entity)).Returns(responseDto);
        _summaryMock.Setup(s => s.GetNomineeSummaryAsync(5, 3))
            .ReturnsAsync("not found");
        _summaryMock.Setup(s => s.CreateNomineeSummaryAsync(It.Is<NomineeSummaryCreateDto>(c => c.NomineeId == 5 && c.CategoryId == 3 && c.TotalNominations == 1)))
            .ReturnsAsync(new NomineeSummaryResponseDto { Id = 1, NomineeId = 5, CategoryId = 3, TotalNominations = 1 });

        var result = await _service.CreateNominationAsync(dto, userId: 7);

        result.Match(
            onSuccess: returned =>
            {
                returned.Should().BeEquivalentTo(responseDto);
                _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
                _aiMock.Verify(a => a.GenerateNominationSummaryAsync(entity, It.IsAny<IEnumerable<NominationAnswer>>()), Times.Once);
                _summaryMock.Verify(s => s.CreateNomineeSummaryAsync(It.IsAny<NomineeSummaryCreateDto>()), Times.Once);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue($"Expected success but got error {error}");
                return false;
            });
    }

    [Fact]
    public async Task GetNominationByIdAsync_WhenExists_ReturnsDto()
    {
        var nomination = new Nomination { Id = 2 };
        _repoMock.Setup(r => r.GetNominationByIdAsync(2)).ReturnsAsync(nomination);
        var dto = new NominationResponseDto { Id = 2 };
        _mapperMock.Setup(m => m.Map<NominationResponseDto>(nomination)).Returns(dto);

        var result = await _service.GetNominationByIdAsync(2);

        result.Match(
            onSuccess: returned =>
            {
                returned.Should().BeEquivalentTo(dto);
                return true;
            },
            onError: error =>
            {
                false.Should().BeTrue($"Expected success but got {error}");
                return false;
            });
    }

    [Fact]
    public async Task GetNominationByIdAsync_WhenNotExists_ReturnsError()
    {
        _repoMock.Setup(r => r.GetNominationByIdAsync(5)).ReturnsAsync(null as Nomination);

        var result = await _service.GetNominationByIdAsync(5);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error");
                return true;
            },
            onError: error =>
            {
                error.Should().Be("Nomination with ID 5 not found.");
                return false;
            });
    }

    [Fact]
    public async Task GetNominationsByCreatorIdAsync_ReturnsDtos()
    {
        var list = new List<Nomination> { new() { Id = 3 } };
        _repoMock.Setup(r => r.GetNominationsByCreatorIdAsync(7)).ReturnsAsync(list);
        var dtos = list.Select(n => new NominationResponseDto { Id = n.Id });
        dtos = dtos.ToArray();
        _mapperMock.Setup(m => m.Map<IEnumerable<NominationResponseDto>>(list)).Returns(dtos);

        var result = await _service.GetNominationsByCreatorIdAsync(7);
        result.Match(
            onSuccess: returned =>
            {
                returned.Should().BeEquivalentTo(dtos);
                return true;
            },
            onError: _ => false);
    }

    [Fact]
    public async Task GetNominationsForNomineeIdAsync_ReturnsDtos()
    {
        var list = new List<Nomination> { new Nomination { Id = 4 } };
        _repoMock.Setup(r => r.GetNominationsForNomineeIdAsync(8)).ReturnsAsync(list);
        var dtos = list.Select(n => new NominationResponseDto { Id = n.Id });
        dtos = dtos.ToArray();
        _mapperMock.Setup(m => m.Map<IEnumerable<NominationResponseDto>>(list)).Returns(dtos);

        var result = await _service.GetNominationsForNomineeIdAsync(8);
        result.Match(
            onSuccess: returned =>
            {
                returned.Should().BeEquivalentTo(dtos);
                return true;
            },
            onError: _ => false);
    }

    [Fact]
    public async Task GetTeamNominationsForMemberAsync_WhenEmpty_ReturnsError()
    {
        _repoMock.Setup(r => r.GetTeamNominationsForMemberAsync(9)).ReturnsAsync(new List<Nomination>());

        var result = await _service.GetTeamNominationsForMemberAsync(9);

        result.Match(
            onSuccess: _ =>
            {
                false.Should().BeTrue("Expected error");
                return true;
            },
            onError: error =>
            {
                error.Should().Be("No nominations found for user ID 9.");
                return false;
            });
    }

    [Fact]
    public async Task GetTeamNominationsForMemberAsync_WhenNotEmpty_ReturnsDtos()
    {
        var list = new List<Nomination> { new() { Id = 6 } };
        _repoMock.Setup(r => r.GetTeamNominationsForMemberAsync(9)).ReturnsAsync(list);
        var dtos = list.Select(n => new NominationResponseDto { Id = n.Id });
        dtos = dtos.ToArray();
        _mapperMock.Setup(m => m.Map<IEnumerable<NominationResponseDto>>(list)).Returns(dtos);

        var result = await _service.GetTeamNominationsForMemberAsync(9);
        result.Match(
            onSuccess: returned =>
            {
                returned.Should().BeEquivalentTo(dtos);
                return true;
            },
            onError: _ => false);
    }
}