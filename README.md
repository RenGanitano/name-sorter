# Name Sorter

[![Coverage](https://renganitano.github.io/name-sorter/badge.svg)](https://renganitano.github.io/name-sorter/)

A command-line application and a minimal web API that sort a list of names by **last name**, then by **given names**.

## Project layout

| Project | Purpose |
| --- | --- |
| `src/NameSorter.Core` | Models, comparers and the parse → sort pipeline shared by both hosts |
| `src/NameSorter.Cli` | The `name-sorter` command-line executable (file in, stdout + file out) |
| `src/NameSorter.Api` | ASP.NET Core minimal API exposing the same pipeline over HTTP |
| `tests/NameSorter.Tests` | Unit and end-to-end tests for Core and the CLI |
| `tests/NameSorter.Api.Tests` | In-process HTTP tests for the API |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or later)

## Build

```bash
dotnet build
```

## Publish

Create a standalone executable you can invoke directly:

```bash
dotnet publish src/NameSorter.Cli/NameSorter.Cli.csproj -c Release -o ./publish
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
dotnet run --project src/NameSorter.Cli -- ./unsorted-names-list.txt
```

```text
Usage: name-sorter [--descending|-d] <file-path>
```

| Option | Effect |
| --- | --- |
| `--descending`, `-d` | Emit the names in reverse order (Z→A by last name). May appear before or after the file path. |

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

French surname particles — `de`, `du`, `des`, `d'`, `le`, `la`, `les`, `l'`, `Saint`,
`Sainte`, `St`, `Ste` (with or without a trailing period) — are treated as part of the
last name and do not count toward the three-given-name limit:

```
Jean Le Blanc                → given "Jean",       last name "Le Blanc"
Louis de la Durantaye        → given "Louis",      last name "de la Durantaye"
Jean Marc de la Durantaye    → given "Jean Marc",  last name "de la Durantaye"
```

Hyphenated names (`Marie-Ève`, `St-Pierre`) are single words and need no special handling.

### Sort order

- Names are compared case-insensitively using French-Canadian (`fr-CA`) collation, so
  accented letters sort beside their unaccented forms: `Cote`, `Côté`, `Cotton`.
- A last name that starts with a particle is sorted as written, under the particle:
  `Le Blanc` is filed under **L** (between `Lavoie` and `Lemieux`), `de la Durantaye`
  under **D**.
- Ties on last name are broken by given names left-to-right; a name with fewer given
  names sorts first (`Adam Smith` before `Adam Bruce Smith`).
- `--descending` reverses the whole ordering.

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

To collect coverage the way CI does (`.runsettings` excludes the OpenAPI package's
source-generated code from the numbers):

```bash
dotnet test --settings .runsettings --collect:"XPlat Code Coverage"
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
| `1` | Invalid arguments (missing file path, unknown option, or more than one file path) |
| `2` | The input file does not exist |
| `3` | An unexpected processing or output error occurred |

## Web API

The same parse → sort pipeline is exposed over HTTP by `src/NameSorter.Api`:

```bash
dotnet run --project src/NameSorter.Api
```

This listens on `http://localhost:5128` (see `Properties/launchSettings.json`). In the
Development environment the OpenAPI document is served at `/openapi/v1.json`.

### `POST /names/sort`

Request body: the names to sort, one per array entry, plus an optional `direction`
(`ascending`, the default, or `descending`):

```bash
curl -s -X POST http://localhost:5128/names/sort \
  -H 'Content-Type: application/json' \
  -d '{"names": ["Janet Parsons", "Adonis Julius Archer", "SingleName", "Marin Alvarez"]}'
```

Response — `200 OK`:

```json
{
  "sortedNames": ["Marin Alvarez", "Adonis Julius Archer", "Janet Parsons"],
  "invalidNames": [
    { "lineNumber": 3, "value": "SingleName", "reason": "expected 2-4 parts, got 1" }
  ]
}
```

- `sortedNames` are formatted exactly as the CLI prints them.
- `invalidNames` mirrors the CLI's stderr warnings; `lineNumber` is the 1-based position in
  the request's `names` array. Blank entries are ignored but still count toward the position.
- The API never writes `sorted-names-list.txt`; the response is the output.

| Status | Meaning |
| ---: | --- |
| `200` | Processed, including when some entries were invalid (see `invalidNames`) |
| `400` | Malformed JSON, missing `names`, or an unknown `direction` (RFC 9457 problem details) |
| `500` | An unexpected processing error occurred |

## Design Decisions

Design and Approach

I opted to split the program to the following main operations: read input, parse, sort and write. There is an interface for each allowing for loose coupling and maintainability, extensibility and testability. ie. Comparers can be extended to enable different sorting rules — `--descending` is implemented by wrapping the default comparer in a `DescendingComparer<T>`, and the collation is an injectable `StringComparer` (defaulting to `fr-CA`) so other cultures can be supported without touching the sort logic.

Culture-aware comparison relies on ICU. If the runtime is switched to invariant globalization mode (`DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1` or `<InvariantGlobalization>true</InvariantGlobalization>`), comparison silently falls back to ordinal and the accent-ordering tests will fail.

For build and test pipeline, opted for Github Actions as it is free and seemed the easiest option in Github. I also included test coverage metrics.

Should handle 1000+ lines of input fine, loads the file contents into memory. For anything more I would look to switching to reading file chunks/pagination.

Result pattern introduced for more verbose error handling

The parse → sort pipeline lives in `NameSorter.Core` behind `INameSortingProcessor`, with no I/O of its own. The CLI adds a file reader and console/file writers around it; the web API binds it to a single endpoint. Both hosts register the pipeline through the same `AddNameSorting()` extension so the two cannot drift.
