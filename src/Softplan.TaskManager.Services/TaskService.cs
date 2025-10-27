using Microsoft.Extensions.Logging;
using Softplan.TaskManager.Database.Repositories;
using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Dominio.Enums;

namespace Softplan.TaskManager.Services;

public class TaskService : ITaskService
{
    private readonly ILogger<TaskService> _logger;
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public TaskService(
        ILogger<TaskService> logger,
        ITaskRepository taskRepository, 
        IUserRepository userRepository)
    {
        _logger = logger;
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
        {
            _logger.LogWarning("Task is null");
            return null;
        }

        return new TaskDto(
            task.Id,
            task.Title,
            task.Description,
            task.CreationDate,
            task.Deadline,
            new UserDto(task.User.Id, task.User.Email),
            task.TaskStatus);
    }

    public async Task<IEnumerable<TaskByUserDto>> GetByUserIdAsync(Guid userId)
    {
        var tasks = await _taskRepository.GetByUserIdAsync(userId);
        return tasks.Select(t => new TaskByUserDto(
            t.Id,
            t.Title,
            t.Description,
            t.CreationDate,
            t.Deadline,
            t.TaskStatus));
    }

    public async Task<NewTaskResultDto> AddNewTaskAsync(NewTaskDto newTaskDto, string userEmail)
    {
        var newTask = await _taskRepository.AddAsync(newTaskDto);
        await AttachUserAsync(newTask.Id, userEmail);
        return newTask;
    }

    public async Task CompleteTaskAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is not null && task.TaskStatus == TaskStatusInfo.Pending && task.Deadline >= DateTime.Now)
        {
            _logger.LogInformation("Task can be completed - TaskStatus {taskStatus}", task.TaskStatus);
            await _taskRepository.CompleteTaskAsync(id);
        }
        else
            _logger.LogWarning("Task cannot be completed - TaskStatus {taskStatus}", task?.TaskStatus);
        
    }

    public async Task DeleteTaskAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is not null && task.TaskStatus != TaskStatusInfo.Completed)
        {
            _logger.LogInformation("Task can be deleted - TaskStatus {taskStatus}", task.TaskStatus);
            await _taskRepository.DeleteAsync(id);
        }
        else
            _logger.LogWarning("Task cannot be deleted - TaskStatus {taskStatus}", task?.TaskStatus);
    }

    public async Task AttachUserAsync(Guid taskId, string userEmail)
    {
        var user = await _userRepository.GetByEmailAsync(userEmail);
        if (user is not null)
        {
            var taskEntity = await _taskRepository.GetByIdAsync(taskId);
            if (taskEntity is not null)
            {
                taskEntity.User = user;
                await _taskRepository.UpdateAsync(taskEntity);
            }
        }
    }
}