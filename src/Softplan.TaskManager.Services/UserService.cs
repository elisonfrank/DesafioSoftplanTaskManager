using Softplan.TaskManager.Database.Repositories;
using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Shared;

namespace Softplan.TaskManager.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }
    
    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user is not null ? new UserDto(user.Id, user.Email) : null;
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("Mais de um registro encontrado");
            throw;
        }
    }

    public async Task<UserDto> AddNewUserAsync(NewUserDto newUserDto)
    {
        var userDto = await _userRepository.AddAsync(newUserDto);
        userDto.Email = userDto.Email.MaskEmail();
        return userDto;
    }

    public async Task<LoginResultDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);

        if (user is null) return null;
        var loginValid = PasswordHasher.VerifyPassword(loginDto.Password, user.Password);
            
        if (!loginValid) return null;
            
        var (tokenJwt, expiration) = _tokenService.GenerateToken(user.Email);
        return new LoginResultDto(tokenJwt, expiration, new UserDto(user.Id, user.Email.MaskEmail()));
    }
}