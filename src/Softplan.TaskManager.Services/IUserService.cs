using Softplan.TaskManager.Dominio.Dto;

namespace Softplan.TaskManager.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto> AddNewUserAsync(NewUserDto newUserDto);
    Task<LoginResultDto?> LoginAsync(LoginDto loginDto);
}