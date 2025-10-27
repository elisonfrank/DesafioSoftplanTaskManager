using System.ComponentModel;

namespace Softplan.TaskManager.Dominio.Dto;

public sealed record NewTaskDto(
    [property:DefaultValue("")] string Title, 
    [property:DefaultValue("")] string Description, 
    DateTime Deadline);