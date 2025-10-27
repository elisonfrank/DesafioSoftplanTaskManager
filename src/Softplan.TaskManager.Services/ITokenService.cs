namespace Softplan.TaskManager.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(string email);
}