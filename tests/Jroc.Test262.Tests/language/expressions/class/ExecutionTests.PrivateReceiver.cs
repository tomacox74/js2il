using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_;

public partial class ExecutionTests
{
    [Fact(DisplayName = "private-getter-brand-check-multiple-evaluations-of-class-factory.js")]
    public Task private_receiver_private_getter_brand_check_multiple_evaluations_of_class_factory() => ExecutionTest("private-getter-brand-check-multiple-evaluations-of-class-factory");

    [Fact(DisplayName = "private-method-brand-check-multiple-evaluations-of-class-factory.js")]
    public Task private_receiver_private_method_brand_check_multiple_evaluations_of_class_factory() => ExecutionTest("private-method-brand-check-multiple-evaluations-of-class-factory");

}
