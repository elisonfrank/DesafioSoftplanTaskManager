using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Softplan.TaskManager.Database.Context;
using Softplan.TaskManager.Database.Repositories;
using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Dominio.Entidades;
using Softplan.TaskManager.Dominio.Enums;

namespace Softplan.TaskManager.UnitTests.Database.Repositories;

public class TaskRepositoryTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
    
    [Fact]
    public async Task GetByIdAsync_DeveRetornarTaskComUsuario()
    {
        //Arrange
        var context = GetInMemoryDbContext();
        var user = new User { Id = Guid.NewGuid(), Email = "joao@teste.com.br", Password = "1234"};
        var task = new TaskEntity { Id = Guid.NewGuid(), Title = "Teste", Description = "Teste Description", User = user };

        context.Users.Add(user);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var repo = new TaskRepository(context);

        //Act
        var result = await repo.GetByIdAsync(task.Id);

        //Assert
        result.Should().NotBeNull();
        result!.User.Should().NotBeNull();
        result.User.Email.Should().Be("joao@teste.com.br");
        result.User.Password.Should().Be("1234");
    }
    
    [Fact]
    public async Task GetByUserIdAsync_DeveRetornarTasksDoUsuario()
    {
        //Arrange
        var context = GetInMemoryDbContext();
        var user = new User { Id = Guid.NewGuid(), Email = "maria@teste.com.br", Password = "1234" };
        var task1 = new TaskEntity { Id = Guid.NewGuid(), Title = "Tarefa 1", Description = "Teste Description", User = user };
        var task2 = new TaskEntity { Id = Guid.NewGuid(), Title = "Tarefa 2", Description = "Teste Description", User = user };

        context.Users.Add(user);
        context.Tasks.AddRange(task1, task2);
        await context.SaveChangesAsync();

        var repo = new TaskRepository(context);

        //Act
        var result = await repo.GetByUserIdAsync(user.Id);

        //Assert
        result.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task AddAsync_DeveCriarNovaTaskERetornarId()
    {
        //Arrange
        var context = GetInMemoryDbContext();
        var repo = new TaskRepository(context);

        var dto = new NewTaskDto("Nova tarefa", "Descrição", DateTime.UtcNow.AddDays(1));

        //Act
        var result = await repo.AddAsync(dto);

        //Assert
        result.Id.Should().NotBeEmpty();
        var task = await context.Tasks.FindAsync(result.Id);
        task.Should().NotBeNull();
        task!.Title.Should().Be("Nova tarefa");
    }
    
    
    [Fact]
    public async Task CompleteTaskAsync_DeveAlterarStatusParaCompleted()
    {
        //Arrange
        var context = GetInMemoryDbContext();
        var task = new TaskEntity { Id = Guid.NewGuid(), Title = "Incompleta", Description = "Teste Description" };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var repo = new TaskRepository(context);

        //Act
        await repo.CompleteTaskAsync(task.Id);

        //Assert
        var updated = await context.Tasks.FindAsync(task.Id);
        updated!.TaskStatus.Should().Be(TaskStatusInfo.Completed);
    }
}