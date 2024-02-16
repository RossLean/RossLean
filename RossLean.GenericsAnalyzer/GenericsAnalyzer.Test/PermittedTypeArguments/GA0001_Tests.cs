using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0001_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void ImplicitFunctionTypeArgumentUsage()
    {
        var testCode =
            """
            class Program
            {
                void Run()
                {
                    ↓Function(5); // implicit usage of 'int'
                    Function<↓int>(5); // explicit usage of 'int'
                }

                void Function
                <
                    [ProhibitedTypes(typeof(int))]
                    T
                >
                (T v)
                {
                }
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void IntermediateSubsetGenericTypeUsage()
    {
        var testCode =
            """
            class Program
            {
                static void Main()
                {
                    new A<int>();
                    new B<int>();
                    new A<↓A>();
                    new B<↓A>();
                    new A<B>();
                    new B<↓B>();
                }
            }

            class A
            <
                [ProhibitedBaseTypes(typeof(IA))]
                T
            >
            {
            }
            class B
            <
                [ProhibitedBaseTypes(typeof(IB))]
                T
            > : A<T>
            {
            }

            interface IA : IB { }
            interface IB { }

            class A : IA { }
            class B : IB { }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void IntermediateExplicitlyPermittedGenericTypeUsage()
    {
        var testCode =
            """
            class Program
            {
                static void Main()
                {
                    new A<int>();
                    new B<int>();
                    new A<↓string>();
                    new B<↓string>();
                }
            }

            class A
            <
                [PermittedTypes(typeof(int), typeof(uint))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            class B
            <
                [InheritBaseTypeUsageConstraints]
                T
            > : A<T>
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void IntermediateProhibitedGenericTypeUsage()
    {
        var testCode =
            """
            class Program
            {
                static void Main()
                {
                    new A<int>();
                    new B<int>();
                    new A<↓string>();
                    new B<↓string>();
                }
            }

            class A
            <
                [ProhibitedTypes(typeof(string))]
                T
            >
            {
            }
            class B
            <
                [InheritBaseTypeUsageConstraints]
                T
            > : A<T>
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void IntermediateTwoClassProhibitedGenericTypeUsage()
    {
        var testCode =
            """
            class Program
            {
                static void Main()
                {
                    new A<int>();
                    new C<int>();
                    new A<↓string>();
                    new C<↓string>();
                }
            }

            class A
            <
                [ProhibitedTypes(typeof(string))]
                T
            >
            {
            }
            class B
            <
                [InheritBaseTypeUsageConstraints]
                U
            > : A<U>
            {
            }
            class C
            <
                [InheritBaseTypeUsageConstraints]
                V
            > : B<V>
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void GenericClassTestCode()
    {
        var testCode =
            """
            class Program
            {
                static void Main()
                {
                    new Generic<int, int>();
                    new Generic<↓string, int>();
                    new Generic<int, ↓ulong>();
                }
            }

            class Generic
            <
                [ProhibitedTypes(typeof(string))]
                TNotString,
                [PermittedTypes(typeof(int))]
                [ProhibitedBaseTypes(typeof(IComparable<>))]
                TNotComparableButInt
            >
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void GenericFunctionTestCode()
    {
        var testCode =
            """
            class Program
            {
                static void Main()
                {
                    Function<int, int>();
                    Function<↓string, int>();
                    Function<int, ↓ulong>();
                    Function<int, ↓IComparable<ulong>>();
                }

                static void Function
                <
                    [ProhibitedTypes(typeof(string))]
                    TNotString,
                    [PermittedTypes(typeof(int))]
                    [ProhibitedBaseTypes(typeof(IComparable<>))]
                    TNotComparableButInt
                >
                ()
                {   
                }
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void OnlyPermitSpecifiedTypesTestCode()
    {
        var testCode =
            """
            class Program
            {
                static void Main()
                {
                    new Generic<int>();
                    new Generic<long>();
                    new Generic<string>();
                    new Generic<IEnumerable<string>>();
                    new Generic<↓ulong>();
                    new Generic<↓byte>();
                    new Generic<A<int>>();
                    new Generic<B<int>>();
                }
            }

            class Generic
            <
                [PermittedTypes(typeof(int), typeof(long))]
                [PermittedBaseTypes(typeof(IEnumerable<>))]
                [PermittedBaseTypes(typeof(A<>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }

            class A<T> { }
            class B<T> : A<T> { }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void PartialClassTestCode()
    {
        var testCode =
            """
            class Program
            {
                static void Main()
                {
                    new Generic<int>();
                    new Generic<long>();
                    new Generic<string>();
                    new Generic<IEnumerable<string>>();
                    new Generic<↓ulong>();
                    new Generic<↓byte>();
                }
            }

            partial class Generic
            <
                [PermittedBaseTypes(typeof(IEnumerable<>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }

            partial class Generic
            <
                [PermittedTypes(typeof(int), typeof(long))]
                T
            >
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void ReasonedProhibition()
    {
        const string testCode =
            """
            class Program
            {
                static void Main()
                {
                    new Generic<↓IEnumerable<string>>();
                }
            }

            partial class Generic
            <
                [ProhibitedBaseTypes(typeof(IEnumerable<>), Reason = "Bad type 1")]
                [ProhibitedBaseTypes(typeof(IReadOnlyCollection<>), Reason = "Bad type 2")]
                [ProhibitedBaseTypes(typeof(string), Reason = "Bad type 3")]
                T
            >
            {
            }
            """;

        var message =
            """
            The type 'System.Collections.Generic.IEnumerable<string>' cannot be used as a generic type argument for the type 'Generic<T>' in this position
            Reason: Bad type 1
            """;
        var withUsings = UsingsProvider.WithUsings(testCode);
        AssertDiagnosticsWithMessage(withUsings, message);
    }

    [Test]
    public void UnreasonedProhibition()
    {
        const string testCode =
            """
            class Program
            {
                static void Main()
                {
                    new Generic<↓IEnumerable<string>>();
                }
            }

            partial class Generic
            <
                [ProhibitedBaseTypes(typeof(IEnumerable<>))]
                [ProhibitedBaseTypes(typeof(IReadOnlyCollection<>), Reason = "Bad type 2")]
                [ProhibitedBaseTypes(typeof(string), Reason = "Bad type 3")]
                T
            >
            {
            }
            """;

        var message =
            """
            The type 'System.Collections.Generic.IEnumerable<string>' cannot be used as a generic type argument for the type 'Generic<T>' in this position
            """;
        var withUsings = UsingsProvider.WithUsings(testCode);
        AssertDiagnosticsWithMessage(withUsings, message);
    }

    [Test]
    public void GenericAttributeProhibition()
    {
        const string testCode =
            """
            class Program
            {
                [Generic<↓IEnumerable<string>>]
                static void Main()
                {
                }
            }

            sealed class GenericAttribute
            <
                [ProhibitedBaseTypes(typeof(IEnumerable<>))]
                T
            >
                : Attribute
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    #region Type group filters
    private const string _typeGroupFilterTestTypes =
        """
        abstract class Abstract;
        sealed class Sealed;
        class NonAbstractNonSealed;
        record class NonAbstractNonSealedRecordClass;
        abstract record class AbstractRecordClass;
        sealed record class SealedRecordClass;
        struct Struct;
        record struct RecordStruct;
        enum ExampleEnum;
        delegate void ExampleDelegate();
        """;

    [Test]
    public void TypeGroupFilters_01()
    {
        const string testCode =
            $$"""
            {{_typeGroupFilterTestTypes}}

            class Program
            {
                static void Main()
                {
                    new Generic<↓IEnumerable<string>>();
                    new Generic<↓IEnumerable>();
                    new Generic<string>();
                    new Generic<↓Abstract>();
                    new Generic<↓NonAbstractNonSealed>();
                    new Generic<↓NonAbstractNonSealedRecordClass>();
                    new Generic<↓AbstractRecordClass>();
                    new Generic<SealedRecordClass>();
                    new Generic<↓Struct>();
                    new Generic<↓RecordStruct>();
                    new Generic<↓ExampleEnum>();
                    new Generic<ExampleDelegate>();
                }
            }

            partial class Generic
            <
                [OnlyPermitSpecifiedTypeGroups]
                [FilterDelegates(FilterType.Permitted)]
                [FilterSealedClasses(FilterType.Permitted)]
                [FilterAbstractClasses(FilterType.Prohibited)]
                T
            >
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void TypeGroupFilters_02()
    {
        const string testCode =
            $$"""
            {{_typeGroupFilterTestTypes}}

            class Program
            {
                static void Main()
                {
                    new Generic<↓IEnumerable<string>>();
                    new Generic<↓IEnumerable>();
                    new Generic<↓string>();
                    new Generic<↓Abstract>();
                    new Generic<↓NonAbstractNonSealed>();
                    new Generic<↓NonAbstractNonSealedRecordClass>();
                    new Generic<AbstractRecordClass>();
                    new Generic<↓SealedRecordClass>();
                    new Generic<↓Struct>();
                    new Generic<↓RecordStruct>();
                    new Generic<↓ExampleEnum>();
                    new Generic<↓ExampleDelegate>();
                }
            }

            partial class Generic
            <
                [FilterAbstractClasses(FilterType.Exclusive)]
                [FilterRecordClasses(FilterType.Exclusive)]
                T
            >
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
    #endregion
}
