# NameOn
A Roslyn analyzer providing the ability to control usage of nameof

## Disclaimer
The project is still in its early development stage. Some desired features are not yet implemented, bugs may exist, etc.

## References
- [Usage](docs/usage.md)
- [Diagnostics](docs/index.md)

## Downloads
### NuGet Packages
- [NameOn](https://www.nuget.org/packages/NameOn) - includes the analyzer, but NOT the core components
- [NameOn.Core](https://www.nuget.org/packages/NameOn.Core) - ONLY includes the core components

## Consumption
Include a reference to the `NameOn` and `NameOn.Core` NuGet packages in the desired projects individually.

It is likely that the installation of the analyzer in a project in Visual Studio may cause its behavior to be fuzzy, so it is recommended to restart Visual Studio.
