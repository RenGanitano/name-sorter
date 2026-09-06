using NameSorter.Models;

namespace NameSorter.Services;

/// <summary>
/// Represents either a parsed name, an ignored blank line, or a validation error.
/// </summary>
public sealed record NameParseResult(PersonName? Name, string? Error)
{
    public bool IsValid => Name is not null;

    public bool IsIgnored => Name is null && Error is null;
}