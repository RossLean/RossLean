using Microsoft.CodeAnalysis;
using RoseLynn;
using System.Linq;

namespace RossLean.GenericsAnalyzer.Core;

public class GenericTypeConstraintInfo
{
    private readonly TypeConstraintSystem[] systems;

    private GenericTypeConstraintInfo(TypeConstraintSystem[] finalizedSystems)
    {
        systems = finalizedSystems;
    }

    public TypeConstraintSystem this[int index]
    {
        get => systems[index];
        set => systems[index] = value;
    }
    public TypeConstraintSystem this[ITypeParameterSymbol typeParameter]
    {
        get => systems[typeParameter.Ordinal];
        set => systems[typeParameter.Ordinal] = value;
    }

    public class Builder
    {
        private readonly TypeConstraintSystem.Builder[] builders;

        public Builder(ISymbol symbol)
        {
            builders = new TypeConstraintSystem.Builder[symbol.GetArity()];

            var typeParameters = symbol.GetTypeParameters();
            for (int i = 0; i < typeParameters.Length; i++)
                builders[i] = new TypeConstraintSystem.Builder(typeParameters[i]);
        }

        public GenericTypeConstraintInfo FinalizeTypeInfo()
        {
            var finalizedBuilders = builders.Select(b => b.FinalizeSystem()).ToArray();
            return new GenericTypeConstraintInfo(finalizedBuilders);
        }

        public TypeConstraintSystem.Builder this[int index]
        {
            get => builders[index];
            set => builders[index] = value;
        }
        public TypeConstraintSystem.Builder this[ITypeParameterSymbol typeParameter]
        {
            get => builders[typeParameter.Ordinal];
            set => builders[typeParameter.Ordinal] = value;
        }
    }
}
