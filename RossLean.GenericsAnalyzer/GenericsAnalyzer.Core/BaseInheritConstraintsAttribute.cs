using System;

namespace RossLean.GenericsAnalyzer.Core;

[AttributeUsage(AttributeTargets.GenericParameter, AllowMultiple = false)]
public abstract class BaseInheritConstraintsAttribute : Attribute, IGenericTypeConstraintAttribute
{
}
