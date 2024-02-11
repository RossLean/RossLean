namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;

public abstract class PermittedTypeArgumentAnalyzerCodeFixTests<TCodeFix> : BaseCodeFixTests<PermittedTypeArgumentAnalyzer, TCodeFix>
    where TCodeFix : GACodeFixProvider, new()
{
}
