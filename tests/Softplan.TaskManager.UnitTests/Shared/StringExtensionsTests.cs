using FluentAssertions;
using Softplan.TaskManager.Shared;

namespace Softplan.TaskManager.UnitTests.Shared;

public class StringExtensionsTests
{
    [Fact]
    public void MaskEmail_DeveMascararParteDoNome()
    {
        //Arrange
        var email = "usuario@dominio.com";

        //Act
        var result = email.MaskEmail();

        //Assert
        result.Should().Be("u******@dominio.com");
    }
    
    [Fact]
    public void MaskEmail_DeveRetornarMesmoEmail_QuandoNaoConterArroba()
    {
        //Arrange
        var email = "invalido";

        //Act
        var result = email.MaskEmail();

        //Assert
        result.Should().Be("invalido");
    }

    [Fact]
    public void MaskEmail_DeveMascararCorretamente_QuandoNomeTiverUmCaractere()
    {
        //Arrange
        var email = "a@teste.com";

        //Act
        var result = email.MaskEmail();

        //Assert
        result.Should().Be("a@teste.com"); // não há caracteres extras para mascarar
    }

    [Fact]
    public void MaskEmail_DeveMascararCorretamente_QuandoNomeTiverDoisCaracteres()
    {
        //Arrange
        var email = "ab@teste.com";

        //Act
        var result = email.MaskEmail();

        //Assert
        result.Should().Be("a*@teste.com");
    }
}