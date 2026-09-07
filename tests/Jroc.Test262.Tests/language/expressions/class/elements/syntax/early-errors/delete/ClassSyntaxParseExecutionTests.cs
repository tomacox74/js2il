using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.expressions.class_.elements.syntax.early_errors.delete;

public class ClassSyntaxParseExecutionTests : FileSystemExecutionTestsBase
{
    public ClassSyntaxParseExecutionTests() : base("language/expressions/class/elements/syntax/early-errors/delete", "language.expressions.class_.elements.syntax.early_errors.delete") { }

    [Fact(DisplayName = "field-delete-covered-err-delete-call-expression-private-method-accessor-get")]
    public Task field_delete_covered_err_delete_call_expression_private_method_accessor_get()
        => CompilationFailureTest("field-delete-covered-err-delete-call-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-call-expression-private-method-accessor-set")]
    public Task field_delete_covered_err_delete_call_expression_private_method_accessor_set()
        => CompilationFailureTest("field-delete-covered-err-delete-call-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-call-expression-private-method-async-gen")]
    public Task field_delete_covered_err_delete_call_expression_private_method_async_gen()
        => CompilationFailureTest("field-delete-covered-err-delete-call-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-call-expression-private-method-async")]
    public Task field_delete_covered_err_delete_call_expression_private_method_async()
        => CompilationFailureTest("field-delete-covered-err-delete-call-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-call-expression-private-method-gen")]
    public Task field_delete_covered_err_delete_call_expression_private_method_gen()
        => CompilationFailureTest("field-delete-covered-err-delete-call-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-call-expression-private-method")]
    public Task field_delete_covered_err_delete_call_expression_private_method()
        => CompilationFailureTest("field-delete-covered-err-delete-call-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-call-expression-private-no-reference")]
    public Task field_delete_covered_err_delete_call_expression_private_no_reference()
        => CompilationFailureTest("field-delete-covered-err-delete-call-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-call-expression-privatename")]
    public Task field_delete_covered_err_delete_call_expression_privatename()
        => CompilationFailureTest("field-delete-covered-err-delete-call-expression-privatename", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-member-expression-private-method-accessor-get")]
    public Task field_delete_covered_err_delete_member_expression_private_method_accessor_get()
        => CompilationFailureTest("field-delete-covered-err-delete-member-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-member-expression-private-method-accessor-set")]
    public Task field_delete_covered_err_delete_member_expression_private_method_accessor_set()
        => CompilationFailureTest("field-delete-covered-err-delete-member-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-member-expression-private-method-async-gen")]
    public Task field_delete_covered_err_delete_member_expression_private_method_async_gen()
        => CompilationFailureTest("field-delete-covered-err-delete-member-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-member-expression-private-method-async")]
    public Task field_delete_covered_err_delete_member_expression_private_method_async()
        => CompilationFailureTest("field-delete-covered-err-delete-member-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-member-expression-private-method-gen")]
    public Task field_delete_covered_err_delete_member_expression_private_method_gen()
        => CompilationFailureTest("field-delete-covered-err-delete-member-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-member-expression-private-method")]
    public Task field_delete_covered_err_delete_member_expression_private_method()
        => CompilationFailureTest("field-delete-covered-err-delete-member-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-member-expression-private-no-reference")]
    public Task field_delete_covered_err_delete_member_expression_private_no_reference()
        => CompilationFailureTest("field-delete-covered-err-delete-member-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-covered-err-delete-member-expression-privatename")]
    public Task field_delete_covered_err_delete_member_expression_privatename()
        => CompilationFailureTest("field-delete-covered-err-delete-member-expression-privatename", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-call-expression-private-method-accessor-get")]
    public Task field_delete_err_delete_call_expression_private_method_accessor_get()
        => CompilationFailureTest("field-delete-err-delete-call-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-call-expression-private-method-accessor-set")]
    public Task field_delete_err_delete_call_expression_private_method_accessor_set()
        => CompilationFailureTest("field-delete-err-delete-call-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-call-expression-private-method-async-gen")]
    public Task field_delete_err_delete_call_expression_private_method_async_gen()
        => CompilationFailureTest("field-delete-err-delete-call-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-call-expression-private-method-async")]
    public Task field_delete_err_delete_call_expression_private_method_async()
        => CompilationFailureTest("field-delete-err-delete-call-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-call-expression-private-method-gen")]
    public Task field_delete_err_delete_call_expression_private_method_gen()
        => CompilationFailureTest("field-delete-err-delete-call-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-call-expression-private-method")]
    public Task field_delete_err_delete_call_expression_private_method()
        => CompilationFailureTest("field-delete-err-delete-call-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-call-expression-private-no-reference")]
    public Task field_delete_err_delete_call_expression_private_no_reference()
        => CompilationFailureTest("field-delete-err-delete-call-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-call-expression-privatename")]
    public Task field_delete_err_delete_call_expression_privatename()
        => CompilationFailureTest("field-delete-err-delete-call-expression-privatename", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-member-expression-private-method-accessor-get")]
    public Task field_delete_err_delete_member_expression_private_method_accessor_get()
        => CompilationFailureTest("field-delete-err-delete-member-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-member-expression-private-method-accessor-set")]
    public Task field_delete_err_delete_member_expression_private_method_accessor_set()
        => CompilationFailureTest("field-delete-err-delete-member-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-member-expression-private-method-async-gen")]
    public Task field_delete_err_delete_member_expression_private_method_async_gen()
        => CompilationFailureTest("field-delete-err-delete-member-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-member-expression-private-method-async")]
    public Task field_delete_err_delete_member_expression_private_method_async()
        => CompilationFailureTest("field-delete-err-delete-member-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-member-expression-private-method-gen")]
    public Task field_delete_err_delete_member_expression_private_method_gen()
        => CompilationFailureTest("field-delete-err-delete-member-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-member-expression-private-method")]
    public Task field_delete_err_delete_member_expression_private_method()
        => CompilationFailureTest("field-delete-err-delete-member-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-member-expression-private-no-reference")]
    public Task field_delete_err_delete_member_expression_private_no_reference()
        => CompilationFailureTest("field-delete-err-delete-member-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-err-delete-member-expression-privatename")]
    public Task field_delete_err_delete_member_expression_privatename()
        => CompilationFailureTest("field-delete-err-delete-member-expression-privatename", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-call-expression-private-method-accessor-get")]
    public Task field_delete_twice_covered_err_delete_call_expression_private_method_accessor_get()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-call-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-call-expression-private-method-accessor-set")]
    public Task field_delete_twice_covered_err_delete_call_expression_private_method_accessor_set()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-call-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-call-expression-private-method-async-gen")]
    public Task field_delete_twice_covered_err_delete_call_expression_private_method_async_gen()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-call-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-call-expression-private-method-async")]
    public Task field_delete_twice_covered_err_delete_call_expression_private_method_async()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-call-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-call-expression-private-method-gen")]
    public Task field_delete_twice_covered_err_delete_call_expression_private_method_gen()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-call-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-call-expression-private-method")]
    public Task field_delete_twice_covered_err_delete_call_expression_private_method()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-call-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-call-expression-private-no-reference")]
    public Task field_delete_twice_covered_err_delete_call_expression_private_no_reference()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-call-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-call-expression-privatename")]
    public Task field_delete_twice_covered_err_delete_call_expression_privatename()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-call-expression-privatename", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-member-expression-private-method-accessor-get")]
    public Task field_delete_twice_covered_err_delete_member_expression_private_method_accessor_get()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-member-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-member-expression-private-method-accessor-set")]
    public Task field_delete_twice_covered_err_delete_member_expression_private_method_accessor_set()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-member-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-member-expression-private-method-async-gen")]
    public Task field_delete_twice_covered_err_delete_member_expression_private_method_async_gen()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-member-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-member-expression-private-method-async")]
    public Task field_delete_twice_covered_err_delete_member_expression_private_method_async()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-member-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-member-expression-private-method-gen")]
    public Task field_delete_twice_covered_err_delete_member_expression_private_method_gen()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-member-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-member-expression-private-method")]
    public Task field_delete_twice_covered_err_delete_member_expression_private_method()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-member-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-member-expression-private-no-reference")]
    public Task field_delete_twice_covered_err_delete_member_expression_private_no_reference()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-member-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-delete-twice-covered-err-delete-member-expression-privatename")]
    public Task field_delete_twice_covered_err_delete_member_expression_privatename()
        => CompilationFailureTest("field-delete-twice-covered-err-delete-member-expression-privatename", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-call-expression-private-method-accessor-get")]
    public Task method_delete_covered_err_delete_call_expression_private_method_accessor_get()
        => CompilationFailureTest("method-delete-covered-err-delete-call-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-call-expression-private-method-accessor-set")]
    public Task method_delete_covered_err_delete_call_expression_private_method_accessor_set()
        => CompilationFailureTest("method-delete-covered-err-delete-call-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-call-expression-private-method-async-gen")]
    public Task method_delete_covered_err_delete_call_expression_private_method_async_gen()
        => CompilationFailureTest("method-delete-covered-err-delete-call-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-call-expression-private-method-async")]
    public Task method_delete_covered_err_delete_call_expression_private_method_async()
        => CompilationFailureTest("method-delete-covered-err-delete-call-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-call-expression-private-method-gen")]
    public Task method_delete_covered_err_delete_call_expression_private_method_gen()
        => CompilationFailureTest("method-delete-covered-err-delete-call-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-call-expression-private-method")]
    public Task method_delete_covered_err_delete_call_expression_private_method()
        => CompilationFailureTest("method-delete-covered-err-delete-call-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-call-expression-private-no-reference")]
    public Task method_delete_covered_err_delete_call_expression_private_no_reference()
        => CompilationFailureTest("method-delete-covered-err-delete-call-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-call-expression-privatename")]
    public Task method_delete_covered_err_delete_call_expression_privatename()
        => CompilationFailureTest("method-delete-covered-err-delete-call-expression-privatename", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-member-expression-private-method-accessor-get")]
    public Task method_delete_covered_err_delete_member_expression_private_method_accessor_get()
        => CompilationFailureTest("method-delete-covered-err-delete-member-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-member-expression-private-method-accessor-set")]
    public Task method_delete_covered_err_delete_member_expression_private_method_accessor_set()
        => CompilationFailureTest("method-delete-covered-err-delete-member-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-member-expression-private-method-async-gen")]
    public Task method_delete_covered_err_delete_member_expression_private_method_async_gen()
        => CompilationFailureTest("method-delete-covered-err-delete-member-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-member-expression-private-method-async")]
    public Task method_delete_covered_err_delete_member_expression_private_method_async()
        => CompilationFailureTest("method-delete-covered-err-delete-member-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-member-expression-private-method-gen")]
    public Task method_delete_covered_err_delete_member_expression_private_method_gen()
        => CompilationFailureTest("method-delete-covered-err-delete-member-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-member-expression-private-method")]
    public Task method_delete_covered_err_delete_member_expression_private_method()
        => CompilationFailureTest("method-delete-covered-err-delete-member-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-member-expression-private-no-reference")]
    public Task method_delete_covered_err_delete_member_expression_private_no_reference()
        => CompilationFailureTest("method-delete-covered-err-delete-member-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-covered-err-delete-member-expression-privatename")]
    public Task method_delete_covered_err_delete_member_expression_privatename()
        => CompilationFailureTest("method-delete-covered-err-delete-member-expression-privatename", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-call-expression-private-method-accessor-get")]
    public Task method_delete_err_delete_call_expression_private_method_accessor_get()
        => CompilationFailureTest("method-delete-err-delete-call-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-call-expression-private-method-accessor-set")]
    public Task method_delete_err_delete_call_expression_private_method_accessor_set()
        => CompilationFailureTest("method-delete-err-delete-call-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-call-expression-private-method-async-gen")]
    public Task method_delete_err_delete_call_expression_private_method_async_gen()
        => CompilationFailureTest("method-delete-err-delete-call-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-call-expression-private-method-async")]
    public Task method_delete_err_delete_call_expression_private_method_async()
        => CompilationFailureTest("method-delete-err-delete-call-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-call-expression-private-method-gen")]
    public Task method_delete_err_delete_call_expression_private_method_gen()
        => CompilationFailureTest("method-delete-err-delete-call-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-call-expression-private-method")]
    public Task method_delete_err_delete_call_expression_private_method()
        => CompilationFailureTest("method-delete-err-delete-call-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-call-expression-private-no-reference")]
    public Task method_delete_err_delete_call_expression_private_no_reference()
        => CompilationFailureTest("method-delete-err-delete-call-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-call-expression-privatename")]
    public Task method_delete_err_delete_call_expression_privatename()
        => CompilationFailureTest("method-delete-err-delete-call-expression-privatename", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-member-expression-private-method-accessor-get")]
    public Task method_delete_err_delete_member_expression_private_method_accessor_get()
        => CompilationFailureTest("method-delete-err-delete-member-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-member-expression-private-method-accessor-set")]
    public Task method_delete_err_delete_member_expression_private_method_accessor_set()
        => CompilationFailureTest("method-delete-err-delete-member-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-member-expression-private-method-async-gen")]
    public Task method_delete_err_delete_member_expression_private_method_async_gen()
        => CompilationFailureTest("method-delete-err-delete-member-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-member-expression-private-method-async")]
    public Task method_delete_err_delete_member_expression_private_method_async()
        => CompilationFailureTest("method-delete-err-delete-member-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-member-expression-private-method-gen")]
    public Task method_delete_err_delete_member_expression_private_method_gen()
        => CompilationFailureTest("method-delete-err-delete-member-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-member-expression-private-method")]
    public Task method_delete_err_delete_member_expression_private_method()
        => CompilationFailureTest("method-delete-err-delete-member-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-member-expression-private-no-reference")]
    public Task method_delete_err_delete_member_expression_private_no_reference()
        => CompilationFailureTest("method-delete-err-delete-member-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-err-delete-member-expression-privatename")]
    public Task method_delete_err_delete_member_expression_privatename()
        => CompilationFailureTest("method-delete-err-delete-member-expression-privatename", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-call-expression-private-method-accessor-get")]
    public Task method_delete_twice_covered_err_delete_call_expression_private_method_accessor_get()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-call-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-call-expression-private-method-accessor-set")]
    public Task method_delete_twice_covered_err_delete_call_expression_private_method_accessor_set()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-call-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-call-expression-private-method-async-gen")]
    public Task method_delete_twice_covered_err_delete_call_expression_private_method_async_gen()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-call-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-call-expression-private-method-async")]
    public Task method_delete_twice_covered_err_delete_call_expression_private_method_async()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-call-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-call-expression-private-method-gen")]
    public Task method_delete_twice_covered_err_delete_call_expression_private_method_gen()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-call-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-call-expression-private-method")]
    public Task method_delete_twice_covered_err_delete_call_expression_private_method()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-call-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-call-expression-private-no-reference")]
    public Task method_delete_twice_covered_err_delete_call_expression_private_no_reference()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-call-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-call-expression-privatename")]
    public Task method_delete_twice_covered_err_delete_call_expression_privatename()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-call-expression-privatename", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-member-expression-private-method-accessor-get")]
    public Task method_delete_twice_covered_err_delete_member_expression_private_method_accessor_get()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-member-expression-private-method-accessor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-member-expression-private-method-accessor-set")]
    public Task method_delete_twice_covered_err_delete_member_expression_private_method_accessor_set()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-member-expression-private-method-accessor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-member-expression-private-method-async-gen")]
    public Task method_delete_twice_covered_err_delete_member_expression_private_method_async_gen()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-member-expression-private-method-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-member-expression-private-method-async")]
    public Task method_delete_twice_covered_err_delete_member_expression_private_method_async()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-member-expression-private-method-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-member-expression-private-method-gen")]
    public Task method_delete_twice_covered_err_delete_member_expression_private_method_gen()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-member-expression-private-method-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-member-expression-private-method")]
    public Task method_delete_twice_covered_err_delete_member_expression_private_method()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-member-expression-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-member-expression-private-no-reference")]
    public Task method_delete_twice_covered_err_delete_member_expression_private_no_reference()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-member-expression-private-no-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-delete-twice-covered-err-delete-member-expression-privatename")]
    public Task method_delete_twice_covered_err_delete_member_expression_privatename()
        => CompilationFailureTest("method-delete-twice-covered-err-delete-member-expression-privatename", "Failed to parse JavaScript");
}
