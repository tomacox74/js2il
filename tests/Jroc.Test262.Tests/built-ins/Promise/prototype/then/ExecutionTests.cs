using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.prototype.then;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    [Fact(DisplayName = "S25.4.4_A1.1_T1.js")] public Task S25_4_4_A1_1_T1() => ExecutionTestFromFile("S25.4.4_A1.1_T1");
    [Fact(DisplayName = "S25.4.4_A2.1_T1.js")] public Task S25_4_4_A2_1_T1() => ExecutionTestFromFile("S25.4.4_A2.1_T1");
    [Fact(DisplayName = "S25.4.4_A2.1_T2.js")] public Task S25_4_4_A2_1_T2() => ExecutionTestFromFile("S25.4.4_A2.1_T2");
    [Fact(DisplayName = "S25.4.4_A2.1_T3.js")] public Task S25_4_4_A2_1_T3() => ExecutionTestFromFile("S25.4.4_A2.1_T3");
    [Fact(DisplayName = "S25.4.5.3_A4.1_T1.js")] public Task S25_4_5_3_A4_1_T1() => ExecutionTestFromFile("S25.4.5.3_A4.1_T1");
    [Fact(DisplayName = "S25.4.5.3_A4.1_T2.js")] public Task S25_4_5_3_A4_1_T2() => ExecutionTestFromFile("S25.4.5.3_A4.1_T2");
    [Fact(DisplayName = "S25.4.5.3_A4.2_T1.js")] public Task S25_4_5_3_A4_2_T1() => ExecutionTestFromFile("S25.4.5.3_A4.2_T1");
    [Fact(DisplayName = "S25.4.5.3_A4.2_T2.js")] public Task S25_4_5_3_A4_2_T2() => ExecutionTestFromFile("S25.4.5.3_A4.2_T2");
    [Fact(DisplayName = "S25.4.5.3_A5.1_T1.js")] public Task S25_4_5_3_A5_1_T1() => ExecutionTestFromFile("S25.4.5.3_A5.1_T1");
    [Fact(DisplayName = "S25.4.5.3_A5.2_T1.js")] public Task S25_4_5_3_A5_2_T1() => ExecutionTestFromFile("S25.4.5.3_A5.2_T1");
    [Fact(DisplayName = "S25.4.5.3_A5.3_T1.js")] public Task S25_4_5_3_A5_3_T1() => ExecutionTestFromFile("S25.4.5.3_A5.3_T1");
    [Fact(DisplayName = "capability-executor-called-twice.js")] public Task capability_executor_called_twice() => ExecutionTestFromFile("capability-executor-called-twice");
    [Fact(DisplayName = "capability-executor-not-callable.js")] public Task capability_executor_not_callable() => ExecutionTestFromFile("capability-executor-not-callable");
    [Fact(DisplayName = "ctor-access-count.js")] public Task ctor_access_count() => ExecutionTestFromFile("ctor-access-count");
    [Fact(DisplayName = "deferred-is-resolved-value.js")] public Task deferred_is_resolved_value() => ExecutionTestFromFile("deferred-is-resolved-value");
    [Fact(DisplayName = "prfm-fulfilled.js")] public Task prfm_fulfilled() => ExecutionTestFromFile("prfm-fulfilled");
    [Fact(DisplayName = "prfm-pending-fulfulled.js")] public Task prfm_pending_fulfulled() => ExecutionTestFromFile("prfm-pending-fulfulled");
    [Fact(DisplayName = "prfm-pending-rejected.js")] public Task prfm_pending_rejected() => ExecutionTestFromFile("prfm-pending-rejected");
    [Fact(DisplayName = "prfm-rejected.js")] public Task prfm_rejected() => ExecutionTestFromFile("prfm-rejected");
    [Fact(DisplayName = "resolve-pending-fulfilled-non-obj.js")] public Task resolve_pending_fulfilled_non_obj() => ExecutionTestFromFile("resolve-pending-fulfilled-non-obj");
    [Fact(DisplayName = "resolve-pending-fulfilled-non-thenable.js")] public Task resolve_pending_fulfilled_non_thenable() => ExecutionTestFromFile("resolve-pending-fulfilled-non-thenable");
    [Fact(DisplayName = "resolve-pending-fulfilled-poisoned-then.js")] public Task resolve_pending_fulfilled_poisoned_then() => ExecutionTestFromFile("resolve-pending-fulfilled-poisoned-then");
    [Fact(DisplayName = "resolve-pending-fulfilled-prms-cstm-then.js")] public Task resolve_pending_fulfilled_prms_cstm_then() => ExecutionTestFromFile("resolve-pending-fulfilled-prms-cstm-then");
    [Fact(DisplayName = "resolve-pending-fulfilled-self.js")] public Task resolve_pending_fulfilled_self() => ExecutionTestFromFile("resolve-pending-fulfilled-self");
    [Fact(DisplayName = "resolve-pending-fulfilled-thenable.js")] public Task resolve_pending_fulfilled_thenable() => ExecutionTestFromFile("resolve-pending-fulfilled-thenable");
    [Fact(DisplayName = "resolve-pending-rejected-non-obj.js")] public Task resolve_pending_rejected_non_obj() => ExecutionTestFromFile("resolve-pending-rejected-non-obj");
    [Fact(DisplayName = "resolve-pending-rejected-non-thenable.js")] public Task resolve_pending_rejected_non_thenable() => ExecutionTestFromFile("resolve-pending-rejected-non-thenable");
    [Fact(DisplayName = "resolve-pending-rejected-poisoned-then.js")] public Task resolve_pending_rejected_poisoned_then() => ExecutionTestFromFile("resolve-pending-rejected-poisoned-then");
    [Fact(DisplayName = "resolve-pending-rejected-prms-cstm-then.js")] public Task resolve_pending_rejected_prms_cstm_then() => ExecutionTestFromFile("resolve-pending-rejected-prms-cstm-then");
    [Fact(DisplayName = "resolve-pending-rejected-self.js")] public Task resolve_pending_rejected_self() => ExecutionTestFromFile("resolve-pending-rejected-self");
    [Fact(DisplayName = "resolve-pending-rejected-thenable.js")] public Task resolve_pending_rejected_thenable() => ExecutionTestFromFile("resolve-pending-rejected-thenable");
    [Fact(DisplayName = "resolve-settled-fulfilled-non-obj.js")] public Task resolve_settled_fulfilled_non_obj() => ExecutionTestFromFile("resolve-settled-fulfilled-non-obj");
    [Fact(DisplayName = "resolve-settled-fulfilled-non-thenable.js")] public Task resolve_settled_fulfilled_non_thenable() => ExecutionTestFromFile("resolve-settled-fulfilled-non-thenable");
    [Fact(DisplayName = "resolve-settled-fulfilled-poisoned-then.js")] public Task resolve_settled_fulfilled_poisoned_then() => ExecutionTestFromFile("resolve-settled-fulfilled-poisoned-then");
    [Fact(DisplayName = "resolve-settled-fulfilled-prms-cstm-then.js")] public Task resolve_settled_fulfilled_prms_cstm_then() => ExecutionTestFromFile("resolve-settled-fulfilled-prms-cstm-then");
    [Fact(DisplayName = "resolve-settled-fulfilled-self.js")] public Task resolve_settled_fulfilled_self() => ExecutionTestFromFile("resolve-settled-fulfilled-self");
    [Fact(DisplayName = "resolve-settled-fulfilled-thenable.js")] public Task resolve_settled_fulfilled_thenable() => ExecutionTestFromFile("resolve-settled-fulfilled-thenable");
    [Fact(DisplayName = "resolve-settled-rejected-non-obj.js")] public Task resolve_settled_rejected_non_obj() => ExecutionTestFromFile("resolve-settled-rejected-non-obj");
    [Fact(DisplayName = "resolve-settled-rejected-non-thenable.js")] public Task resolve_settled_rejected_non_thenable() => ExecutionTestFromFile("resolve-settled-rejected-non-thenable");
    [Fact(DisplayName = "resolve-settled-rejected-poisoned-then.js")] public Task resolve_settled_rejected_poisoned_then() => ExecutionTestFromFile("resolve-settled-rejected-poisoned-then");
    [Fact(DisplayName = "resolve-settled-rejected-prms-cstm-then.js")] public Task resolve_settled_rejected_prms_cstm_then() => ExecutionTestFromFile("resolve-settled-rejected-prms-cstm-then");
    [Fact(DisplayName = "resolve-settled-rejected-self.js")] public Task resolve_settled_rejected_self() => ExecutionTestFromFile("resolve-settled-rejected-self");
    [Fact(DisplayName = "resolve-settled-rejected-thenable.js")] public Task resolve_settled_rejected_thenable() => ExecutionTestFromFile("resolve-settled-rejected-thenable");
    [Fact(DisplayName = "rxn-handler-fulfilled-invoke-nonstrict.js")] public Task rxn_handler_fulfilled_invoke_nonstrict() => ExecutionTestFromFile("rxn-handler-fulfilled-invoke-nonstrict");
    [Fact(DisplayName = "rxn-handler-fulfilled-invoke-strict.js")] public Task rxn_handler_fulfilled_invoke_strict() => ExecutionTestFromFile("rxn-handler-fulfilled-invoke-strict");
    [Fact(DisplayName = "rxn-handler-fulfilled-next-abrupt.js")] public Task rxn_handler_fulfilled_next_abrupt() => ExecutionTestFromFile("rxn-handler-fulfilled-next-abrupt");
    [Fact(DisplayName = "rxn-handler-fulfilled-next.js")] public Task rxn_handler_fulfilled_next() => ExecutionTestFromFile("rxn-handler-fulfilled-next");
    [Fact(DisplayName = "rxn-handler-fulfilled-return-normal.js")] public Task rxn_handler_fulfilled_return_normal() => ExecutionTestFromFile("rxn-handler-fulfilled-return-normal");
    [Fact(DisplayName = "rxn-handler-identity.js")] public Task rxn_handler_identity() => ExecutionTestFromFile("rxn-handler-identity");
    [Fact(DisplayName = "rxn-handler-rejected-invoke-nonstrict.js")] public Task rxn_handler_rejected_invoke_nonstrict() => ExecutionTestFromFile("rxn-handler-rejected-invoke-nonstrict");
    [Fact(DisplayName = "rxn-handler-rejected-invoke-strict.js")] public Task rxn_handler_rejected_invoke_strict() => ExecutionTestFromFile("rxn-handler-rejected-invoke-strict");
    [Fact(DisplayName = "rxn-handler-rejected-next-abrupt.js")] public Task rxn_handler_rejected_next_abrupt() => ExecutionTestFromFile("rxn-handler-rejected-next-abrupt");
    [Fact(DisplayName = "rxn-handler-rejected-next.js")] public Task rxn_handler_rejected_next() => ExecutionTestFromFile("rxn-handler-rejected-next");
    [Fact(DisplayName = "rxn-handler-rejected-return-normal.js")] public Task rxn_handler_rejected_return_normal() => ExecutionTestFromFile("rxn-handler-rejected-return-normal");
    [Fact(DisplayName = "rxn-handler-thrower.js")] public Task rxn_handler_thrower() => ExecutionTestFromFile("rxn-handler-thrower");

    public ExecutionTests() : base("built_ins.Promise.prototype.then") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "reject-pending-fulfilled.js")]
    public Task reject_pending_fulfilled()
        => ExecutionTestFromFile("reject-pending-fulfilled");

    [Fact(DisplayName = "reject-pending-rejected.js")]
    public Task reject_pending_rejected()
        => ExecutionTestFromFile("reject-pending-rejected");

    [Fact(DisplayName = "reject-settled-fulfilled.js")]
    public Task reject_settled_fulfilled()
        => ExecutionTestFromFile("reject-settled-fulfilled");

    [Fact(DisplayName = "reject-settled-rejected.js")]
    public Task reject_settled_rejected()
        => ExecutionTestFromFile("reject-settled-rejected");

    [Fact(DisplayName = "rxn-handler-fulfilled-return-abrupt.js")]
    public Task rxn_handler_fulfilled_return_abrupt()
        => ExecutionTestFromFile("rxn-handler-fulfilled-return-abrupt");

    [Fact(DisplayName = "rxn-handler-rejected-return-abrupt.js")]
    public Task rxn_handler_rejected_return_abrupt()
        => ExecutionTestFromFile("rxn-handler-rejected-return-abrupt");
}
