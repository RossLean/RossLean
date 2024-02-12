# GenericsAnalyzer
A Roslyn analyzer for C# for generic types and functions

## References
- [Usage](docs/usage.md)
- [Diagnostics](docs/index.md)

## Downloads
### NuGet Packages
- [GenericsAnalyzer](https://www.nuget.org/packages/GenericsAnalyzer) - includes the analyzer, but NOT the core components
- [GenericsAnalyzer.Core](https://www.nuget.org/packages/GenericsAnalyzer.Core) - ONLY includes the core components

## Consumption
Include a reference to the `GenericsAnalyzer` and `GenericsAnalyzer.Core` NuGet packages in the desired projects individually.

It is likely that the installation of the analyzer in a project in Visual Studio may cause its behavior to be fuzzy, so it is recommended to restart Visual Studio.
