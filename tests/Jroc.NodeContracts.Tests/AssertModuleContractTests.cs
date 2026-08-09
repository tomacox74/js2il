using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class AssertModuleContractTests
{
    [Fact]
    public void IAssertModule_MapsCompletePinnedRoster()
    {
        NodeContractTestHelpers.AssertGeneratedContract(
            typeof(IAssertModule),
            "assert",
            19,
            45,
            4);
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(typeof(IAssertModule));
    }

    [Fact]
    public void IAssertModule_PreservesCallableAndOptionalCallForms()
    {
        Assert.Equal(2, NodeContractTestHelpers.GetMethods(typeof(IAssertModule), "assert").Length);
        Assert.Equal(6, NodeContractTestHelpers.GetMethods(typeof(IAssertModule), "fail").Length);
        Assert.Equal(3, NodeContractTestHelpers.GetMethods(typeof(IAssertModule), "throws").Length);
        Assert.Equal(
            typeof(IJavaScriptPromise),
            NodeContractTestHelpers.GetMethods(typeof(IAssertModule), "rejects")[0].ReturnType);
    }

    [Fact]
    public void IntrinsicAssert_IsCallableDelegatesCoreMembersAndThrowsExplicitly()
    {
        var intrinsic = new AssertModule();
        IAssertModule module = intrinsic;

        Assert.IsAssignableFrom<JsFunctionObject>(intrinsic);
        Assert.Null(module.ok(true));
        Assert.IsType<AssertionError>(Assert.Throws<AssertionError>(() => module.strictEqual(1d, 2d)));
        var exception = Assert.Throws<NotImplementedException>(() => module.deepStrictEqual(1d, 1d));
        Assert.Equal(
            "The intrinsic node:assert module does not implement 'assert.deepStrictEqual'.",
            exception.Message);
        NodeContractTestHelpers.AssertUsesStaticAdapters(typeof(AssertModule));
    }
}
