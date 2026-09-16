using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using NameSorter.Api.Contracts;
using NameSorter.DependencyInjection;
using NameSorter.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNameSorting();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));

var app = builder.Build();

// In Development, minimal APIs throw on unreadable request bodies (ThrowOnBadRequest) instead
// of writing a 400 directly; keep the status the exception carries rather than turning it into a 500.
app.UseExceptionHandler(new ExceptionHandlerOptions
{
    StatusCodeSelector = exception => exception is BadHttpRequestException badRequest
        ? badRequest.StatusCode
        : StatusCodes.Status500InternalServerError,
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/names/sort", Results<Ok<SortNamesResponse>, ValidationProblem> (
    SortNamesRequest request,
    INameSortingProcessor processor) =>
{
    if (request.Names is null)
    {
        return TypedResults.ValidationProblem(new Dictionary<string, string[]>
        {
            ["names"] = ["The 'names' field is required."],
        });
    }

    var result = processor.Process(request.Names.ToList(), request.Direction);

    return TypedResults.Ok(new SortNamesResponse(
        result.SortedNames.Select(name => name.ToString()).ToList(),
        result.InvalidNames));
})
.WithName("SortNames");

app.Run();

// Exposes the implicit Program class so WebApplicationFactory<Program> can host the app in tests.
public partial class Program;
