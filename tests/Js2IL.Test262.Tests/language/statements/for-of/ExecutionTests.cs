using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.for_of;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.for_of") { }

    [Fact(DisplayName = "ArgumentsObject_mapped-aliasing")]
    public Task ArgumentsObject_mapped_aliasing()
        => ExecutionTest("ArgumentsObject_mapped-aliasing");

    [Fact(DisplayName = "Array.prototype.Symbol.iterator")]
    public Task Array_prototype_Symbol_iterator()
        => ExecutionTest("Array.prototype.Symbol.iterator");

    [Fact(DisplayName = "Array.prototype.entries")]
    public Task Array_prototype_entries()
        => ExecutionTest("Array.prototype.entries");

    [Fact(DisplayName = "Array.prototype.keys")]
    public Task Array_prototype_keys()
        => ExecutionTest("Array.prototype.keys");

    [Fact(DisplayName = "arguments-mapped-aliasing")]
    public Task arguments_mapped_aliasing()
        => ExecutionTest("arguments-mapped-aliasing");

    [Fact(DisplayName = "arguments-mapped-mutation")]
    public Task arguments_mapped_mutation()
        => ExecutionTest("arguments-mapped-mutation");

    [Fact(DisplayName = "arguments-mapped")]
    public Task arguments_mapped()
        => ExecutionTest("arguments-mapped");

    [Fact(DisplayName = "arguments-unmapped-aliasing")]
    public Task arguments_unmapped_aliasing()
        => ExecutionTest("arguments-unmapped-aliasing");

    [Fact(DisplayName = "arguments-unmapped-mutation")]
    public Task arguments_unmapped_mutation()
        => ExecutionTest("arguments-unmapped-mutation");

    [Fact(DisplayName = "arguments-unmapped")]
    public Task arguments_unmapped()
        => ExecutionTest("arguments-unmapped");

    [Fact(DisplayName = "ArgumentsObject_mapped")]
    public Task ArgumentsObject_mapped()
        => ExecutionTest("ArgumentsObject_mapped");

    [Fact(DisplayName = "ArgumentsObject_unmapped-aliasing")]
    public Task ArgumentsObject_unmapped_aliasing()
        => ExecutionTest("ArgumentsObject_unmapped-aliasing");

    [Fact(DisplayName = "array-contract-expand")]
    public Task array_contract_expand()
        => ExecutionTest("array-contract-expand");

    [Fact(DisplayName = "array-contract")]
    public Task array_contract()
        => ExecutionTest("array-contract");

    [Fact(DisplayName = "array-expand-contract")]
    public Task array_expand_contract()
        => ExecutionTest("array-expand-contract");

    [Fact(DisplayName = "array-expand")]
    public Task array_expand()
        => ExecutionTest("array-expand");

    [Fact(DisplayName = "array-key-get-error")]
    public Task array_key_get_error()
        => ExecutionTest("array-key-get-error");

    [Fact(DisplayName = "array")]
    public Task array()
        => ExecutionTest("array");

    [Fact(DisplayName = "body-dstr-assign-error")]
    public Task body_dstr_assign_error()
        => ExecutionTest("body-dstr-assign-error");
}
