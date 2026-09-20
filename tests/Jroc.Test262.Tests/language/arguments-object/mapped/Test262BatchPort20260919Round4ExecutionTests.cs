using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.arguments_object.mapped;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.arguments_object.mapped") { }

    [Fact(DisplayName = "enumerable-configurable-accessor-descriptor")]
    public Task enumerable_configurable_accessor_descriptor()
        => ExecutionTest("enumerable-configurable-accessor-descriptor");

    [Fact(DisplayName = "mapped-arguments-nonconfigurable-nonwritable-1")]
    public Task mapped_arguments_nonconfigurable_nonwritable_1()
        => ExecutionTest("mapped-arguments-nonconfigurable-nonwritable-1");

    [Fact(DisplayName = "mapped-arguments-nonconfigurable-nonwritable-2")]
    public Task mapped_arguments_nonconfigurable_nonwritable_2()
        => ExecutionTest("mapped-arguments-nonconfigurable-nonwritable-2");

    [Fact(DisplayName = "mapped-arguments-nonconfigurable-nonwritable-3")]
    public Task mapped_arguments_nonconfigurable_nonwritable_3()
        => ExecutionTest("mapped-arguments-nonconfigurable-nonwritable-3");

    [Fact(DisplayName = "mapped-arguments-nonconfigurable-nonwritable-4")]
    public Task mapped_arguments_nonconfigurable_nonwritable_4()
        => ExecutionTest("mapped-arguments-nonconfigurable-nonwritable-4");

    [Fact(DisplayName = "mapped-arguments-nonconfigurable-nonwritable-5")]
    public Task mapped_arguments_nonconfigurable_nonwritable_5()
        => ExecutionTest("mapped-arguments-nonconfigurable-nonwritable-5");

    [Fact(DisplayName = "mapped-arguments-nonwritable-nonconfigurable-1")]
    public Task mapped_arguments_nonwritable_nonconfigurable_1()
        => ExecutionTest("mapped-arguments-nonwritable-nonconfigurable-1");

    [Fact(DisplayName = "mapped-arguments-nonwritable-nonconfigurable-2")]
    public Task mapped_arguments_nonwritable_nonconfigurable_2()
        => ExecutionTest("mapped-arguments-nonwritable-nonconfigurable-2");

    [Fact(DisplayName = "mapped-arguments-nonwritable-nonconfigurable-3")]
    public Task mapped_arguments_nonwritable_nonconfigurable_3()
        => ExecutionTest("mapped-arguments-nonwritable-nonconfigurable-3");

    [Fact(DisplayName = "mapped-arguments-nonwritable-nonconfigurable-4")]
    public Task mapped_arguments_nonwritable_nonconfigurable_4()
        => ExecutionTest("mapped-arguments-nonwritable-nonconfigurable-4");

    [Fact(DisplayName = "nonconfigurable-descriptors-with-param-assign")]
    public Task nonconfigurable_descriptors_with_param_assign()
        => ExecutionTest("nonconfigurable-descriptors-with-param-assign");

    [Fact(DisplayName = "nonconfigurable-nonenumerable-nonwritable-descriptors-basic")]
    public Task nonconfigurable_nonenumerable_nonwritable_descriptors_basic()
        => ExecutionTest("nonconfigurable-nonenumerable-nonwritable-descriptors-basic");

    [Fact(DisplayName = "nonconfigurable-nonenumerable-nonwritable-descriptors-set-by-arguments")]
    public Task nonconfigurable_nonenumerable_nonwritable_descriptors_set_by_arguments()
        => ExecutionTest("nonconfigurable-nonenumerable-nonwritable-descriptors-set-by-arguments");

    [Fact(DisplayName = "nonconfigurable-nonenumerable-nonwritable-descriptors-set-by-param")]
    public Task nonconfigurable_nonenumerable_nonwritable_descriptors_set_by_param()
        => ExecutionTest("nonconfigurable-nonenumerable-nonwritable-descriptors-set-by-param");

    [Fact(DisplayName = "nonconfigurable-nonwritable-descriptors-basic")]
    public Task nonconfigurable_nonwritable_descriptors_basic()
        => ExecutionTest("nonconfigurable-nonwritable-descriptors-basic");

    [Fact(DisplayName = "nonconfigurable-nonwritable-descriptors-define-property-consecutive")]
    public Task nonconfigurable_nonwritable_descriptors_define_property_consecutive()
        => ExecutionTest("nonconfigurable-nonwritable-descriptors-define-property-consecutive");

    [Fact(DisplayName = "nonconfigurable-nonwritable-descriptors-set-by-arguments")]
    public Task nonconfigurable_nonwritable_descriptors_set_by_arguments()
        => ExecutionTest("nonconfigurable-nonwritable-descriptors-set-by-arguments");

    [Fact(DisplayName = "nonconfigurable-nonwritable-descriptors-set-by-param")]
    public Task nonconfigurable_nonwritable_descriptors_set_by_param()
        => ExecutionTest("nonconfigurable-nonwritable-descriptors-set-by-param");

    [Fact(DisplayName = "nonwritable-nonconfigurable-descriptors-basic")]
    public Task nonwritable_nonconfigurable_descriptors_basic()
        => ExecutionTest("nonwritable-nonconfigurable-descriptors-basic");

    [Fact(DisplayName = "nonwritable-nonconfigurable-descriptors-set-by-arguments")]
    public Task nonwritable_nonconfigurable_descriptors_set_by_arguments()
        => ExecutionTest("nonwritable-nonconfigurable-descriptors-set-by-arguments");

    [Fact(DisplayName = "nonwritable-nonenumerable-nonconfigurable-descriptors-basic")]
    public Task nonwritable_nonenumerable_nonconfigurable_descriptors_basic()
        => ExecutionTest("nonwritable-nonenumerable-nonconfigurable-descriptors-basic");

    [Fact(DisplayName = "nonwritable-nonenumerable-nonconfigurable-descriptors-set-by-arguments")]
    public Task nonwritable_nonenumerable_nonconfigurable_descriptors_set_by_arguments()
        => ExecutionTest("nonwritable-nonenumerable-nonconfigurable-descriptors-set-by-arguments");

    [Fact(DisplayName = "nonwritable-nonenumerable-nonconfigurable-descriptors-set-by-param")]
    public Task nonwritable_nonenumerable_nonconfigurable_descriptors_set_by_param()
        => ExecutionTest("nonwritable-nonenumerable-nonconfigurable-descriptors-set-by-param");

}
