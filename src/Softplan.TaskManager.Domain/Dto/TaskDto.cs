using Softplan.TaskManager.Dominio.Enums;

namespace Softplan.TaskManager.Dominio.Dto;

public record TaskDto(Guid Id, string Title, string Description, DateTime CreationDate, DateTime Deadline, UserDto Owner, TaskStatusInfo TaskStatus);