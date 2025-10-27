using System.Security.Claims;
using Asp.Versioning;
using Asp.Versioning.Builder;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Softplan.TaskManager.Dominio.Dto;
using Softplan.TaskManager.Services;

namespace Softplan.TaskManager.Api.Endpoints;

public static class TaskV1Endpoints
{
    public static void AddTaskEndpoints(this IEndpointRouteBuilder builder, ApiVersionSet versionSet)
    {
        builder.MapGet("/api/v{version:apiVersion}/tasks/{userId:Guid}", async (
            ILogger<Program> logger,
            [FromRoute] Guid userId,
            [FromServices] ITaskService taskService) =>
        {
            try
            {
                logger.LogInformation("Retrieving tasks by UserId {Id}", userId);
        
                var tasks = await taskService.GetByUserIdAsync(userId);
                return Results.Ok(tasks);
            }
            catch (Exception)
            {
                logger.LogError("Fail while fetching tasks by UserId {Id}", userId);
                return Results.Problem("Unexpected error");
            }
        })
        .WithOpenApi()
        .WithName("GetTasksByUserId")
        .WithTags("Tasks")
        .Produces<Ok<IEnumerable<TaskByUserDto>>>()
        .Produces<ProblemDetails>()
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(new ApiVersion(1, 0));
        
        builder.MapGet("/api/v{version:apiVersion}/tasks/by-id/{id:Guid}", async (
            ILogger<Program> logger,
            [FromRoute] Guid id,
            [FromServices] ITaskService taskService) =>
        {
            try
            {
                logger.LogInformation("Retrieving task Id {Id}", id);
                
                var task = await taskService.GetByIdAsync(id);
                return Results.Ok(task);
            }
            catch (Exception)
            {
                logger.LogError("Fail while fetching task Id {Id}", id);
                return Results.Problem("Unexpected error");
            }
        })
        .WithOpenApi()
        .WithName("GetTaskById")
        .WithTags("Tasks")
        .Produces<Ok<TaskDto>>()
        .Produces<ProblemDetails>()
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(new ApiVersion(1, 0));

        builder.MapPost("/api/v{version:apiVersion}/tasks", async (
            HttpContext http,
            ILogger<Program> logger,
            IValidator<NewTaskDto> validator,
            [FromBody] NewTaskDto newTaskDto,
            [FromServices] ITaskService taskService,
            [FromServices] IUserService userService) =>
        {
            try
            {
                logger.LogInformation("Validating new task");
                
                var validationResult = validator.Validate(newTaskDto);
                if (!validationResult.IsValid)
                {
                    logger.LogWarning("Validation failed with Errors {Errors}", validationResult.Errors);
                    return Results.BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
                }
                
                logger.LogInformation("Posting task");
                
                var userEmail = http.User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                {
                    logger.LogWarning("Unauthorized access to POST task");
                    return Results.Unauthorized();
                }

                var result = await taskService.AddNewTaskAsync(newTaskDto, userEmail);
                
                logger.LogInformation("Task {Id} created", result.Id);

                return Results.Created("/api/tasks/{id:Guid}", result);
            }
            catch (Exception)
            {
                logger.LogError("Fail while posting task");
                return Results.Problem("Unexpected error");
            }
        })
        .WithOpenApi()
        .WithName("PostTask")
        .WithTags("Tasks")
        .RequireAuthorization()
        .Produces<BadRequestResult>()
        .Produces<UnauthorizedResult>()
        .Produces<Created<NewTaskResultDto>>()
        .Produces<ProblemDetails>()
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(new ApiVersion(1, 0));

        builder.MapPut("/api/v{version:apiVersion}/tasks/{id:Guid}/complete", async (
                HttpContext http,
                ILogger<Program> logger,
                [FromRoute] Guid id,
                [FromServices] ITaskService taskService) =>
        {
            try
            {
                logger.LogInformation("Updating task Id {Id}", id);
                
                var userEmail = http.User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                {
                    logger.LogWarning("Unauthorized access to PUT task");
                    return Results.Unauthorized();
                }
                
                await taskService.CompleteTaskAsync(id);
                
                return Results.NoContent();
            }
            catch (Exception)
            {
                logger.LogError("Fail while updating task Id {Id}", id);
                return Results.Problem("Unexpected error");
            }
        })
        .WithOpenApi()
        .WithName("PutTask")
        .WithTags("Tasks")
        .RequireAuthorization()
        .Produces<UnauthorizedResult>()
        .Produces<NoContentResult>()
        .Produces<ProblemDetails>()
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(new ApiVersion(1, 0));

        builder.MapDelete("/api/v{version:apiVersion}/tasks/{id:Guid}", async (
                HttpContext http,
                ILogger<Program> logger,
                [FromRoute] Guid id,
                [FromServices] ITaskService taskService) =>
        {
            try
            {
                logger.LogInformation("Deleting task Id {Id}", id);
                
                var userEmail = http.User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                {
                    logger.LogWarning("Unauthorized access to DELETE task");
                    return Results.Unauthorized();
                }
                
                await taskService.DeleteTaskAsync(id);
                
                return Results.NoContent();
            }
            catch (Exception)
            {
                logger.LogError("Fail while deleting task Id {Id}", id);
                return Results.Problem("Unexpected error");
            }
        })
        .WithOpenApi()
        .WithName("DeleteTask")
        .WithTags("Tasks")
        .RequireAuthorization()
        .Produces<UnauthorizedResult>()
        .Produces<NoContentResult>()
        .Produces<ProblemDetails>()
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(new ApiVersion(1, 0));
    }
}