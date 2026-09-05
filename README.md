# Name Sorter

A command-line application that sorts a list of names by **last name**, then by **given names**. Built with C# / .NET 10, following SOLID principles.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or later)

## Build

```bash
dotnet build
```

## Run

```bash
dotnet run --project src/NameSorter -- ./unsorted-names-list.txt
```

Or, after publishing:

```bash
name-sorter ./unsorted-names-list.txt
```

### What it does

1. Reads names from the specified file (one name per line).
2. Sorts them by last name, then by given names (left-to-right).
3. Prints the sorted list to **stdout**.
4. Writes the sorted list to `sorted-names-list.txt` in the working directory.

### Input format

Each line contains 1–3 given names followed by a last name, separated by spaces:

```
Janet Parsons
Adonis Julius Archer
Hunter Uriah Mathew Clarke
```

### Example

Given `unsorted-names-list.txt`:

```
Janet Parsons
Vaughn Lewis
Adonis Julius Archer
Shelby Nathan Yoder
Marin Alvarez
London Lindsey
Beau Tristan Bentley
Leo Gardner
Hunter Uriah Mathew Clarke
Mikayla Lopez
Frankie Conner Ritter
```

Running:

```bash
dotnet run --project src/NameSorter -- ./unsorted-names-list.txt
```

Outputs:

```
Marin Alvarez
Adonis Julius Archer
Beau Tristan Bentley
Hunter Uriah Mathew Clarke
Leo Gardner
Vaughn Lewis
London Lindsey
Mikayla Lopez
Janet Parsons
Frankie Conner Ritter
Shelby Nathan Yoder
```

## Test

```bash
dotnet test
```

## Project Structure

```
name-sort/
├── src/NameSorter/           # Console application
│   ├── Program.cs            # Entry point (DI setup)
│   ├── NameSortingApplication.cs  # Workflow orchestrator
│   ├── Models/
│   │   └── PersonName.cs     # Name data model
│   ├── Comparers/
│   │   └── LastNameFirstComparer.cs  # Sort strategy
│   └── Services/
│       ├── INameParser.cs / NameParser.cs
│       ├── INameSorter.cs / NameSorterService.cs
│       ├── INameReader.cs / FileNameReader.cs
│       └── INameWriter.cs / ConsoleNameWriter.cs / FileNameWriter.cs
├── tests/NameSorter.Tests/   # xUnit tests (30 tests)
├── name-sort.sln
└── .github/workflows/build-and-test.yml  # CI pipeline
```

## Design

The application follows **SOLID** principles:

| Principle | Application |
|---|---|
| **Single Responsibility** | Each class has one job: `Program` configures DI, `NameSortingApplication` orchestrates, parser/sorter/reader/writer each handle one concern. |
| **Open/Closed** | Sort strategy is injectable via `IComparer<PersonName>` — new strategies require no existing code changes. |
| **Liskov Substitution** | All `INameWriter` implementations are fully interchangeable. |
| **Interface Segregation** | Focused, single-method interfaces. |
| **Dependency Inversion** | High-level classes depend on abstractions, wired via `Microsoft.Extensions.DependencyInjection`. |
