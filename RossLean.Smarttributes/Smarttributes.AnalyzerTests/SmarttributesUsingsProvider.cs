using RoseLynn;

namespace RossLean.Smarttributes.AnalyzerTests;

public sealed class SmarttributesUsingsProvider : UsingsProviderBase
{
    public static readonly SmarttributesUsingsProvider Instance = new();

    public const string DefaultUsings =
        """
        using RossLean.Smarttributes;
        using RossLean.Smarttributes.Constraints;
        using System;
        using System.Collections;
        using System.Collections.Generic;
        """;

    public override string DefaultNecessaryUsings => DefaultUsings;

    private SmarttributesUsingsProvider() { }
}