using FluentAssertions;
using Moq;
using Softplan.TaskManager.Database.Repositories;
using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Dominio.Entidades;
using Softplan.TaskManager.Services;

namespace Softplan.TaskManager.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();

    private UserService CreateService() => new (_userRepoMock.Object, _tokenServiceMock.Object);
    
    [Fact]
    public async Task GetUserByEmailAsync_DeveRetornarUserDto_QuandoUsuarioExistir()
    {
        //Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "teste@email.com", Password = "123" };
        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var service = CreateService();
        
        //Act
        var result = await service.GetUserByEmailAsync(user.Email);

        //Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
    }
    
    [Fact]
    public async Task GetUserByEmailAsync_DeveRetornarNull_QuandoUsuarioNaoExistir()
    {
        //Arrange
        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var service = CreateService();

        //Act
        var result = await service.GetUserByEmailAsync("naoexiste@email.com");

        //Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task AddNewUserAsync_DeveChamarRepositorioERetornarUserDto()
    {
        //Arrange
        var dto = new NewUserDto("novo@email.com", "senha");
        var expected = new UserDto(Guid.NewGuid(), dto.Email);

        _userRepoMock.Setup(r => r.AddAsync(dto)).ReturnsAsync(expected);

        var service = CreateService();

        //Act
        var result = await service.AddNewUserAsync(dto);

        //Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task LoginAsync_DeveRetornarNull_QuandoUsuarioNaoExistir()
    {
        //Arrange
        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var service = CreateService();

        //Act
        var result = await service.LoginAsync(new LoginDto("email@email.com", "123"));

        //Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task LoginAsync_DeveRetornarNull_QuandoSenhaInvalida()
    {
        //Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "teste@email.com", Password = "123" };
        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var service = CreateService();

        //Act
        var result = await service.LoginAsync(new LoginDto(user.Email, "senhaErrada"));

        //Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task LoginAsync_DeveRetornarLoginResultDto_QuandoCredenciaisValidas()
    {
        //Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "teste@email.com", Password = "123" };
        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var token = "fake-jwt";
        var expiration = DateTime.UtcNow.AddHours(1);
        _tokenServiceMock.Setup(t => t.GenerateToken(user.Email)).Returns((token, expiration));

        var service = CreateService();

        //Act
        var result = await service.LoginAsync(new LoginDto(user.Email, "123"));

        //Assert
        result.Should().NotBeNull();
        result!.Token.Should().Be(token);
        result.ExpiresAt.Should().BeCloseTo(expiration, TimeSpan.FromSeconds(1));
        result.UserInfo.Email.Should().Be(user.Email);
    }
}