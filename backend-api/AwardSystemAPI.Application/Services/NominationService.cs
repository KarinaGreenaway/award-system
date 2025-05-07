using AutoMapper;
using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using AwardSystemAPI.Application.DTOs;
using Newtonsoft.Json;

namespace AwardSystemAPI.Application.Services;

public interface INominationService
{
    Task<ApiResponse<NominationResponseDto, string>> CreateNominationAsync(NominationCreateDto dto, int userId);
    Task<ApiResponse<NominationResponseDto?, string>> GetNominationByIdAsync(int id);
    Task<ApiResponse<IEnumerable<NominationResponseDto>, string>> GetNominationsByCreatorIdAsync(int creatorId);
    Task<ApiResponse<IEnumerable<NominationResponseDto>, string>> GetNominationsForNomineeIdAsync(int nomineeId);
    Task<ApiResponse<IEnumerable<NominationResponseDto>, string>> GetTeamNominationsForMemberAsync(int userId);
    Task<ApiResponse<IEnumerable<NominationResponseDto>, string>> GetNominationsByCategoryIdAsync(int categoryId);
}

public class NominationService : INominationService
{
    private readonly INominationRepository _repository;
    private readonly IGenericRepository<NominationAnswer> _answerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<NominationService> _logger;
    private readonly IAiSummaryService _aiSummaryService;
    private readonly INomineeSummaryService _nomineeSummaryService;
    
    public NominationService(INominationRepository repository,
        IGenericRepository<NominationAnswer> answerRepository, IMapper mapper,
        ILogger<NominationService> logger, IAiSummaryService aiSummaryService,
        INomineeSummaryService nomineeSummaryService)
    {
        _repository = repository;
        _answerRepository = answerRepository;
        _aiSummaryService = aiSummaryService;
        _nomineeSummaryService = nomineeSummaryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<NominationResponseDto, string>> CreateNominationAsync(NominationCreateDto dto, int userId)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var nomination = _mapper.Map<Nomination>(dto);
        var answers = _mapper.Map<IEnumerable<NominationAnswer>>(dto.Answers);
        
        var nominationAnswers = answers as NominationAnswer[] ?? answers.ToArray();
        nomination.Answers = nominationAnswers;

        if(dto.TeamMembers != null && dto.TeamMembers.Any())
        {
            var teamMembers = _mapper.Map<IEnumerable<TeamMember>>(dto.TeamMembers);
            nomination.TeamMembers = teamMembers.ToList();
        }
        
        
        nomination.CreatedAt = DateTime.UtcNow;
        nomination.UpdatedAt = DateTime.UtcNow;
        nomination.CreatorId = userId;

        var promptData = JsonConvert.SerializeObject(new
        {
            Answers = JsonConvert.SerializeObject(nominationAnswers)
        });
        
        nomination.AiSummary = await _aiSummaryService.GenerateNominationSummaryAsync(promptData);

        await _repository.AddAsync(nomination);
        _logger.LogInformation("Created Nomination with ID {Id}.", nomination.Id);

        await TriggerNomineeSummaryUpdate(nomination);

        var responseDto = _mapper.Map<NominationResponseDto>(nomination);
        return responseDto;
    }

    public async Task<ApiResponse<NominationResponseDto?, string>> GetNominationByIdAsync(int id)
    {
        var nomination = await _repository.GetNominationByIdAsync(id);
        if (nomination == null)
        {
            _logger.LogWarning("Nomination with ID {Id} not found.", id);
            return $"Nomination with ID {id} not found.";
        }

        var responseDto = _mapper.Map<NominationResponseDto>(nomination);
        return responseDto;
    }

    public async Task<ApiResponse<IEnumerable<NominationResponseDto>, string>> GetNominationsByCreatorIdAsync(int creatorId)
    {
        var nominations = await _repository.GetNominationsByCreatorIdAsync(creatorId);
        var responseDtos = _mapper.Map<IEnumerable<NominationResponseDto>>(nominations);
        return responseDtos.ToArray();
    }

    public async Task<ApiResponse<IEnumerable<NominationResponseDto>, string>> GetNominationsForNomineeIdAsync(int nomineeId)
    {
        var nominations = await _repository.GetNominationsForNomineeIdAsync(nomineeId);
        var responseDtos = _mapper.Map<IEnumerable<NominationResponseDto>>(nominations);
        return responseDtos.ToArray();
    }

    public async Task<ApiResponse<IEnumerable<NominationResponseDto>, string>> GetTeamNominationsForMemberAsync(int userId)
    {
        var nominations = await _repository.GetTeamNominationsForMemberAsync(userId);
        if (!nominations.Any())
        {
            _logger.LogWarning("No nominations found for user ID {UserId}.", userId);
            return $"No nominations found for user ID {userId}.";
        }
        var responseDtos = _mapper.Map<IEnumerable<NominationResponseDto>>(nominations);
        
        
        return responseDtos.ToArray();
    }

    public async Task<ApiResponse<IEnumerable<NominationResponseDto>, string>> GetNominationsByCategoryIdAsync(int categoryId)
    {
        var nominations = await _repository.GetNominationsByCategoryIdAsync(categoryId);
        var responseDtos = _mapper.Map<IEnumerable<NominationResponseDto>>(nominations);
        return responseDtos.ToArray();
    }
    private async Task TriggerNomineeSummaryUpdate(Nomination nomination)
    {
        ApiResponse<NomineeSummaryResponseDto, string> nomineeSummaryAsync;
        
        if (nomination.NomineeId != null)
        {
            nomineeSummaryAsync = await _nomineeSummaryService.GetNomineeSummaryAsync(nomination.NomineeId.Value, nomination.CategoryId);
            
            await nomineeSummaryAsync.Match(
                onSuccess: async nomineeSummary =>
                {
                    await _nomineeSummaryService.UpdateNomineeSummaryTotalNominationCountAsync(nomination.NomineeId.Value, nomineeSummary.CategoryId);
                    _logger.LogInformation("Updated NomineeSummary for Nominee ID {NomineeId}.", nomination.NomineeId.Value);
                },
                onError: async _ =>
                {
                    var nomineeSummary = new NomineeSummaryCreateDto
                    {
                        NomineeId = nomination.NomineeId.Value,
                        CategoryId = nomination.CategoryId,
                        Location = nomination.Location,
                        TotalNominations = 1
                    };
                    await _nomineeSummaryService.CreateNomineeSummaryAsync(nomineeSummary);
                    _logger.LogInformation("Created NomineeSummary for Nominee ID {NomineeId}.", nomination.NomineeId.Value);
                }
            );
 
        }
        else
        {
            nomineeSummaryAsync = await _nomineeSummaryService.GetTeamNominationSummaryAsync(nomination.Id, nomination.CategoryId);
            
            await nomineeSummaryAsync.Match(
                onSuccess: _ => Task.CompletedTask,
                onError: async _ =>
                {
                    var nomineeSummary = new NomineeSummaryCreateDto
                    {
                        NomineeId = null,
                        TeamNominationId = nomination.Id,
                        CategoryId = nomination.CategoryId,
                        Location = nomination.Location,
                        TotalNominations = 1
                    };
                    await _nomineeSummaryService.CreateNomineeSummaryAsync(nomineeSummary);
                    _logger.LogInformation("Created NomineeSummary for Team Nomination ID {TeamNominationId}.", nomination.Id);
                }
            );
        }
        
            
    }
}
