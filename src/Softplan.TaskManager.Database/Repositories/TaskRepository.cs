using Microsoft.EntityFrameworkCore;
using Softplan.TaskManager.Database.Context;
using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Dominio.Entidades;
using Softplan.TaskManager.Dominio.Enums;

namespace Softplan.TaskManager.Database.Repositories;

public class TaskRepository : Repository<TaskEntity>, ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public new async Task<TaskEntity?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskEntity>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Tasks
            .Where(t => t.User.Id == userId)
            .ToListAsync();
    }

    public async Task<NewTaskResultDto> AddAsync(NewTaskDto newTaskDto)
    {
        var newTask = new TaskEntity
        {
            Title = newTaskDto.Title,
            Description = newTaskDto.Description,
            Deadline = newTaskDto.Deadline
        };

        await AddAsync(newTask);

        return new NewTaskResultDto(newTask.Id);
    }

    public async Task CompleteTaskAsync(Guid id)
    {
        var task = await GetByIdAsync(id);
        if (task is not null)
        {
            task.TaskStatus = TaskStatusInfo.Completed;
            await UpdateAsync(task);
        }
    }
}