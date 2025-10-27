using Softplan.TaskManager.Dominio.Enums;

namespace Softplan.TaskManager.Dominio.Entidades;

public class TaskEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime Deadline { get; set; }
    public Guid? UserId { get; set; }
    public User User { get; set; }
    public TaskStatusInfo TaskStatus { get; set; } = TaskStatusInfo.Pending;
}