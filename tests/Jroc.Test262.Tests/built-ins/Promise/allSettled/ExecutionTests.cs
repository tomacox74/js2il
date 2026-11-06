using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.allSettled;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.allSettled") { }

    [Fact(DisplayName = "resolved-all-fulfilled.js")] public Task resolved_all_fulfilled() => ExecutionTestFromFile("resolved-all-fulfilled");
    [Fact(DisplayName = "resolved-all-mixed.js")] public Task resolved_all_mixed() => ExecutionTestFromFile("resolved-all-mixed");
    [Fact(DisplayName = "resolved-all-rejected.js")] public Task resolved_all_rejected() => ExecutionTestFromFile("resolved-all-rejected");
    [Fact(DisplayName = "resolved-immed.js")] public Task resolved_immed() => ExecutionTestFromFile("resolved-immed");
    [Fact(DisplayName = "resolved-sequence-extra-ticks.js")] public Task resolved_sequence_extra_ticks() => ExecutionTestFromFile("resolved-sequence-extra-ticks");
    [Fact(DisplayName = "resolved-sequence-mixed.js")] public Task resolved_sequence_mixed() => ExecutionTestFromFile("resolved-sequence-mixed");
    [Fact(DisplayName = "resolved-sequence-with-rejections.js")] public Task resolved_sequence_with_rejections() => ExecutionTestFromFile("resolved-sequence-with-rejections");
    [Fact(DisplayName = "resolved-sequence.js")] public Task resolved_sequence() => ExecutionTestFromFile("resolved-sequence");
    [Fact(DisplayName = "resolves-empty-array.js")] public Task resolves_empty_array() => ExecutionTestFromFile("resolves-empty-array");
    [Fact(DisplayName = "resolves-to-array.js")] public Task resolves_to_array() => ExecutionTestFromFile("resolves-to-array");
}
