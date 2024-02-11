namespace RossLean.Smarttributes.Constraints;

/// <summary>
/// Denotes that the marked attribute imposes a requirement on the attributed symbol
/// about the presence of other defined attributes.
/// <br/>
/// For example, consider the attribute A that has a RequiresPresence with the attributes
/// B and C, that are both attributes. If a type T is applied the attribute A,
/// it is then required that T is also applied the attributes B and C.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequiresPresenceAttribute : Attribute
{
    public virtual Type[] Types { get; }

    /// <inheritdoc cref="RequiresPresenceAttribute"/>
    /// <param name="types">
    /// The attribute types that are required to be present on the target attributed symbol.
    /// </param>
    /// <remarks>
    /// <i>Open generic attribute types are not supported.</i>
    /// </remarks>
    public RequiresPresenceAttribute(params Type[] types)
    {
        Types = types;
    }
}

/// <inheritdoc cref="RequiresPresenceAttribute"/>
/// <typeparam name="T">The attribute type that is required to be present.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequiresPresenceAttribute<T> : RequiresPresenceAttribute
    where T : Attribute
{
    private static readonly Type[] types = new[] { typeof(T) };

    public override Type[] Types => types;
}

/// <inheritdoc cref="RequiresPresenceAttribute"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequiresPresenceAttribute<T1, T2> : RequiresPresenceAttribute
    where T1 : Attribute
    where T2 : Attribute
{
    private static readonly Type[] types = new[] { typeof(T1), typeof(T2) };

    public override Type[] Types => types;
}

/// <inheritdoc cref="RequiresPresenceAttribute"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequiresPresenceAttribute<T1, T2, T3> : RequiresPresenceAttribute
    where T1 : Attribute
    where T2 : Attribute
    where T3 : Attribute
{
    private static readonly Type[] types = new[] { typeof(T1), typeof(T2), typeof(T3) };

    public override Type[] Types => types;
}

/// <inheritdoc cref="RequiresPresenceAttribute"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequiresPresenceAttribute<T1, T2, T3, T4> : RequiresPresenceAttribute
    where T1 : Attribute
    where T2 : Attribute
    where T3 : Attribute
    where T4 : Attribute
{
    private static readonly Type[] types = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };

    public override Type[] Types => types;
}

// Any more than 4 could become ugly
