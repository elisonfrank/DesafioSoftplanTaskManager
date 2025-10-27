using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Dominio.Entidades;

namespace Softplan.TaskManager.Database.Repositories;

public interface ITaskRepository : IRepository<TaskEntity>
{
    new Task<TaskEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskEntity>> GetByUserIdAsync(Guid userId);
    Task<NewTaskResultDto> AddAsync(NewTaskDto newTaskDto);
    Task CompleteTaskAsync(Guid id);
}