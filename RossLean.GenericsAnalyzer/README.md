# GenericsAnalyzer
A Roslyn analyzer for C# for generic types and functions

## References
- [Usage](docs/usage.md)
- [Diagnostics](docs/index.md)

## Downloads
### NuGet Packages
- [GenericsAnalyzer](https://www.nuget.org/packages/GenericsAnalyzer) - includes the analyzer, but NOT the core components
- [GenericsAnalyzer.Core](https://www.nuget.org/packages/GenericsAnalyzer.Core) - ONLY includes the core components

### Extensions
**NOTE**: The VSIX was discontinued for the following reasons:
- VSIX releases of an analyzer do not cause compilations to fail despite displaying errors.
- The analyzer should not be exclusive to a single IDE as an extension, but available to other platforms as a NuGet package.

A VSIX is in the (very distant future) plans for more user-friendly consumption of the feature, by offering some UI to handle the system. Likewise, a Rider extension might be developed as well.

## Consumption
Include a reference to the `GenericsAnalyzer` and `GenericsAnalyzer.Core` NuGet packages in the desired projects individually.

It is likely that the installation of the analyzer in a project in Visual Studio may cause its behavior to be fuzzy, so it is recommended to restart Visual Studio.
