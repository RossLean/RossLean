using Microsoft.CodeAnalysis;
using RossLean.Common.Base;
using RossLean.GenericsAnalyzer.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RossLean.GenericsAnalyzer.Core;

/// <summary>Denotes that a generic type argument permits the usage of the specified types.</summary>
[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = true)]
public abstract class ConstrainedTypesAttributeBase : Attribute, IGenericTypeConstraintAttribute
{
    private static readonly InstanceContainer instanceContainer = new();

    private sealed class InstanceContainer : BaseInstanceContainer<ConstrainedTypesAttributeBase>
    {
        public override IEnumerable<Type> TypeSource => TypeSources.AssemblyOfType<ConstrainedTypesAttributeBase>();

        protected override object[] GetDefaultInstanceArguments()
        {
            return [Type.EmptyTypes];
        }
    }

    private readonly Type[] types;

    protected abstract TypeConstraintRule Rule { get; }

    /// <summary>Gets the types that are permitted.</summary>
    public Type[] Types => types.ToArray();

    /// <summary>Initializes a new instance of the <seealso cref="ConstrainedTypesAttributeBase"/> from the given permitted types.</summary>
    /// <param name="constrainedTypes">The types that are constrained accordingly for the marked generic type.</param>
    protected ConstrainedTypesAttributeBase(params Type[] constrainedTypes)
    {
        types = constrainedTypes;
    }

    /// <summary>Gets the constraint rule that the attribute with the given attribute name reflects.</summary>
    /// <param name="attributeTypeName">The name of the attribute whose constraint rule to get.</param>
    /// <returns>The <seealso cref="TypeConstraintRule"/> that is reflected from the attribute with the given name.</returns>
    public static TypeConstraintRule? GetConstraintRuleFromAttribute(AttributeData data)
    {
        var name = data.AttributeClass.Name;
        var rule = instanceContainer.GetDefaultInstance(name)?.Rule;
        if (rule is null)
            return rule;

        var reasonArgument = data.GetNamedArgument(nameof(IReasonedConstraint.Reason));
        var reasonString = reasonArgument?.Value as string;
        return rule.Value with
        {
            Reason = reasonString
        };
    }
}
