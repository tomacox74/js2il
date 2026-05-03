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

}

