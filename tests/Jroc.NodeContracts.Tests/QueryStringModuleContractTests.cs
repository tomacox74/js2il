using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class QueryStringModuleContractTests
{
    [Fact]
    public void IQueryStringModule_MapsCompletePinnedRoster()
    {
        NodeContractTestHelpers.AssertGeneratedContract(
            typeof(IQueryStringModule),
            "querystring",
            6,
            18,
            0);
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(typeof(IQueryStringModule));
    }

    [Fact]
    public void IQueryStringModule_PreservesAliasAndOptionalCallForms()
    {
        Assert.Equal(4, NodeContractTestHelpers.GetMethods(typeof(IQueryStringModule), "decode").Length);
        Assert.Equal(4, NodeContractTestHelpers.GetMethods(typeof(IQueryStringModule), "parse").Length);
        Assert.All(
            NodeContractTestHelpers.GetMethods(typeof(IQueryStringModule), "stringify"),
            method => Assert.Equal(typeof(string), method.ReturnType));
    }

    [Fact]
    public void IntrinsicQueryString_DelegatesAliasesAndThrowsExplicitly()
    {
        IQueryStringModule module = new QueryString();

        Assert.Equal("value=1", module.encode(new JsObject { ["value"] = 1d }));
        Assert.IsType<JsObject>(module.decode("value=1"));
        var exception = Assert.Throws<NotImplementedException>(() => module.escape("a b"));
        Assert.Equal(
            "The intrinsic node:querystring module does not implement 'querystring.escape'.",
            exception.Message);
        NodeContractTestHelpers.AssertUsesStaticAdapters(typeof(QueryString));
    }
}
