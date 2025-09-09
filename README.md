# C# Version

## Features
- Loads local `users.csv` in this `csharp/` directory
- Filtering users by minimum age (default 30)
- Counting users by country
- Calculating average age (1 decimal)
- Top N oldest users (default 3)
- Region aggregation (Europe, North America, South America, Asia, Oceania, Other)

## Prerequisites
- .NET SDK 8.0+

Check version:
```pwsh
dotnet --version
```

## Build & Run
From this `csharp` directory:
```pwsh
# Run (summary default)
dotnet run

# Explicit operations
dotnet run -- summary
dotnet run -- filter
dotnet run -- group
dotnet run -- avg
dotnet run -- top
dotnet run -- region
```

## Publish (optional)
```pwsh
dotnet publish -c Release -o out
./out/csharp.exe summary  # on Windows PowerShell
```

## Notes
- Pure BCL; no external NuGet packages.
- CSV is parsed with simple string splitting; robust enough for the provided dataset.
- Adjust `Program.cs` for more advanced CSV parsing if needed.
