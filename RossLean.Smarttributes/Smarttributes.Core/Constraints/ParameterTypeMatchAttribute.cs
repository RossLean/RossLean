namespace RossLean.Smarttributes.Constraints;

/// <summary>
/// Marks the specified attribute parameter of type <see langword="object"/>[]
/// that it reflects the arguments passed onto a method. Furthermore, the array
/// passed onto this parameter must contain objects that match the types of the
/// parameters of the denoted method.
/// <br/>
/// For example, a method M(int, string, bool) is applied the attribute A that
/// contains one single parameter in its constructor, the parameter object[] P,
/// that has the ParameterTypeMatchAttribute.
/// This means that the object[] passed onto the parameter P must contain exactly
/// 3 objects, which are an int, a string and a bool in the same order they appear
/// in the method's signature. Therefore, passing { 1, "hello", false } would be
/// valid, but passing { "hello", 1, false, null, "extra" } would not be valid.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class ParameterTypeMatchAttribute : Attribute { }
