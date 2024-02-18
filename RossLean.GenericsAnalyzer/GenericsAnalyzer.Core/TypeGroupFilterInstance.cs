using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace RossLean.GenericsAnalyzer.Core;

// Aliases like that are great

// Resolving ambiguity with Garyon.Reflection.TypeKind
public sealed record TypeGroupFilterInstance(
    ITypeGroupFilterIdentifier Identifier, FilterType FilterType);
