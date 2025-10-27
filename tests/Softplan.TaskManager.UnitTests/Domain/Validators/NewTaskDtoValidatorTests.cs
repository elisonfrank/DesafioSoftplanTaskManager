using FluentAssertions;
using FluentValidation.TestHelper;
using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Dominio.Validators;

namespace Softplan.TaskManager.UnitTests.Domain.Validators;

public class NewTaskDtoValidatorTests
{
    private readonly NewTaskDtoValidator _validator = new();
    
    [Fact]
    public void Deve_Falhar_Quando_Title_Estiver_Vazio()
    {
        //Arrange
        var dto = new NewTaskDto ( "", "desc", DateTime.UtcNow );

        //Act
        var result = _validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }
    
    [Fact]
    public void Deve_Falhar_Quando_Title_For_MuitoLongo()
    {
        //Arrange
        var dto = new NewTaskDto (new string('a', 101), "desc", DateTime.UtcNow );

        //Act
        var result = _validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title must not exceed 100 characters");
    }

    [Fact]
    public void Deve_Falhar_Quando_Description_Estiver_Vazia()
    {
        //Arrange
        var dto = new NewTaskDto ( "Tarefa", "", DateTime.UtcNow );

        //Act
        var result = _validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description is required");
    }

    [Fact]
    public void Deve_Falhar_Quando_Description_For_MuitoLonga()
    {
        //Arrange
        var dto = new NewTaskDto ( "Tarefa", new string('b', 251), DateTime.UtcNow );

        //Act
        var result = _validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 250 characters");
    }

    [Fact]
    public void Deve_Falhar_Quando_Deadline_For_Default()
    {
        //Arrange
        var dto = new NewTaskDto ("Tarefa", "desc", default );

        //Act
        var result = _validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Deadline)
            .WithErrorMessage("Deadline is required");
    }

    [Fact]
    public void Deve_Passar_Quando_Dados_Forem_Validos()
    {
        //Arrange
        var dto = new NewTaskDto
        (
            "Tarefa válida",
            "Descrição válida",
            DateTime.UtcNow.AddDays(1)
        );

        //Act
        var result = _validator.TestValidate(dto);

        //Assert
        result.IsValid.Should().BeTrue();
    }
}