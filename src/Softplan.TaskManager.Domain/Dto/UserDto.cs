namespace Softplan.TaskManager.Dominio.Dto;

public record UserDto(Guid Id, string Email)
{
    public string Email { get; set; } = Email;
}