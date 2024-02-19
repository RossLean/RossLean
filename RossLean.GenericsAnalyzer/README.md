# GenericsAnalyzer
A Roslyn analyzer for empowering the constraint model for generic types and functions in C#. This analyzer also offers quick code fixes on the reported diagnostics regarding the specified constraints.

## References
- [Usage](https://github.com/RossLean/RossLean/blob/master/RossLean.GenericsAnalyzer/docs/usage.md)
- [Diagnostics](https://github.com/RossLean/RossLean/blob/master/RossLean.GenericsAnalyzer/docs/index.md)

## Downloads

### NuGet Packages
- [RossLean.GenericsAnalyzer](https://www.nuget.org/packages/RossLean.GenericsAnalyzer) - includes the analyzer and the core components.
- [RossLean.GenericsAnalyzer.Core](https://www.nuget.org/packages/RossLean.GenericsAnalyzer.Core) - ONLY includes the core components. Using this package alone is not recommended.

### Consumption
Include a reference to the `RossLean.GenericsAnalyzer` NuGet package in the desired projects. This will automatically include a reference to the `RossLean.GenericsAnalyzer.Core` package.

It is likely that the installation of the analyzer in a project in Visual Studio may cause its behavior to be fuzzy, so it is recommended to restart Visual Studio.

The consumption of this analyzer is tested on Visual Studio 2022. The analyzer may be unusable in Visual Studio 2019. Please file any compatibility issues in the [issues of the repository](https://github.com/RossLean/RossLean/issues).
