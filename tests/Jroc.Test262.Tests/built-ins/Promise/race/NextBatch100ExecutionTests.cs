using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.race;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.race") { }

    [Fact(DisplayName = "S25.4.4.3_A1.1_T1")]
    public Task S25_4_4_3_A1_1_T1() => ExecutionTestFromFile("S25.4.4.3_A1.1_T1");

    [Fact(DisplayName = "S25.4.4.3_A2.1_T1")]
    public Task S25_4_4_3_A2_1_T1() => ExecutionTestFromFile("S25.4.4.3_A2.1_T1");

    [Fact(DisplayName = "iter-next-val-err-no-close")]
    public Task iter_next_val_err_no_close() => ExecutionTestFromFile("iter-next-val-err-no-close");

    [Fact(DisplayName = "iter-step-err-no-close")]
    public Task iter_step_err_no_close() => ExecutionTestFromFile("iter-step-err-no-close");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "species-get-error")]
    public Task species_get_error() => ExecutionTestFromFile("species-get-error");
}
