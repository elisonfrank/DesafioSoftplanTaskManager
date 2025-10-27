using System.ComponentModel.DataAnnotations.Schema;

namespace Softplan.TaskManager.Dominio.Entidades;

[Table("Users")]
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public ICollection<TaskEntity>? Tasks { get; set; }
}