using NUnit.Framework;
using RossLean.Common.Test;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

[Ignore(TestIgnoreStrings.NotYetImplemented)]
public sealed class GA0043_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void ConstrainedToStruct()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓FilterInterfaces(FilterType.Permitted)]
                [↓FilterDelegates(FilterType.Permitted)]
                [FilterEnums(FilterType.Permitted)]
                [↓FilterAbstractClasses(FilterType.Permitted)]
                [↓FilterSealedClasses(FilterType.Permitted)]
                [↓FilterRecordClasses(FilterType.Permitted)]
                [↓FilterRecordStructs(FilterType.Permitted)]
                [FilterGenericTypes(FilterType.Permitted)]
                [↓FilterArrayTypes(FilterType.Permitted)]
                [OnlyPermitSpecifiedTypeGroups]
                T
            >
                where T : struct
            ;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
