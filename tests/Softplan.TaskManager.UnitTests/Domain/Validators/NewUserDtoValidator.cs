using FluentAssertions;
using FluentValidation.TestHelper;
using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Dominio.Validators;

namespace Softplan.TaskManager.UnitTests.Domain.Validators;

public class NewUserDtoValidatorTests
{
    private readonly NewUserDtoValidator _validator = new();

    [Fact]
    public void Deve_Falhar_Quando_Email_Estiver_Vazio()
    {
        //Arrange
        var dto = new NewUserDto ("", "123456" );

        //Act
        var result = _validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("E-mail is required");
    }

    [Fact]
    public void Deve_Falhar_Quando_Email_For_Invalido()
    {
        //Arrange
        var dto = new NewUserDto ("invalido", "123456" );

        //Act
        var result = _validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("E-mail invalid");
    }

    [Fact]
    public void Deve_Falhar_Quando_Senha_Estiver_Vazia()
    {
        //Arrange
        var dto = new NewUserDto( "teste@email.com", "" );

        //Act
        var result = _validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Deve_Passar_Quando_EmailESenha_Forem_Validos()
    {
        //Arrange
        var dto = new NewUserDto ("teste@email.com", "123456" );

        //Act
        var result = _validator.TestValidate(dto);

        //Assert
        result.IsValid.Should().BeTrue();
    }
}