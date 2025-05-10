using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface IUserService
{
    Task<ApiResponse<IEnumerable<UserResponseDto>, string>> GetAllUsersAsync();
}

public class UserService : IUserService
{
    private readonly IGenericRepository<Users> _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IGenericRepository<Users> userRepository, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<UserResponseDto>, string>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);
        return dtos.ToArray();
    }
}