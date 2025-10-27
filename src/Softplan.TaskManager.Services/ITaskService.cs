using Softplan.TaskManager.Dominio.Dto;

namespace Softplan.TaskManager.Services;

public interface ITaskService
{
    Task<TaskDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskByUserDto>> GetByUserIdAsync(Guid userId);
    Task<NewTaskResultDto> AddNewTaskAsync(NewTaskDto newTaskDto, string userEmail);
    Task CompleteTaskAsync(Guid id);
    Task DeleteTaskAsync(Guid id);
}