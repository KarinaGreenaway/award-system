using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services
{
    public interface IAwardProcessService
    {
        Task<IEnumerable<AwardProcessResponseDto>> GetAllAwardProcessesAsync();
        Task<AwardProcessResponseDto?> GetAwardProcessByIdAsync(int id);
        Task<AwardProcessResponseDto> CreateAwardProcessAsync(AwardProcessCreateDto dto);
        Task<bool> UpdateAwardProcessAsync(int id, AwardProcessUpdateDto dto);
        Task<bool> DeleteAwardProcessAsync(int id);
    }
    public class AwardProcessService : IAwardProcessService
    {
        private readonly IGenericRepository<AwardProcess> _repository;
        private readonly ILogger<AwardProcessService> _logger;

        public AwardProcessService(IGenericRepository<AwardProcess> repository, ILogger<AwardProcessService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<AwardProcessResponseDto>> GetAllAwardProcessesAsync()
        {
            var processes = await _repository.GetAllAsync();
            return processes.Select(process => new AwardProcessResponseDto
            {
                Id = process.Id,
                AwardsName = process.AwardsName,
                StartDate = process.StartDate,
                EndDate = process.EndDate,
                Status = process.Status,
                CreatedAt = process.CreatedAt
            });
        }

        public async Task<AwardProcessResponseDto?> GetAwardProcessByIdAsync(int id)
        {
            var process = await _repository.GetByIdAsync(id);
            if (process == null)
            {
                _logger.LogWarning("AwardProcess with ID {Id} not found.", id);
                return null;
            }

            return new AwardProcessResponseDto
            {
                Id = process.Id,
                AwardsName = process.AwardsName,
                StartDate = process.StartDate,
                EndDate = process.EndDate,
                Status = process.Status,
                CreatedAt = process.CreatedAt
            };
        }

        public async Task<AwardProcessResponseDto> CreateAwardProcessAsync(AwardProcessCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var process = new AwardProcess
            {
                AwardsName = dto.AwardsName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(process);
            _logger.LogInformation("Created AwardProcess with ID {Id}.", process.Id);

            return new AwardProcessResponseDto
            {
                Id = process.Id,
                AwardsName = process.AwardsName,
                StartDate = process.StartDate,
                EndDate = process.EndDate,
                Status = process.Status,
                CreatedAt = process.CreatedAt
            };
        }

        public async Task<bool> UpdateAwardProcessAsync(int id, AwardProcessUpdateDto dto)
        {
            var process = await _repository.GetByIdAsync(id);
            if (process == null)
            {
                _logger.LogWarning("AwardProcess with ID {Id} not found for update.", id);
                return false;
            }

            process.AwardsName = dto.AwardsName;
            process.StartDate = dto.StartDate;
            process.EndDate = dto.EndDate;
            process.Status = dto.Status;
            process.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(process);
            _logger.LogInformation("Updated AwardProcess with ID {Id}.", id);
            return true;
        }

        public async Task<bool> DeleteAwardProcessAsync(int id)
        {
            var process = await _repository.GetByIdAsync(id);
            if (process == null)
            {
                _logger.LogWarning("AwardProcess with ID {Id} not found for deletion.", id);
                return false;
            }

            await _repository.DeleteAsync(process);
            _logger.LogInformation("Deleted AwardProcess with ID {Id}.", id);
            return true;
        }
    }
}
