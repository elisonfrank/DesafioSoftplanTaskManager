using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Softplan.TaskManager.Database.Context;
using Softplan.TaskManager.Database.Repositories;
using Softplan.TaskManager.Dominio.Entidades;

namespace Softplan.TaskManager.UnitTests.Database.Repositories;

public class RepositoryTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
    
    [Fact]
    public async Task AddAsync_DeveAdicionarEntidade()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repo = new Repository<TaskEntity>(context);
        var entity = new TaskEntity { Id = Guid.NewGuid(), Title = "Teste", Description = "Teste Description"};

        // Act
        await repo.AddAsync(entity);
        var result = await repo.GetByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Teste");
    }
    
    [Fact]
    public async Task GetAllAsync_DeveRetornarTodasEntidades()
    {
        //Arrange
        var context = GetInMemoryDbContext();
        var repo = new Repository<TaskEntity>(context);

        await repo.AddAsync(new TaskEntity { Id = Guid.NewGuid(), Title = "Tarefa 1", Description = "Teste Description tarefa 1" });
        await repo.AddAsync(new TaskEntity { Id = Guid.NewGuid(), Title = "Tarefa 2", Description = "Teste Description tarefa 2" });

        //Act
        var result = await repo.GetAllAsync();

        //Assert
        result.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task UpdateAsync_DeveAtualizarEntidade()
    {
        //Arrange
        var context = GetInMemoryDbContext();
        var repo = new Repository<TaskEntity>(context);

        var entity = new TaskEntity { Id = Guid.NewGuid(), Title = "Antigo", Description = "Teste Description Antigo" };
        await repo.AddAsync(entity);

        //Act
        entity.Title = "Atualizado";
        await repo.UpdateAsync(entity);

        //Assert
        var result = await repo.GetByIdAsync(entity.Id);
        result!.Title.Should().Be("Atualizado");
    }
    
    [Fact]
    public async Task DeleteAsync_DeveRemoverEntidade()
    {
        //Arrange
        var context = GetInMemoryDbContext();
        var repo = new Repository<TaskEntity>(context);

        var entity = new TaskEntity { Id = Guid.NewGuid(), Title = "Para deletar", Description = "Teste Description Para deletar" };
        await repo.AddAsync(entity);

        //Act
        await repo.DeleteAsync(entity.Id);
        var result = await repo.GetByIdAsync(entity.Id);

        //Assert
        result.Should().BeNull();
    }
}