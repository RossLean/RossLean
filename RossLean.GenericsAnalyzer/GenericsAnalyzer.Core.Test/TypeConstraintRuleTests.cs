using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Core.Test;

using static TypeConstraintRule;

public class TypeConstraintRuleTests
{
    [Test]
    public void FullySatisfiesTest()
    {
        foreach (var valid in AllValidRules)
            Assert.IsTrue(valid.FullySatisfies(valid));

        Assert.IsTrue(PermitBaseType.FullySatisfies(PermitExactType));
        Assert.IsTrue(ProhibitBaseType.FullySatisfies(ProhibitExactType));

        Assert.IsFalse(PermitExactType.FullySatisfies(PermitBaseType));
        Assert.IsFalse(ProhibitExactType.FullySatisfies(PermitBaseType));
        Assert.IsFalse(ProhibitExactType.FullySatisfies(PermitExactType));
        Assert.IsFalse(ProhibitExactType.FullySatisfies(ProhibitBaseType));
    }
}
