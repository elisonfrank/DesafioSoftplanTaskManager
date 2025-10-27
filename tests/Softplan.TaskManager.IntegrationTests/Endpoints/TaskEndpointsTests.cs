
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Softplan.TaskManager.Dominio.Dto;

namespace Softplan.TaskManager.IntegrationTests.Endpoints;

public class TaskEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    
    public TaskEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetTasksByUserId_DeveRetornarOk()
    {
        //Arrange
        var userId = Guid.NewGuid();

        //Act
        var response = await _client.GetAsync($"/api/v1/tasks/{userId}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTaskById_DeveRetornarOk()
    {
        //Arrange
        var taskId = Guid.NewGuid();

        //Act
        var response = await _client.GetAsync($"/api/v1/tasks/by-id/{taskId}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostTask_DeveRetornar401_QuandoNaoAutenticado()
    {
        //Arrange
        var newTask = new NewTaskDto
        (
            "Nova tarefa",
            "Descrição",
            DateTime.UtcNow.AddDays(1)
        );

        //Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", newTask);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PutTaskComplete_DeveRetornar401_QuandoNaoAutenticado()
    {
        //Arrange
        var taskId = Guid.NewGuid();

        //Act
        var response = await _client.PutAsync($"/api/v1/tasks/{taskId}/complete", null);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteTask_DeveRetornar401_QuandoNaoAutenticado()
    {
        //Arrange
        var taskId = Guid.NewGuid();

        _client.DefaultRequestHeaders.Authorization = null;
        
        //Act
        var response = await _client.DeleteAsync($"/api/v1/tasks/{taskId}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task PostTask_DeveRetornar201_QuandoAutenticado()
    {
        //Arrange
        var newTask = new NewTaskDto
        (
            "Tarefa autenticada",
            "Descrição",
            DateTime.UtcNow.AddDays(1)
        );
        
        var authClient = _factory.CreateClient();
        authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Fake");

        //Act
        var response = await authClient.PostAsJsonAsync("/api/v1/tasks", newTask);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<NewTaskResultDto>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task PutTaskComplete_DeveRetornar204_QuandoAutenticado()
    {
        //Arrange
        var taskId = Guid.NewGuid();

        var authClient = _factory.CreateClient();
        authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Fake");
        
        //Act
        var response = await authClient.PutAsync($"/api/v1/tasks/{taskId}/complete", null);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task DeleteTask_DeveRetornar204_QuandoAutenticado()
    {
        //Arrange
        var taskId = Guid.NewGuid();

        var authClient = _factory.CreateClient();
        authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Fake");
        
        //Act
        var response = await authClient.DeleteAsync($"/api/v1/tasks/{taskId}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
