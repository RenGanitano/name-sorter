using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using NameSorter.Api.Contracts;

namespace NameSorter.Api.Tests;

public class SortNamesEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Route = "/names/sort";

    private static readonly string[] UnsortedNames =
    [
        "Janet Parsons",
        "Vaughn Lewis",
        "Adonis Julius Archer",
        "Shelby Nathan Yoder",
        "Marin Alvarez",
        "London Lindsey",
        "Beau Tristan Bentley",
        "Leo Gardner",
        "Hunter Uriah Mathew Clarke",
        "Mikayla Lopez",
        "Frankie Conner Ritter",
    ];

    private static readonly string[] SortedNames =
    [
        "Marin Alvarez",
        "Adonis Julius Archer",
        "Beau Tristan Bentley",
        "Hunter Uriah Mathew Clarke",
        "Leo Gardner",
        "Vaughn Lewis",
        "London Lindsey",
        "Mikayla Lopez",
        "Janet Parsons",
        "Frankie Conner Ritter",
        "Shelby Nathan Yoder",
    ];

    private readonly HttpClient client;

    public SortNamesEndpointTests(WebApplicationFactory<Program> factory)
    {
        this.client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_ValidNames_ReturnsNamesSortedByLastNameThenGivenNames()
    {
        var response = await this.client.PostAsJsonAsync(Route, new { names = UnsortedNames });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<SortNamesResponse>();
        body!.SortedNames.Should().Equal(SortedNames);
        body.InvalidNames.Should().BeEmpty();
    }

    [Fact]
    public async Task Post_DescendingDirection_ReturnsReverseOrder()
    {
        var response = await this.client.PostAsJsonAsync(Route, new { names = UnsortedNames, direction = "descending" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<SortNamesResponse>();
        body!.SortedNames.Should().Equal(SortedNames.Reverse());
    }

    [Fact]
    public async Task Post_UnknownDirection_ReturnsBadRequest()
    {
        var response = await this.client.PostAsJsonAsync(Route, new { names = UnsortedNames, direction = "sideways" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Blank entries are skipped but still count toward the reported position, matching the
    // CLI's file line numbers.
    [Fact]
    public async Task Post_InvalidAndBlankEntries_ReportsInvalidEntriesWithTruePosition()
    {
        var response = await this.client.PostAsJsonAsync(
            Route,
            new { names = new[] { "Janet Parsons", "", "InvalidSingleName", "Marin Alvarez" } });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<SortNamesResponse>();
        body!.SortedNames.Should().Equal("Marin Alvarez", "Janet Parsons");
        body.InvalidNames.Should().ContainSingle()
            .Which.Should().Be(new Models.InvalidName(3, "InvalidSingleName", "expected 2-4 parts, got 1"));
    }

    [Fact]
    public async Task Post_ResponseUsesCamelCaseJson()
    {
        var response = await this.client.PostAsJsonAsync(Route, new { names = new[] { "Bad" } });

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        json.RootElement.TryGetProperty("sortedNames", out _).Should().BeTrue();
        var invalid = json.RootElement.GetProperty("invalidNames")[0];
        invalid.GetProperty("lineNumber").GetInt32().Should().Be(1);
        invalid.GetProperty("value").GetString().Should().Be("Bad");
        invalid.GetProperty("reason").GetString().Should().Be("expected 2-4 parts, got 1");
    }

    [Fact]
    public async Task Post_EmptyNames_ReturnsEmptyResult()
    {
        var response = await this.client.PostAsJsonAsync(Route, new { names = Array.Empty<string>() });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<SortNamesResponse>();
        body!.SortedNames.Should().BeEmpty();
        body.InvalidNames.Should().BeEmpty();
    }

    [Fact]
    public async Task Post_MissingNames_ReturnsValidationProblem()
    {
        var response = await this.client.PostAsJsonAsync(Route, new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("names");
    }

    [Fact]
    public async Task Post_MalformedJson_ReturnsBadRequest()
    {
        using var content = new StringContent("{ not json", Encoding.UTF8, "application/json");

        var response = await this.client.PostAsync(Route, content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
