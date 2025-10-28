using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Softplan.TaskManager.Shared;

namespace Softplan.TaskManager.Dominio.Dto;

public sealed record NewUserDto(
    [property: DefaultValue(""), EmailAddress]
    string Email,
    string Password)
{
    [DefaultValue("")] public string Password { get; init; } = !string.IsNullOrEmpty(Password) ? PasswordHasher.HashPassword(Password) : "";
}