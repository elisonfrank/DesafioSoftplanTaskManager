using Microsoft.EntityFrameworkCore;
using Softplan.TaskManager.Database.Context;
using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Dominio.Entidades;

namespace Softplan.TaskManager.Database.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Where(u => u.Email == email)
            .SingleOrDefaultAsync();
    }
    
    public async Task<UserDto> AddAsync(NewUserDto newUserDto)
    {
        var newUser = new User
        {
            Email = newUserDto.Email,
            Password = newUserDto.Password
        };

        await AddAsync(newUser);

        return new UserDto
        (
            newUser.Id,
            newUser.Email
        );
    }
}