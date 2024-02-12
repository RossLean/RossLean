namespace RossLean.Smarttributes.Constraints;

/// <summary>
/// Provides flags for the various function targets, including methods, local methods,
/// lambdas and anonymous methods.
/// </summary>
[Flags]
public enum FunctionTargets
{
    /// <summary>
    /// Represents no function targets.
    /// </summary>
    None = 0,

    /// <summary>
    /// Represents the methods inside a type.
    /// </summary>
    Method = 1 << 0,
    /// <summary>
    /// Represents the local methods.
    /// </summary>
    LocalMethod = 1 << 1,
    /// <summary>
    /// Represents the lambda expression methods.
    /// </summary>
    /// <remarks>
    /// Lambda expression methods began being supported for attribute targets from C# 10 onwards.
    /// </remarks>
    Lambda = 1 << 2,
    /// <summary>
    /// Represents the anonymous methods (declared with the <see langword="delegate"/> operator).
    /// </summary>
    /// <remarks>
    /// Anonymous methods are not supported for attribute targets, as of C# 12.
    /// </remarks>
    AnonymousMethod = 1 << 3,

    /// <summary>
    /// Represents all function targets.
    /// </summary>
    All = Method | LocalMethod | Lambda | AnonymousMethod,
}
