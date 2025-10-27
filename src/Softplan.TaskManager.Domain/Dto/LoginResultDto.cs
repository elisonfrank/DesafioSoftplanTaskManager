namespace Softplan.TaskManager.Dominio.Dto;

public record LoginResultDto(string Token, DateTime ExpiresAt, UserDto UserInfo);