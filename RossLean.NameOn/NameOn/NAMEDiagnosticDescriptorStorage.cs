using Microsoft.CodeAnalysis;
using RoseLynn.Analyzers;
using System.Resources;

namespace RossLean.NameOn;

internal sealed class NAMEDiagnosticDescriptorStorage : DiagnosticDescriptorStorageBase
{
    public static readonly NAMEDiagnosticDescriptorStorage Instance = new();

    protected override string BaseRuleDocsURI => "https://github.com/Rekkonnect/NameOn/blob/master/docs/rules";
    protected override string DiagnosticIDPrefix => "NAME";
    protected override ResourceManager ResourceManager => Resources.ResourceManager;

    #region Category Constants
    public const string APIRestrictionsCategory = "API Restrictions";
    public const string BrevityCategory = "Brevity";
    public const string DesignCategory = "Design";
    public const string InformationCategory = "Information";
    public const string ValidityCategory = "Validity";
    #endregion

    #region Rules
    private NAMEDiagnosticDescriptorStorage()
    {
        SetDefaultDiagnosticAnalyzer<NameOfUsageAnalyzer>();

        CreateDiagnosticDescriptor(0001, APIRestrictionsCategory, DiagnosticSeverity.Error);
        CreateDiagnosticDescriptor(0002, APIRestrictionsCategory, DiagnosticSeverity.Error);
        CreateDiagnosticDescriptor(0003, APIRestrictionsCategory, DiagnosticSeverity.Error);
        CreateDiagnosticDescriptor(0004, APIRestrictionsCategory, DiagnosticSeverity.Error);

        CreateDiagnosticDescriptor(0010, ValidityCategory, DiagnosticSeverity.Error);
    }
    #endregion
}
