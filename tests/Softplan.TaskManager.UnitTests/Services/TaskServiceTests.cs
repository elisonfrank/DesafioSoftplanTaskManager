using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Softplan.TaskManager.Database.Repositories;
using Softplan.TaskManager.Dominio.Entidades;
using Softplan.TaskManager.Dominio.Enums;
using Softplan.TaskManager.Services;

namespace Softplan.TaskManager.UnitTests.Services;

public class TaskServiceTests
{
    private readonly Mock<ILogger<TaskService>> _loggerMock = new();
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();

    private TaskService CreateService() => new (_loggerMock.Object, _taskRepoMock.Object, _userRepoMock.Object);
    
    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoTaskNaoExistir()
    {
        //Arrange
        _taskRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((TaskEntity?)null);

        var service = CreateService();

        //Act
        var result = await service.GetByIdAsync(Guid.NewGuid());

        //Assert
        result.Should().BeNull();
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
    
    [Fact]
    public async Task GetByIdAsync_DeveRetornarTaskDto_QuandoTaskExistir()
    {
        //Arrange
        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Title = "Teste",
            Description = "Desc",
            Deadline = DateTime.UtcNow.AddDays(1),
            User = new User { Id = Guid.NewGuid(), Email = "user@email.com" }
        };

        _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var service = CreateService();

        //Act
        var result = await service.GetByIdAsync(task.Id);

        //Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Teste");
        result.Owner.Email.Should().Be(task.User.Email);
    }
    
    [Fact]
    public async Task CompleteTaskAsync_DeveChamarCompleteTask_QuandoTaskPendenteDentroDoPrazo()
    {
        //Arrange
        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            TaskStatus = TaskStatusInfo.Pending,
            Deadline = DateTime.UtcNow.AddDays(1)
        };

        _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var service = CreateService();

        //Act
        await service.CompleteTaskAsync(task.Id);

        //Assert
        _taskRepoMock.Verify(r => r.CompleteTaskAsync(task.Id), Times.Once);
    }
    
    [Fact]
    public async Task CompleteTaskAsync_NaoDeveChamarCompleteTask_QuandoTaskExpirada()
    {
        //Arrange
        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            TaskStatus = TaskStatusInfo.Pending,
            Deadline = DateTime.UtcNow.AddDays(-1)
        };

        _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var service = CreateService();

        //Act
        await service.CompleteTaskAsync(task.Id);

        //Assert
        _taskRepoMock.Verify(r => r.CompleteTaskAsync(It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task DeleteTaskAsync_DeveChamarDelete_QuandoTaskNaoConcluida()
    {
        //Arrange
        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            TaskStatus = TaskStatusInfo.Pending
        };

        _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var service = CreateService();

        //Act
        await service.DeleteTaskAsync(task.Id);

        //Assert
        _taskRepoMock.Verify(r => r.DeleteAsync(task.Id), Times.Once);
    }
    
    [Fact]
    public async Task DeleteTaskAsync_NaoDeveChamarDelete_QuandoTaskConcluida()
    {
        //Arrange
        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            TaskStatus = TaskStatusInfo.Completed
        };

        _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var service = CreateService();

        //Act
        await service.DeleteTaskAsync(task.Id);

        //Assert
        _taskRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task AttachUserAsync_DeveAtualizarTaskComUsuario()
    {
        //Arrange
        var taskId = Guid.NewGuid();
        var user = new User { Id = Guid.NewGuid(), Email = "teste@email.com" };
        var task = new TaskEntity { Id = taskId };

        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _taskRepoMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

        var service = CreateService();

        //Act
        await service.AttachUserAsync(taskId, user.Email);

        //Assert
        _taskRepoMock.Verify(r => r.UpdateAsync(It.Is<TaskEntity>(t => t.User == user)), Times.Once);
    }
}