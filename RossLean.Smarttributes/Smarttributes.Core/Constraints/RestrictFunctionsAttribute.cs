namespace RossLean.Smarttributes.Constraints;

/// <summary>
/// Restricts the kinds of functions that can be marked with the marked attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
[RequiredAttributeTargets(RestrictionKind.Exact, AttributeTargets.Method)]
public class RestrictFunctionsAttribute : Attribute
{
    /// <summary>
    /// The function targets that can be assigned the marked attribute.
    /// </summary>
    public FunctionTargets Targets { get; }

    /// <inheritdoc cref="RestrictFunctionsAttribute"/>
    /// <param name="targets">
    /// The function targets that can be assigned the marked attribute.
    /// </param>
    public RestrictFunctionsAttribute(FunctionTargets targets)
    {
        Targets = targets;
    }
}
