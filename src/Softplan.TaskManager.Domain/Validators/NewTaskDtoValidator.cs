using FluentValidation;
using Softplan.TaskManager.Dominio.Dto;

namespace Softplan.TaskManager.Dominio.Validators;

public class NewTaskDtoValidator : AbstractValidator<NewTaskDto>
{
    public NewTaskDtoValidator()
    {
        RuleFor(t => t.Title)
            .NotEmpty().WithMessage("Title is required")
            .NotNull().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");
        
        RuleFor(t => t.Description)
            .NotEmpty().WithMessage("Description is required")
            .NotNull().WithMessage("Description is required")
            .MaximumLength(250).WithMessage("Description must not exceed 250 characters");

        RuleFor(t => t.Deadline)
            .NotEmpty().WithMessage("Deadline is required")
            .NotNull().WithMessage("Deadline is required");
    }
}