using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace RossLean.GenericsAnalyzer;

public class TypeParameterAttributeArgumentCorrelationDictionary : Dictionary<ITypeParameterSymbol, ICollection<AttributeArgumentSyntax>>
{
    public void Add(ITypeParameterSymbol typeParameter, AttributeArgumentSyntax argument)
    {
        bool contained = TryGetValue(typeParameter, out var arguments);
        if (!contained)
            Add(typeParameter, arguments = new List<AttributeArgumentSyntax>());
        arguments.Add(argument);
    }
}
