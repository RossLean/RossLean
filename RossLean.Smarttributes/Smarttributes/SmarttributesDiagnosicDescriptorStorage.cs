using Microsoft.CodeAnalysis;
using RoseLynn.Analyzers;
using System.Resources;

namespace RossLean.Smarttributes;

public sealed class SmarttributesDiagnosticDescriptorStorage : DiagnosticDescriptorStorageBase
{
    public static readonly SmarttributesDiagnosticDescriptorStorage Instance = new();

    protected override string BaseRuleDocsURI => "https://github.com/RossLean/RossLean/tree/master/RossLean.Smarttributes/docs/rules";
    protected override string DiagnosticIDPrefix => "SMTTR";
    protected override ResourceManager ResourceManager => DiagnosticResources.ResourceManager;

    #region Category Constants
    public const string APIRestrictionsCategory = "API Restrictions";
    public const string BrevityCategory = "Brevity";
    public const string DesignCategory = "Design";
    public const string InformationCategory = "Information";
    public const string ValidityCategory = "Validity";
    #endregion

    #region Rules
    private SmarttributesDiagnosticDescriptorStorage()
    {
        // Looks fancy, doesn't it?

        SetDefaultDiagnosticAnalyzer<AttributePresenceAnalyzer>();
        {
            CreateDiagnosticDescriptor(0001, APIRestrictionsCategory, DiagnosticSeverity.Error);
        }

        SetDefaultDiagnosticAnalyzer<RestrictFunctionsAnalyzer>();
        {
            CreateDiagnosticDescriptor(0005, APIRestrictionsCategory, DiagnosticSeverity.Error);
        }

        SetDefaultDiagnosticAnalyzer<ParameterTypeMatchAnalyzer>();
        {
            CreateDiagnosticDescriptor(0010, APIRestrictionsCategory, DiagnosticSeverity.Error);
            CreateDiagnosticDescriptor(0011, APIRestrictionsCategory, DiagnosticSeverity.Error);
        }
    }
    #endregion
}
