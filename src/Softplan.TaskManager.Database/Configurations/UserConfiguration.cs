using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Softplan.TaskManager.Dominio.Entidades;

namespace Softplan.TaskManager.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        
        builder.Property(u => u.Email).IsRequired();
        builder.Property(u => u.Password).IsRequired();
    }
}