# Name Sorter

A command-line application that sorts a list of names by **last name**, then by **given names**.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or later)

## Build

```bash
dotnet build
```

## Publish

Create a standalone executable you can invoke directly:

```bash
dotnet publish src/NameSorter/NameSorter.csproj -c Release -o ./publish
```

Then run it either directly from the publish directory:

```bash
./publish/name-sorter ./unsorted-names-list.txt
```

Or add the publish directory to your `PATH`:

```bash
export PATH="$PATH:$PWD/publish"
name-sorter ./unsorted-names-list.txt
```

If you want to install it system-wide on macOS:

```bash
sudo cp ./publish/name-sorter /usr/local/bin/name-sorter
```

## Run

```bash
dotnet run --project src/NameSorter -- ./unsorted-names-list.txt
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
name-sorter/
├── src/NameSorter/                 # Console application
│   ├── Program.cs                  # Entry point and DI setup
│   ├── NameSortingApplication.cs   # Workflow orchestrator
│   ├── Comparers/
│   │   └── LastNameFirstComparer.cs
│   ├── Models/
│   │   └── PersonName.cs
│   └── Services/
│       ├── ConsoleNameWriter.cs
│       ├── FileNameReader.cs
│       ├── FileNameWriter.cs
│       ├── INameParser.cs
│       ├── INameReader.cs
│       ├── INameSorter.cs
│       ├── INameWriter.cs
│       ├── NameParser.cs
│       └── NameSorterService.cs
├── tests/NameSorter.Tests/         # xUnit tests (34 tests)
├── name-sorter.slnx                # solution file
├── unsorted-names-list.txt         # sample input
├── sorted-names-list.txt           # generated output
└── README.md
```

