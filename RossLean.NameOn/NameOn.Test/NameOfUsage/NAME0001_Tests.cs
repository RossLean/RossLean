using NUnit.Framework;
using RossLean.Common.Test;

namespace RossLean.NameOn.Test.NameOfUsage;

public sealed class NAME0001_Tests : NameOfUsageAnalyzerDiagnosticTests
{
    [Test]
    public void PropertyNameOf()
    {
        var testCode =
            """
            class Program
            {
                [ForceNameOf]
                string Property { get; set; }

                void Run()
                {
                    Property = ↓"nothing";
                }
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void FieldNameOf()
    {
        var testCode =
            """
            class Program
            {
                [ForceNameOf]
                string Field;

                void Run()
                {
                    Field = ↓"nothing";
                }
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    [Ignore(TestIgnoreStrings.NotYetImplemented)]
    public void FunctionArgumentNameOf()
    {
        var testCode =
            """
            class Program
            {
                void Run()
                {
                    Function(nameof(Run));
                    Function(↓"Run");
                }

                void Function([ForceNameOf] string name)
                {
                }
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    [Ignore(TestIgnoreStrings.NotYetImplemented)]
    public void DelegateInvocationArgumentNameOf()
    {
        var testCode =
            """
            class Program
            {
                void Run(Function f)
                {
                    f(nameof(Run));
                    f(↓"Run");
                }

                delegate void Function([ForceNameOf] string name);
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    [Ignore(TestIgnoreStrings.NotYetImplemented)]
    public void ConstructorAssignmentNameOf()
    {
        var testCode =
            """
            class C
            {
                [ForceNameOf]
                string Value { get; set; }

                public C(string value)
                {
                    Value = ↓value;
                }
                public C([ForceNameOf] string value, bool dummy)
                {
                    Value = value;
                }
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    [Ignore(TestIgnoreStrings.NotYetImplemented)]
    public void ConstructorInvocationNameOf()
    {
        var testCode =
            """
            class Program
            {
                void Function()
                {
                    new C(↓"value");
                    new C(nameof(C));
                }
            }

            class C
            {
                public C([ForceNameOf] string value)
                {
                }
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void FunctionReturnNameOf()
    {
        var testCode =
            """
            class Program
            {
                [return: ForceNameOf]
                string Function()
                {
                    return ↓"value";
                }
                [return: ForceNameOf]
                string AnotherFunction()
                {
                    return nameof(AnotherFunction);
                }
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    [Ignore(TestIgnoreStrings.NotYetImplemented)]
    public void DelegateReturnNameOf()
    {
        var testCode =
            """
            class Program
            {
                void Function()
                {
                    NameGetter getter = ↓GetRandomString;
                    NameGetter anotherGetter = GetName;

                    var name = anotherGetter();
                    AcceptName(name);
                }

                void AcceptName([ForceNameOf] string name) { }

                string GetRandomString()
                {
                    return "random";
                }
                [return: ForceNameOf]
                string GetName()
                {
                    return nameof(NameGetter);
                }

                [return: ForceNameOf]
                delegate string NameGetter();
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
