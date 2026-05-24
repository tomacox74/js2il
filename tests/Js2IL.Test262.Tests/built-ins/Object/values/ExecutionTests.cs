using Js2IL.Test262.Tests.built_ins;



namespace Js2IL.Test262.Tests.built_ins.Object.values;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.Object.values") { }



    [Fact(DisplayName = "primitive-booleans")]

    public Task primitive_booleans()

        => ExecutionTestFromFile("primitive-booleans");



    [Fact(DisplayName = "primitive-numbers")]

    public Task primitive_numbers()

        => ExecutionTestFromFile("primitive-numbers");



    [Fact(DisplayName = "primitive-strings")]

    public Task primitive_strings()

        => ExecutionTestFromFile("primitive-strings");



    [Fact(DisplayName = "inherited-properties-omitted")]

    public Task inherited_properties_omitted()

        => ExecutionTestFromFile("inherited-properties-omitted");



    [Fact(DisplayName = "getter-removing-future-key")]

    public Task getter_removing_future_key()

        => ExecutionTestFromFile("getter-removing-future-key");

    [Fact(DisplayName = "exception-during-enumeration")]
    public Task exception_during_enumeration()
        => ExecutionTestFromFile("exception-during-enumeration");

    [Fact(DisplayName = "exception-not-object-coercible")]
    public Task exception_not_object_coercible()
        => ExecutionTestFromFile("exception-not-object-coercible");

    [Fact(DisplayName = "function-length")]
    public Task function_length()
        => ExecutionTestFromFile("function-length");

    [Fact(DisplayName = "function-name")]
    public Task function_name()
        => ExecutionTestFromFile("function-name");

}
