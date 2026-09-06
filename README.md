# Name Sorter

[![Coverage](https://renganitano.github.io/name-sorter/badge.svg)](https://renganitano.github.io/name-sorter/)

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
5. Reports invalid non-empty lines to **stderr** and continues processing valid names.

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

Running:

```bash
dotnet run --project src/NameSorter -- ./unsorted-names-list.txt
```

Invalid lines are skipped with a line number and reason. A summary is written to
stderr after processing, for example:

```text
Warning: Line 3 ("SingleName") skipped: expected 2-4 parts, got 1.
Processed 10 valid name(s); skipped 1 invalid line(s).
```

The process exit codes are:

| Code | Meaning |
| ---: | --- |
| `0` | Input was processed successfully, including when invalid lines were skipped |
| `1` | No input path was supplied |
| `2` | The input file does not exist |
| `3` | An unexpected processing or output error occurred |

## Design Decisions

Design and Approach

I opted to split the program to the following main operations: read input, parse, sort and write. There is an interface for each allowing for loose coupling and maintainability, extensability and testability. ie. Comparers can be extended to enable different sorting rules.

For build and test pipeline, opted for Github Actions as it is free and seemed the easiest option in Github. I also included test coverage metrics.

Should handle 1000+ lines of input fine, loads the file contents into memory. For anything more I would look to switching to reading file chunks/pagination.

Result pattern introduced for more verbose error handling
