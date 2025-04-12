using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface INomineeSummaryService
{
    Task<ApiResponse<NomineeSummaryResponseDto, string>> GetNomineeSummaryAsync(int nomineeId, int categoryId);
    Task<ApiResponse<IEnumerable<NomineeSummaryResponseDto>, string>> GetAllNomineeSummariesAsync();
    Task<ApiResponse<IEnumerable<NomineeSummaryResponseDto>, string>> GetAllNomineeSummariesByCategoryIdAsync(int categoryId);
    Task<ApiResponse<NomineeSummaryResponseDto, string>> CreateNomineeSummaryAsync(NomineeSummaryCreateDto dto);
    Task<ApiResponse<bool, string>> UpdateNomineeSummaryAsync(int nomineeSummaryId, NomineeSummaryUpdateDto updateDto);
    Task<ApiResponse<bool, string>> UpdateNomineeSummaryTotalNominationCountyAsync(int nomineeId, int categoryId);
    Task<ApiResponse<bool, string>> DeleteNomineeSummaryAsync(int nomineeId, int categoryId);

}

public class NomineeSummaryService : INomineeSummaryService
{
    private readonly INomineeSummaryRepository _repository;
    private readonly ILogger<NomineeSummaryService> _logger;
    private readonly IMapper _mapper;

    public NomineeSummaryService(
        INomineeSummaryRepository repository,
        ILogger<NomineeSummaryService> logger,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<NomineeSummaryResponseDto, string>> CreateNomineeSummaryAsync(NomineeSummaryCreateDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var entity = _mapper.Map<NomineeSummary>(dto);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.AddAsync(entity);
        _logger.LogInformation("Created AwardEvent with ID {Id}.", entity.Id);

        var responseDto = _mapper.Map<NomineeSummaryResponseDto>(entity);
        return responseDto;
    }

    public async Task<ApiResponse<bool, string>> UpdateNomineeSummaryAsync(int nomineeSummaryId, NomineeSummaryUpdateDto updateDto)
    {
        var nomineeSummary = await _repository.GetByIdAsync(nomineeSummaryId);
        if (nomineeSummary == null)
        {
            _logger.LogWarning("NomineeSummary with ID {Id} not found.", nomineeSummaryId);
            return $"NomineeSummary with ID {nomineeSummaryId} not found.";
        }
        
        updateDto.TotalNominations =
            await _repository.CountIndividualNominationsForNomineeAsync(nomineeSummary.NomineeId,
                nomineeSummary.CategoryId);

        _mapper.Map(updateDto, nomineeSummary);
        nomineeSummary.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(nomineeSummary);
        _logger.LogInformation("Updated NomineeSummary with ID {Id}.", nomineeSummary.Id);

        return true;
    }

    public async Task<ApiResponse<bool, string>> UpdateNomineeSummaryTotalNominationCountyAsync(int nomineeId, int categoryId)
    {
        var nomineeSummary = await _repository.GetByNomineeIdAndCategoryIdAsync(nomineeId, categoryId);
        if (nomineeSummary == null)
        {
            _logger.LogWarning("NomineeSummary with NomineeId {NomineeId} and CategoryId {CategoryId} not found.", nomineeId, categoryId);
            return $"NomineeSummary with NomineeId {nomineeId} and CategoryId {categoryId} not found.";
        }
        
        var updateDto = _mapper.Map<NomineeSummaryUpdateDto>(nomineeSummary);
        updateDto.TotalNominations = await _repository.CountIndividualNominationsForNomineeAsync(nomineeId, categoryId);
        
        _mapper.Map(updateDto, nomineeSummary);
        
        await _repository.UpdateAsync(nomineeSummary);
        _logger.LogInformation("Updated NomineeSummary with NomineeId {NomineeId} and CategoryId {CategoryId}.", nomineeId, categoryId);
        
        return true;
        
    }

    public async Task<ApiResponse<NomineeSummaryResponseDto, string>> GetNomineeSummaryAsync(int nomineeId, int categoryId)
    {
        var summary = await _repository.GetByNomineeIdAndCategoryIdAsync(nomineeId, categoryId);
        if (summary == null)
        {
            _logger.LogWarning("NomineeSummary for NomineeId {NomineeId} and CategoryId {CategoryId} not found.",
                nomineeId, categoryId);
            return $"NomineeSummary for NomineeId {nomineeId} and CategoryId {categoryId} not found.";
        }
        
        var responseDto = _mapper.Map<NomineeSummaryResponseDto>(summary);

        return responseDto;
    }

    public async Task<ApiResponse<IEnumerable<NomineeSummaryResponseDto>, string>> GetAllNomineeSummariesAsync()
    {
        var summaries = await _repository.GetAllAsync();
        var responseDtos = _mapper.Map<IEnumerable<NomineeSummaryResponseDto>>(summaries);
        return responseDtos.ToArray();
    }

    public async Task<ApiResponse<IEnumerable<NomineeSummaryResponseDto>, string>> GetAllNomineeSummariesByCategoryIdAsync(int categoryId)
    {
        
        var nominations = await _repository.GetByCategoryIdAsync(categoryId);
        if (!nominations.Any())
        {
            _logger.LogWarning("NomineeSummary for CategoryId {CategoryId} not found.", categoryId);
            return $"NomineeSummary for CategoryId {categoryId} not found.";
        }
        var responseDtos = _mapper.Map<IEnumerable<NomineeSummaryResponseDto>>(nominations);
        return responseDtos.ToArray();
    }

    public async Task<ApiResponse<bool, string>> DeleteNomineeSummaryAsync(int nomineeId, int categoryId)
    {
        var nomineeSummary = await _repository.GetByNomineeIdAndCategoryIdAsync(nomineeId, categoryId);
        if (nomineeSummary == null)
        {
            _logger.LogWarning("NomineeSummary for NomineeId {NomineeId} and CategoryId {CategoryId} not found for deletion.",
                nomineeId, categoryId);
            return $"NomineeSummary for NomineeId {nomineeId} and CategoryId {categoryId} not found for deletion.";
        }

        await _repository.DeleteAsync(nomineeSummary);
        _logger.LogInformation("Deleted NomineeSummary for NomineeId {NomineeId} and CategoryId {CategoryId}.", nomineeId, categoryId);
        
        return true;
    }
}