namespace RossLean.Smarttributes.Constraints;

/// <summary>
/// Denotes that the marked attribute imposes a requirement on the attributed symbol
/// about the presence of other defined attributes.
/// <br/>
/// For example, consider the attribute A that has a RequiresPresence with the attributes
/// B and C, that are both attributes. If a type T is applied the attribute A,
/// it is then required that T is also applied the attributes B and C.
/// </summary>
[RestrictToAttributes]
[AttributeUsage(Targets, AllowMultiple = true, Inherited = true)]
public class RestrictTypesAttribute : Attribute
{
    protected const AttributeTargets Targets =
        AttributeTargets.Class | AttributeTargets.Interface;

    public virtual Type[] Types { get; }

    /// <inheritdoc cref="RestrictTypesAttribute"/>
    /// <param name="types">
    /// The types that are required to be inherited by the attributed type.
    /// </param>
    /// <remarks>
    /// <i>Open generic types are supported.</i>
    /// </remarks>
    public RestrictTypesAttribute(params Type[] types)
    {
        Types = types;
    }
}

/// <inheritdoc cref="RestrictTypesAttribute"/>
/// <typeparam name="T">The attribute type that is required to be present.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RestrictTypesAttribute<T> : RestrictTypesAttribute
    where T : class
{
    private static readonly Type[] types = new[] { typeof(T) };

    public override Type[] Types => types;
}

/// <inheritdoc cref="RestrictTypesAttribute"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RestrictTypesAttribute<T1, T2> : RestrictTypesAttribute
    where T1 : class
    where T2 : class
{
    private static readonly Type[] types = new[] { typeof(T1), typeof(T2) };

    public override Type[] Types => types;
}

/// <inheritdoc cref="RestrictTypesAttribute"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RestrictTypesAttribute<T1, T2, T3> : RestrictTypesAttribute
    where T1 : class
    where T2 : class
    where T3 : class
{
    private static readonly Type[] types = new[] { typeof(T1), typeof(T2), typeof(T3) };

    public override Type[] Types => types;
}

/// <inheritdoc cref="RestrictTypesAttribute"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RestrictTypesAttribute<T1, T2, T3, T4> : RestrictTypesAttribute
    where T1 : class
    where T2 : class
    where T3 : class
    where T4 : class
{
    private static readonly Type[] types = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };

    public override Type[] Types => types;
}

[RestrictToAttributes]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RestrictToAttributesAttribute : RestrictTypesAttribute<Attribute> { }

[RestrictToAttributes]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RestrictToExceptionsAttribute : RestrictTypesAttribute<Exception> { }