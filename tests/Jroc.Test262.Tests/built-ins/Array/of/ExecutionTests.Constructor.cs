namespace Jroc.Test262.Tests.built_ins.Array.of;

public partial class ExecutionTests
{
    [Fact(DisplayName = "construct-this-with-the-number-of-arguments.js")]
    public Task construct_this_with_the_number_of_arguments()
        => ExecutionTestFromFile("construct-this-with-the-number-of-arguments");

    [Fact(DisplayName = "does-not-use-prototype-properties.js")]
    public Task does_not_use_prototype_properties()
        => ExecutionTestFromFile("does-not-use-prototype-properties");

    [Fact(DisplayName = "does-not-use-set-for-indices.js")]
    public Task does_not_use_set_for_indices()
        => ExecutionTestFromFile("does-not-use-set-for-indices");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "of.js")]
    public Task of() => ExecutionTestFromFile("of");

    [Fact(DisplayName = "return-a-custom-instance.js")]
    public Task return_a_custom_instance()
        => ExecutionTestFromFile("return-a-custom-instance");

    [Fact(DisplayName = "return-abrupt-from-contructor.js")]
    public Task return_abrupt_from_contructor()
        => ExecutionTestFromFile("return-abrupt-from-contructor");

    [Fact(DisplayName = "return-abrupt-from-setting-length.js")]
    public Task return_abrupt_from_setting_length()
        => ExecutionTestFromFile("return-abrupt-from-setting-length");

    [Fact(DisplayName = "sets-length.js")]
    public Task sets_length() => ExecutionTestFromFile("sets-length");
}
