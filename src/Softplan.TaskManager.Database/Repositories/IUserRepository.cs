using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Dominio.Entidades;

namespace Softplan.TaskManager.Database.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<UserDto> AddAsync(NewUserDto newUserDto);
}