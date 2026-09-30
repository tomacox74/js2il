using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.elements;

public partial class ExecutionTests
{
    [Fact(DisplayName = "private-field-on-nested-class.js")]
    public Task private_receiver_private_field_on_nested_class() => ExecutionTest("private-field-on-nested-class");

    [Fact(DisplayName = "private-getter-access-on-inner-function.js")]
    public Task private_receiver_private_getter_access_on_inner_function() => ExecutionTest("private-getter-access-on-inner-function");

    [Fact(DisplayName = "private-getter-on-nested-class.js")]
    public Task private_receiver_private_getter_on_nested_class() => ExecutionTest("private-getter-on-nested-class");

    [Fact(DisplayName = "private-getter-shadowed-by-getter-on-nested-class.js")]
    public Task private_receiver_private_getter_shadowed_by_getter_on_nested_class() => ExecutionTest("private-getter-shadowed-by-getter-on-nested-class");

    [Fact(DisplayName = "private-getter-shadowed-by-method-on-nested-class.js")]
    public Task private_receiver_private_getter_shadowed_by_method_on_nested_class() => ExecutionTest("private-getter-shadowed-by-method-on-nested-class");

    [Fact(DisplayName = "private-getter-shadowed-by-setter-on-nested-class.js")]
    public Task private_receiver_private_getter_shadowed_by_setter_on_nested_class() => ExecutionTest("private-getter-shadowed-by-setter-on-nested-class");

    [Fact(DisplayName = "private-method-access-on-inner-function.js")]
    public Task private_receiver_private_method_access_on_inner_function() => ExecutionTest("private-method-access-on-inner-function");

    [Fact(DisplayName = "private-method-on-nested-class.js")]
    public Task private_receiver_private_method_on_nested_class() => ExecutionTest("private-method-on-nested-class");

    [Fact(DisplayName = "private-method-shadowed-by-getter-on-nested-class.js")]
    public Task private_receiver_private_method_shadowed_by_getter_on_nested_class() => ExecutionTest("private-method-shadowed-by-getter-on-nested-class");

    [Fact(DisplayName = "private-method-shadowed-by-setter-on-nested-class.js")]
    public Task private_receiver_private_method_shadowed_by_setter_on_nested_class() => ExecutionTest("private-method-shadowed-by-setter-on-nested-class");

    [Fact(DisplayName = "private-setter-on-nested-class.js")]
    public Task private_receiver_private_setter_on_nested_class() => ExecutionTest("private-setter-on-nested-class");

    [Fact(DisplayName = "private-static-field-shadowed-by-getter-on-nested-class.js")]
    public Task private_receiver_private_static_field_shadowed_by_getter_on_nested_class() => ExecutionTest("private-static-field-shadowed-by-getter-on-nested-class");

    [Fact(DisplayName = "private-static-field-shadowed-by-method-on-nested-class.js")]
    public Task private_receiver_private_static_field_shadowed_by_method_on_nested_class() => ExecutionTest("private-static-field-shadowed-by-method-on-nested-class");

    [Fact(DisplayName = "private-static-field-usage-inside-nested-class.js")]
    public Task private_receiver_private_static_field_usage_inside_nested_class() => ExecutionTest("private-static-field-usage-inside-nested-class");

    [Fact(DisplayName = "private-static-method-shadowed-by-getter-on-nested-class.js")]
    public Task private_receiver_private_static_method_shadowed_by_getter_on_nested_class() => ExecutionTest("private-static-method-shadowed-by-getter-on-nested-class");

    [Fact(DisplayName = "private-static-method-shadowed-by-method-on-nested-class.js")]
    public Task private_receiver_private_static_method_shadowed_by_method_on_nested_class() => ExecutionTest("private-static-method-shadowed-by-method-on-nested-class");

    [Fact(DisplayName = "private-static-method-usage-inside-nested-class.js")]
    public Task private_receiver_private_static_method_usage_inside_nested_class() => ExecutionTest("private-static-method-usage-inside-nested-class");

    [Fact(DisplayName = "static-private-getter-access-on-inner-class.js")]
    public Task private_receiver_static_private_getter_access_on_inner_class() => ExecutionTest("static-private-getter-access-on-inner-class");

    [Fact(DisplayName = "static-private-getter-access-on-inner-function.js")]
    public Task private_receiver_static_private_getter_access_on_inner_function() => ExecutionTest("static-private-getter-access-on-inner-function");

    [Fact(DisplayName = "static-private-method-access-on-inner-function.js")]
    public Task private_receiver_static_private_method_access_on_inner_function() => ExecutionTest("static-private-method-access-on-inner-function");

    [Fact(DisplayName = "static-private-setter-access-on-inner-class.js")]
    public Task private_receiver_static_private_setter_access_on_inner_class() => ExecutionTest("static-private-setter-access-on-inner-class");

}
