using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Softplan.TaskManager.Database.Context;
using Softplan.TaskManager.Database.Repositories;
using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Dominio.Entidades;

namespace Softplan.TaskManager.UnitTests.Database.Repositories;

public class UserRepositoryTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new AppDbContext(options);
    }
    
    [Fact]
    public async Task GetByEmailAsync_DeveRetornarUsuario_QuandoEmailExistir()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User { Id = Guid.NewGuid(), Email = "teste@email.com", Password = "123456" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context);

        // Act
        var result = await repo.GetByEmailAsync("teste@email.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("teste@email.com");
    }
    
    [Fact]
    public async Task GetByEmailAsync_DeveRetornarNull_QuandoEmailNaoExistir()
    {
        //Arrange
        var context = GetInMemoryDbContext();
        var repo = new UserRepository(context);

        //Act
        var result = await repo.GetByEmailAsync("naoexiste@email.com");

        //Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task AddAsync_DeveCriarNovoUsuarioERetornarUserDto()
    {
        //Arrange
        var context = GetInMemoryDbContext();
        var repo = new UserRepository(context);

        var dto = new NewUserDto("novo@email.com", "senha123");

        //Act
        var result = await repo.AddAsync(dto);

        //Assert
        result.Id.Should().NotBeEmpty();
        result.Email.Should().Be("novo@email.com");

        var user = await context.Users.FindAsync(result.Id);
        user.Should().NotBeNull();
        user!.Email.Should().Be("novo@email.com");
    }
}