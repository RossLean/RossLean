using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;


public class GA0024_CodeFixTests : InstanceTypeMemberRemoverCodeFixTests
{
    [Test]
    public void RedundantUsageWithCodeFix()
    {
        var testCode =
            """
            [TypeConstraintProfileGroup]
            interface {|*:IGroup0|}
            {
                int Property { get; set; }
                void Function();
                interface INested { }
            }
            """;

        var fixedCode =
            """
            [TypeConstraintProfileGroup]
            interface IGroup0
            {
            }
            """;

        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
