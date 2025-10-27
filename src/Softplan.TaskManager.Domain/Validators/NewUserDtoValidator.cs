using FluentValidation;
using Softplan.TaskManager.Dominio.Dto;

namespace Softplan.TaskManager.Dominio.Validators;

public class NewUserDtoValidator : AbstractValidator<NewUserDto>
{
    public NewUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail is required")
            .EmailAddress().WithMessage("E-mail invalid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .NotNull().WithMessage("Password is required");
    }
}