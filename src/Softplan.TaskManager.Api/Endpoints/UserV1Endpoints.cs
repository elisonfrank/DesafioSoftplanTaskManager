using Asp.Versioning.Builder;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Services;
using Softplan.TaskManager.Shared;
using ApiVersion = Asp.Versioning.ApiVersion;

namespace Softplan.TaskManager.Api.Endpoints;

public static class UserV1Endpoints
{
    public static void AddUserEndpoints(this IEndpointRouteBuilder builder, ApiVersionSet versionSet)
    {
        builder.MapPost("api/v{version:apiVersion}/users/login", async (
                ILogger<Program> logger,
                IValidator<LoginDto> validator,
                [FromBody] LoginDto loginDto,
                [FromServices] IUserService userService) =>
            {
                try
                {
                    logger.LogInformation("Validating new login");

                    var validationResult = validator.Validate(loginDto);
                    if (!validationResult.IsValid)
                    {
                        logger.LogWarning("Validation failed with Errors {Errors}", validationResult.Errors);
                        return Results.BadRequest(
                            validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
                    }

                    logger.LogInformation("Login user E-mail {Email}", loginDto.Email.MaskEmail());

                    var result = await userService.LoginAsync(loginDto);
                    if (result is null)
                    {
                        logger.LogWarning("Unauthorized access");
                        return Results.Unauthorized();
                    }

                    return Results.Ok(result);
                }
                catch (Exception)
                {
                    logger.LogError("Fail while login user E-mail {Email}", loginDto.Email.MaskEmail());
                    return Results.Problem("Unexpected error");
                }
            })
            .WithOpenApi()
            .WithName("Login")
            .WithTags("Users")
            .Produces<UnauthorizedResult>()
            .Produces<BadRequestResult>()
            .Produces<LoginResultDto>()
            .Produces<ProblemDetails>()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(new ApiVersion(1, 0));

        builder.MapPost("api/v{version:apiVersion}/users", async (
            ILogger<Program> logger,
            IValidator<NewUserDto> validator,
            [FromBody] NewUserDto newUserDto,
            [FromServices] IUserService userService) =>
        {
            try
            {
                logger.LogInformation("Validating new user object");
                
                var validationResult = validator.Validate(newUserDto);
                if (!validationResult.IsValid)
                {
                    logger.LogWarning("Validation failed with Errors {Errors}", validationResult.Errors);
                    return Results.BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
                }
                
                logger.LogInformation("Posting new user E-mail {Email}", newUserDto.Email.MaskEmail());
                
                var userDto = await userService.AddNewUserAsync(newUserDto);
                return Results.Created("/api/users", userDto);

            }
            catch (Exception)
            {
                logger.LogError("Fail while posting user E-mail {Email}", newUserDto.Email.MaskEmail());
                return Results.Problem("Unexpected error");
            }
        })
        .WithOpenApi()
        .WithName("Users")
        .WithTags("Users")
        .Produces<BadRequestResult>()
        .Produces<Created<UserDto>>()
        .Produces<ProblemDetails>()
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(new ApiVersion(1, 0));
    }
}