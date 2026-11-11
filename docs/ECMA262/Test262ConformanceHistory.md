# Test262 Conformance History and Runner Notes

[Current coverage summary](Test262Conformance.md) | [ECMA-262 Coverage Index](Index.md)

Historical coverage updates and runner details moved out of the summary on
2026-11-06. The notes below retain their original wording and evidence
qualifications; batch counts, support statements, and reporting-access
limitations describe the work at that time, not necessarily the current state.
Use the [coverage summary](Test262Conformance.md) for current totals.

For resumable passing-but-unported discovery, see the [Test262 artifact catalog](Test262Catalog.md).
Its MVP-runner evidence is separate from the native conformance results.

Keep future batch narratives and detailed validation notes here rather than in
the table-first summary.

## Module-Code Coverage Batch (2026-11-06)

This batch adds 200 byte-identical standalone fixtures and 107 required sibling
dependencies from Test262 revision
`2b2ecead6e828dd9af13a9ec72065e645724a50f`. The standalone cases comprise 108
root module-code cases, 35 namespace internals, 32 top-level-await cases,
17 top-level-await syntax cases, five ambiguous-export cases, two namespace
cases, and one import-attributes case. None depends on eval; no skips or
execution snapshots were added.

Selection used scoped, read-only Supabase reporting, reconciled against all
48 existing unique module-code cases and pending PRs. The first 200 of 250
runnable, unregistered leads were selected; central incomplete results were
discovery evidence, not proof that all leads failed. The initial native run
passed 115 of 248 registrations and failed 133.

Compiler/runtime fixes preserve live default reads, normalize re-exported
imports to their underlying binding identity, omit ambiguous star names,
initialize exported var cells before dependency evaluation, and mirror
destructuring initialization into export cells. Namespace inventories now
include all resolved names before evaluation. Strict module mutation throws
TypeError without reading an uninitialized binding; descriptor/hasOwnProperty
queries do read bindings, while the in operator does not. Anonymous default
function/generator names no longer expose synthetic Closure names.

All native ESM graphs now carry evaluation metadata. Asynchronous cycle roots
govern external dependency completion and deferred readiness. Dynamic import
uses nonblocking loading instead of synchronous require's nested event-loop
pump, then awaits the cached evaluation capability. This preserves repeated
import order and leaf-to-root fulfillment/rejection. Dynamic CommonJS star
forwarding remains live even when the consumer has a static export inventory.
The screening host forwards the explicit .NET stack-size and package-cache
settings while retaining its credential-free worker environment.

Fresh local validation covers all 248 native registrations and all 250 required
variants, including 202 variants of the selected 200 cases. Representative
repaired cases include `get-str-found-uninit`, `delete-exported-uninit`,
`instn-named-bndng-dflt-fun-anon`, `instn-named-bndng-dflt-gen-anon`,
`dynamic-import-of-waiting-module`, `fulfillment-order`, `rejection-order`,
and `pending-async-dep-from-cycle`. Module-code coverage becomes 248/594
(41.75%); language coverage becomes 22,276/23,643 (94.22%); overall coverage
becomes 40,510/48,241 (83.97%). Support files and execution variants do not
increase standalone counts.

The initial central target was native/master provenance
`1e7df726-2470-518d-ad69-e4a4c5180958`, source/validation revision
`c820427b30d07f0cce81911344600ec62976fb6b`, as of
2026-11-06 06:23:05 UTC. After Promise PR #2458 merged, a bounded 400-path
reconciliation found all 200 Promise cases mapped and the 200 selected module
cases still unregistered/incomplete. That coherent sealed registration target
used provenance `66d43a1c-795a-50bb-9e11-60cab4454f7a`, snapshot
`2c05c9aa-1804-51eb-a204-3f010b899618`, validation
`0d1552d7-abf2-5a38-939a-155a7bb7cbbf`, source/validation revision
`61597c5edc33e5404ef24cd9c39a5b7122c9b909`, as of
2026-11-06 06:27:41 UTC. The observed-master field still lagged at `c820427...`;
the registration and validation revisions themselves matched the exact merged
SHA, whose master CI and central refresh succeeded.

These branch-local native results validate the PR; they do not publish trusted
Supabase acceptance or refresh central registrations. Exact-SHA master CI and
a central registration refresh are still required after this batch merges.

## Promise Coverage Batch (2026-11-06)

This batch adds 200 byte-identical fixtures from Test262 revision
`2b2ecead6e828dd9af13a9ec72065e645724a50f`: 62 Promise.any, 55 then,
43 race, 12 allSettledKeyed, ten allSettled, eight finally, and ten constructor
or static-property cases. Selection took the first 200 complete-path-sorted
unregistered Promise cases out of 226; 26 remain deferred. None of the selected
cases depends on eval, so no eval skips were added.

Four preserved upstream cases initially failed. Runtime fixes ignore
non-callable reaction handlers, make the observable CatchFinally thrower throw
the original rejection reason, and select a missing newTarget prototype from
the constructor's realm. The original failing cases were
`prototype/then/S25.4.5.3_A4.2_T2.js`,
`prototype/finally/rejected-observable-then-calls-argument.js`,
`prototype/finally/rejected-observable-then-calls-PromiseResolve.js`, and
`proto-from-ctor-realm.js`, relative to `test/built-ins/Promise/`.

Fresh local native validation passes all 652 Promise xUnit registrations and
all 396 metadata-required variants of the selected batch (198 strict and 198
non-strict) under one frozen build. All four original failures and 56 existing
runtime-intrinsic isolation regressions pass. Existing fixtures and upstream
assertions were not changed. These are branch-local PR results, not trusted
central native acceptance.

Selection used authorized read-only Supabase reporting, reconciled after #2456
against master `4b436ee84c721ff67ea4d9706198ea7d15ec1c8f`. The refreshed target
has provenance `6b7cc464-2a04-5031-9dd9-c131a42e03e9`, corpus
`024b79d7-0f92-5f9d-ade7-8bdf3907e097`, and registration snapshot
`1dd473dc-ef8a-5e89-b977-8f15c84b05a4`, as of
`2026-11-06 04:02:28.507085+00:00`. All 200 selected paths were unregistered
with incomplete central observations; central reporting did not establish
their passing status. The exact-SHA master CI and central registration refresh
for the preceding URI batch succeeded before this reconciliation.

The previous Promise count included two byte-identical registrations of
`test/built-ins/Promise/prototype/Symbol.toStringTag.js`. Both registrations
remain intact, but the upstream case is counted once. The unique baseline is
therefore 451, not 452; adding 200 cases produces 651 of 677 verified passing
Promise cases and a net increase of 199 in the published aggregates.

## Earlier Coverage Updates

Class statements, await-using, and async-function statements now have outcomes
for every applicable pinned case. The remaining 154 results comprise 149 verified
passing cases, three unsupported eval dependencies, and two obsolete
`nonextensible-applies-to-private` proposal cases. ES2025 and Node.js permit
private elements on nonextensible objects; those two contrary proposal tests
remain explicitly excluded rather than changing compatible runtime behavior.
Native validation covers every required variant of the three supported
corpora. Catalogue reporting remains unavailable, so this is branch-local
evidence, not central trusted acceptance.

Overall and language totals now sum the published component rows. This also
corrects earlier inconsistent rollups; it does not reclassify a product failure
as passing or carry forward unaccounted passing results.

Array and Date now have verified passing results for all applicable tests at the
pinned revision. String's remaining twelve known-unsupported cases require eval.
All newly verified cases passed every required native execution variant on the
development branch. This is branch-local evidence; central reporting was
unavailable, and these results are not presented as central trusted acceptance.

Async-arrow-function, async-function, await, and delete expression features now
have results for every applicable pinned case. Six cases require unsupported
eval; all other cases have verified native results, including all required
variants of the newly verified cases. This completion uses the same explicitly
branch-local evidence scope.

The core-expression operator batch adds 197 registrations: 181 verified passing
cases (including 54 parse-negative cases) and 16 explicit eval skips. The
affected operator rows are reconciled against unique pinned files and fresh
native results for their already-registered cases, producing a net increase of
306 verified passing results in this report. These rows now contain 484 verified
passing cases and 16 known-unsupported eval cases. The remaining comma and
coalesce tail-call cases are verified by the follow-up below. Candidate
selection used current central registration reporting through the registered
MCP connector. Native validation remains branch-local, not central trusted
acceptance.

New-expression, property-accessor, and this-keyword groups now have results for
all applicable pinned cases: 78 verified passing and eight unsupported eval
cases. This batch adds 51 exact fixtures: 43 verified passing, including two
parse-negative cases, and eight eval skips. All required native variants of
both the new and previously registered cases were checked under one frozen
Release build. Catalogue MCP tools were unavailable for this batch, so
selection and acceptance are explicitly local diagnosis/PR validation, not
fresh central reporting or trusted central acceptance.

Yield expressions now have verified passing results for all 63 applicable
pinned cases, including all 123 required native execution variants. The 36
newly verified cases cover operand errors and delegated iterator next, return,
throw, result-access, and cleanup semantics. No case in this area depends on
unsupported eval. Selection and validation are branch-local: catalogue MCP
tools were unavailable, and the Data API denied the reporting schema. These
results are not presented as central trusted acceptance.

Dynamic import now has results for every applicable pinned case: 940 verified
passing and one unsupported eval case. All 1,776 required native variants of the
940 runnable cases pass under one frozen Release build. Coverage includes
specifier and options evaluation, import attributes, namespace exotic-object
operations, cached evaluation failures, source-phase rejection for JavaScript,
and deferred evaluation with asynchronous dependencies. These results are
branch-local; catalogue MCP tools were unavailable and the Data API denied the
reporting schema. They are not central trusted acceptance.

The expression batch adds 200 exact pinned cases: 147 verified passing across
all 263 required native variants and 53 explicit eval exclusions. Coverage
includes super references and construction, tagged templates, import.meta,
new.target, calls, assignments, function/generator grammar, and operators.
All 1,987 runnable cases in the affected feature rows passed their 3,742
required native variants during branch-local reconciliation. The new cases
were checked again after the constructor regression repairs. Catalogue MCP
tools were unavailable and the Data API denied the reporting schema; these
results are not central trusted acceptance.

The affected rows and expression aggregate are reconciled to unique pinned
files, including correction of the assignment denominator from 489 to 485.
Language and overall aggregates now sum their visible area rows. Earlier
aggregate overcounts are corrected rather than carried forward as verified
coverage.

The expression follow-up adds 16 exact pinned cases: ten verified passing
across all 17 required native variants and six explicit eval-related
exclusions. Seven cases verify template-literal syntax errors; three verify
direct tail calls through nullish coalescing and final comma operands at the
upstream recursion depth of 100,000. Three exclusions call a rebound identifier
named `eval`, rather than the intrinsic eval function; calls through that
binding are currently rejected by the compiler. All 204 runnable cases in the
five affected feature rows were rechecked with their required native variants.
This is branch-local evidence, not central trusted acceptance.

The unsigned-right-shift row previously claimed its eval-dependent whitespace
case as passing. It is now correctly classified as known unsupported, so the
net increase in verified results is nine rather than ten. Three expression
cases remain without a published result: a dynamic tail call and two
tagged-template tail calls, which require general proper-tail-call support.

Identifiers and static imports now have verified passing results for all 395
applicable pinned cases: 268 identifier cases and 127 import cases. The 233
newly verified cases comprise 116 identifier syntax negatives and 117 import
cases, with no eval-related exclusions. All 668 required native variants
across both complete areas pass under one frozen Release build. Coverage
includes static JSON/text/immutable-byte attributes, deferred namespaces,
asynchronous dependency ordering and readiness, cached failures, and class
initialization against module namespace objects. Resolution-negative checks
require a successfully parsed entry and a verified dependency syntax or
linking failure; missing files and unrelated compiler failures do not qualify.
Selection and validation are explicitly branch-local: catalogue MCP tools
were unavailable and the Data API denied the reporting schema. These results
are not central trusted acceptance.

The first RegExp property-escape batch verifies 200 cases: 175 Unicode Script
cases and 25 Script_Extensions cases. All 400 required strict/non-strict variants
pass under one frozen Release build. Coverage exercises Unicode 17 ranges,
exact long/short aliases, script-extension overrides, complements, astral
characters, and lone surrogates. No case in this batch requires eval.
Script-property matching also preserves mode-specific ignoreCase complement
ordering and scoped i modifiers. Selection and native validation are
branch-local: catalogue MCP tools were unavailable and the reporting Data API
denied access. These results are not central trusted acceptance.

The second RegExp batch verifies another 200 cases: the remaining 150 generated
Script_Extensions cases, all 38 generated General_Category cases, and twelve
binary-property cases. All 400 required strict/non-strict variants pass under
one frozen Release build, with no eval-related exclusions. General-category
and selected binary-property matching now uses independently pinned Unicode 17
ranges and exact aliases, including unassigned code points, composite
categories, astral characters, and lone surrogates. Mode-specific case-folding
complement order also applies to these properties. The two batches together
verify every generated Script and Script_Extensions case. Selection and native
validation have the same branch-local scope and central-access limitation as
the first batch; these are not central trusted acceptance results.

The final core RegExp batch verifies 294 more cases across all 588 required
strict/non-strict variants and explicitly excludes seven direct-eval cases.
All 1,879 pinned core `built-ins/RegExp` cases now have an outcome: 1,872
verified passing and seven known unsupported. The separately reported
Annex B RegExp area is not included in this total. Coverage includes the
remaining binary properties, all seven emoji properties of strings, nested
v-mode unions/intersections/subtractions and string disjunctions, scoped
modifiers, JavaScript character escapes, constructor coercion and early
errors, Unicode capture names, capture resets, nullable repetitions, and
large quantifier bounds. Official Unicode 17 UCD and emoji files are
independently checksum-pinned. Long concatenation chains also exposed and
fixed exponential compiler AST traversal. Selection and native validation
remain branch-local because central reporting access was unavailable; these
results are not central trusted acceptance.

Classic `for`, function declarations, generator declarations, and `if` statements
now have published outcomes for all 1,171 applicable pinned cases: 1,121 verified
passing and 50 known unsupported eval dependencies. The completion adds 74 exact
upstream cases: 38 runnable cases and 36 explicit eval exclusions. It also
reconciles seven already-verified `for` cases previously reported without a
published result, giving a net increase of 45 verified results. Existing
parse-negative cases remain verified, not excluded merely for mentioning eval.

Coverage includes the first per-iteration lexical environment, implicit arguments
initialization with `var arguments`, statically prepared `Function.call`
construction, generator parameter initialization and nested destructuring.
All 2,106 metadata-required native variants of the 1,121 supported cases pass
under one unchanged build. Strict-only preparation preserves leading string-valued
expressions. Selection
used central reporting for the matching master revision and upstream pin; that
target had no native observations. Fresh execution results are branch-local PR
validation, not central trusted acceptance.

Labeled statements, `let` declarations, `return`, `switch`, `try`, and synchronous
`using` declarations now have outcomes for all 575 applicable pinned cases:
532 verified passing and 43 unsupported eval dependencies. The completion adds
141 exact upstream cases: 100 verified passing and 41 explicit eval exclusions.
Two older labeled completion cases now use their original eval-dependent sources
and are correctly excluded instead of counting rewritten scenarios as passing.
Revalidating eleven previously registered but unpublished cases gives a net
increase of 109 verified passing results and resolves all 152 unpublished results
in these six rows.

Coverage includes captured lexical writes during the temporal dead zone,
catch-pattern defaults and their separate parameter/body environments,
proper tail returns from catch/finally, return grammar and line terminators,
switch lexical isolation, and resource initialization/disposal ordering,
receivers, lifetimes, and suppressed errors. All 1,030 metadata-required native
variants of the 532 supported cases pass under one unchanged build, including
all 174 syntax-negative cases. Parse negatives mentioning eval remain runnable,
as does an ordinary object method named `eval`.
Selection used read-only central reporting for the matching master revision
and pin; its native rows were incomplete or excluded, not fresh acceptance.
The new execution results are branch-local PR validation, not central trusted
acceptance.

Literals, line terminators, reserved words, rest parameters, statement lists,
language types, and white space now have outcomes for all 873 applicable pinned
cases: 774 verified passing and 99 unsupported eval dependencies. The completion
accounts for 132 additional cases: 33 verified passing and 99 explicit eval
exclusions. Unique-file accounting also corrects six previously double-counted
literal results, giving a net increase of 27 verified passing results and
resolving all 126 unpublished results in these seven rows.

Coverage includes Unicode RegExp scalar quantification, primitive-property
assignment receivers and inherited Proxy behavior, line terminators in comments,
module reserved words, rest-parameter construction and early errors, and
language-type values and references. All 1,503 required native variants of the
774 supported cases pass under one unchanged build, including all 367
syntax-negative cases. Runtime-negative cases must throw their declared error
type; syntax negatives must fail during parsing under their metadata-selected
strictness and parse goal. Eval-dependent cases remain excluded without rewriting
their original assertions. Selection used read-only central reporting for the
matching master revision and pin; the target had no native observations or
trusted acceptance. Fresh execution results are branch-local PR validation,
not central trusted acceptance.

## Native test runner

Each runnable `[Fact]` in `tests/Jroc.Test262.Tests` remains an independent xUnit case.
During execution, the runner discovers registered (non-skipped) fixture calls in the
C# files beside each `JavaScript/` folder and compiles those entries into one
in-memory assembly for that folder. Nested fixtures under the same `JavaScript/`
directory share it; a nested test directory with its own `JavaScript/` directory
has a separate assembly. Logical entry names avoid CLR identifier collisions,
while the original fixture paths remain the source paths for debugging.
Fixtures marked `flags: [module]` explicitly select the Module parse goal
without changing their paths, even when their source has no import or export declarations.
Other fixtures select the standard Script goal, rejecting top-level return and
import/export declarations and instantiating realm-wide global declarations.
Script-goal entries use private logical paths while retaining their original
source paths for diagnostics. Importing the original path therefore creates a
distinct Module Record rather than reusing the Script Record. Computed imports
bundle sibling module/JSON support files without executing them as entries.
The native `$262.evalScript` helper compiles additional global scripts into
collectible assemblies in the current realm; it does not implement JavaScript
direct or indirect `eval`. Script completion values are not currently exposed
by this host helper.
The assembly is loaded once per folder and unloaded when the xUnit execution
run ends; each case executes only its own entry with fresh runtime services,
test262 helpers, async completion, and output capture.

Expected parse/compilation-failure facts are deliberately **not** part of the
shared successful assembly: they retain separate in-memory compilation and
their own diagnostic assertions. Skipped facts and support files are not
compiled as independent entries. An unexpected failure building a runnable
folder fails its test cases rather than retrying them as individual assemblies.
Resolution-negative SyntaxError facts additionally require valid entry syntax
and a recognized linking diagnostic or an independently verified dependency
syntax error, without executing the module.
Worker-based Atomics tests use a native `$262.agent` host implementation with
independent compiled agent scripts, shared buffer backing stores, reports, and
bounded worker cleanup.

Fixtures including `regExpUtils.js` receive native `buildString`,
`testPropertyEscapes`, `testPropertyOfStrings`, its identical
`testExtendedCharacterClass` alias, and `matchValidator` helpers. String construction
preserves code points, lone surrogates, and argument coercion order. Property
checks first try the complete string, then retain the upstream per-code-point
fallback, callback receiver/order, assertion messages, and iterator closing.
String-property checks retain the upstream complete-string and conditional
per-string fallback behavior. The standard host `print` binding is also available.

## Additional Coverage Updates

Class-declaration elements have 309 additional verified passing results
across all 618 required native variants and 30 known unsupported eval cases.
Coverage includes field and private-member forms,
computed names, initialization, valid syntax, and private assignment targets
in destructuring and loop heads. Subsequent completion results below cover
additional private-brand, computed-name, and indirect-super initialization
semantics; eval and the documented decorator limitations remain unsupported.
Catalogue reporting was unavailable, so these results are branch-local
native validation, not central trusted acceptance.

Class-declaration methods have 352 further verified passing results across all
704 required native variants. These cover 350 async-generator method parameter
destructuring cases, including defaults, rest, nested patterns, iterator cleanup,
static methods, and private methods, plus both computed getter/setter name cases.
Computed accessors preserve their get/set prefixes and Symbol descriptions on
instance and static methods. None of these cases requires eval. These are
branch-local native results; catalogue reporting remained unavailable.

Class-generator methods have 400 additional verified passing results across all
790 required native variants: 266 parameter-destructuring cases and 134
async-generator method cases. Coverage includes private/static methods, defaults,
rest and nested patterns, iterator cleanup, promise rejection, delegated
iteration, and restricted caller/arguments properties. These cases require no
product or harness changes, and none requires eval. Results are branch-local
native validation, not central trusted acceptance; catalogue reporting remained
unavailable.

Async class methods and elements have 400 additional verified passing results
across all 790 required native variants. Coverage includes private/static async
methods, async-generator delegation and promise rejection, default parameters,
lexical arguments/new.target, restricted caller/arguments properties, and element
names, separators, and private-member forms. All selected cases pass without
product or harness changes, and none requires eval. These are branch-local native
results, not central trusted acceptance; catalogue reporting remained unavailable.

The class-definition follow-up registers 339 exact pinned fixtures: 265 verified
passing cases across all 505 required native variants and 74 explicit eval skips.
Coverage includes computed names, method restrictions, fields, subclass
initialization, and class grammar. Static-block var declarations now remain in
their own block, and new.target is undefined in static blocks and their lexical
arrows without changing ordinary nested functions.

The follow-up completes the class-statement corpus with 4,261 verified passing
cases, 104 unsupported eval dependencies, and two excluded obsolete proposal
cases. It covers the previously deferred private-brand lifecycle, inheritance,
built-in subclasses, static initialization, and noncomputed public element
decorators. Computed/private decorated elements and decorated auto-accessors
remain unsupported. Selection and native validation are explicitly
offline/branch-local because catalogue MCP tools were unavailable and the Data
API rejected the reporting schema. These are not central trusted acceptance
results.

Variable and with statements now have outcomes for all 359 applicable pinned
cases: 163 verified variable-statement cases and 15 unsupported eval cases,
plus 164 verified with-statement cases and 17 unsupported eval cases. Every
required native variant of the 327 supported cases passed under one frozen
Release build. Coverage includes strict binding-name early errors, declaration
and destructuring grammar, primitive with environments, captured outer variables,
dynamic constant shadowing, Symbol.unscopables, Proxy trap order, and strict
reads after binding deletion. Eval-dependent cases include direct calls and
calls through aliases; syntax negatives restricting the identifier eval do not
execute eval and remain verified.

Previously unpublished results in these two rows are reconciled against the
same fresh native evidence, producing a net increase of 292 verified results.
This remains explicitly branch-local native evidence, not central trusted
acceptance.

Function gains ten previously unpublished outcomes: four verified private-method
`toString` cases and six known-unsupported eval-dependent caller cases. Private
instance and static methods in both class declarations and expressions now
produce valid native-function syntax. All eight required strict/non-strict
variants of the four supported cases pass. Selection used recent central native
failure evidence reconciled with current master coverage; validation of the fix
is branch-local, not central trusted acceptance.

## URI Decoder Coverage (2026-11-06)

The URI-decoding batch adds 100 verified passing cases: 45 `decodeURI` and
55 `decodeURIComponent` tests. All 200 required strict/non-strict variants pass,
as does the 110-case native xUnit suite including ten existing `decodeURI` cases.
The batch covers malformed UTF-8 and percent escapes, valid multibyte sequences,
reserved delimiters, string coercion, property descriptors, and nonconstructibility.
No selected case requires eval, and no product or harness fix was needed.

Selection used the first 100 path-sorted, unregistered decoder cases from scoped
central native reporting, reconciled against master
`b3d19d44e3ca6eb50697a7c392b49883db824ac8` with no pending PRs. Central observations
were incomplete, not evidence of passing or failure; acceptance is fresh
branch-local native validation, not trusted central acceptance.
The remaining `decodeURIComponent/throw-URIError.js` case was outside the requested
100-case batch. `decodeURI` now has results for all 55 pinned cases;
`decodeURIComponent` has 55 verified results out of 56.
