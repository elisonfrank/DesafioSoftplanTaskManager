using Softplan.TaskManager.Dominio.Enums;

namespace Softplan.TaskManager.Dominio.Dto;

public record TaskByUserDto(Guid Id, string Title, string Description, DateTime CreationDate, DateTime Deadline, TaskStatusInfo TaskStatus);