using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NameSorter.Models;
using NameSorter.Services;

namespace NameSorter.Api.Tests;

public class HostConfigurationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public HostConfigurationTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task OpenApiDocument_InDevelopment_IsServed()
    {
        // WebApplicationFactory hosts the app in the Development environment by default.
        var client = this.factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var document = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        document.GetProperty("paths").TryGetProperty("/names/sort", out _).Should().BeTrue();
    }

    [Fact]
    public async Task OpenApiDocument_OutsideDevelopment_IsNotServed()
    {
        var client = this.factory
            .WithWebHostBuilder(builder => builder.UseEnvironment(Environments.Production))
            .CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Anything unexpected thrown while processing is reported as a generic problem-details
    // response rather than leaking the exception, mirroring the CLI's exit code 3.
    [Fact]
    public async Task Post_UnexpectedProcessingError_ReturnsProblemDetails500()
    {
        var client = this.factory
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
                services.AddSingleton<INameSortingProcessor>(new ThrowingProcessor())))
            .CreateClient();

        var response = await client.PostAsJsonAsync("/names/sort", new { names = new[] { "Jane Doe" } });

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Status.Should().Be(500);
        problem.Detail.Should().BeNull();
    }

    private sealed class ThrowingProcessor : INameSortingProcessor
    {
        public NameSortingResult Process(IList<string> rawNames, SortDirection direction = SortDirection.Ascending)
            => throw new InvalidOperationException("boom");
    }
}
