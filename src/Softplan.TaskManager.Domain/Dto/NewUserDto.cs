using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Softplan.TaskManager.Dominio.Dto;

public sealed record NewUserDto(
    [property:DefaultValue(""), EmailAddress] string Email, 
    [property:DefaultValue("")] string Password);