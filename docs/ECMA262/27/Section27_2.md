# Section 27.2: Promise Objects

[Back to Section27](Section27.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 27.2 | Promise Objects | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 27.2.1 | Promise Abstract Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-promise-abstract-operations) |
| 27.2.1.1 | PromiseCapability Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-promisecapability-records) |
| 27.2.1.1.1 | IfAbruptRejectPromise ( value , capability ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ifabruptrejectpromise) |
| 27.2.1.2 | PromiseReaction Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-promisereaction-records) |
| 27.2.1.3 | CreateResolvingFunctions ( promise ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createresolvingfunctions) |
| 27.2.1.4 | FulfillPromise ( promise , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-fulfillpromise) |
| 27.2.1.5 | NewPromiseCapability ( C ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newpromisecapability) |
| 27.2.1.6 | IsPromise ( x ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ispromise) |
| 27.2.1.7 | RejectPromise ( promise , reason ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-rejectpromise) |
| 27.2.1.8 | TriggerPromiseReactions ( reactions , argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-triggerpromisereactions) |
| 27.2.1.9 | HostPromiseRejectionTracker ( promise , operation ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-host-promise-rejection-tracker) |
| 27.2.2 | Promise Jobs | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-promise-jobs) |
| 27.2.2.1 | NewPromiseReactionJob ( reaction , argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newpromisereactionjob) |
| 27.2.2.2 | NewPromiseResolveThenableJob ( promiseToResolve , thenable , then ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newpromiseresolvethenablejob) |
| 27.2.3 | The Promise Constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise-constructor) |
| 27.2.3.1 | Promise ( executor ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise-executor) |
| 27.2.4 | Properties of the Promise Constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-promise-constructor) |
| 27.2.4.1 | Promise.all ( iterable ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.all) |
| 27.2.4.1.1 | GetPromiseResolve ( promiseConstructor ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getpromiseresolve) |
| 27.2.4.1.2 | PerformPromiseAll ( iteratorRecord , constructor , resultCapability , promiseResolve ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-performpromiseall) |
| 27.2.4.2 | Promise.allSettled ( iterable ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.allsettled) |
| 27.2.4.2.1 | PerformPromiseAllSettled ( iteratorRecord , constructor , resultCapability , promiseResolve ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-performpromiseallsettled) |
| 27.2.4.3 | Promise.any ( iterable ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.any) |
| 27.2.4.3.1 | PerformPromiseAny ( iteratorRecord , constructor , resultCapability , promiseResolve ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-performpromiseany) |
| 27.2.4.4 | Promise.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.prototype) |
| 27.2.4.5 | Promise.race ( iterable ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.race) |
| 27.2.4.5.1 | PerformPromiseRace ( iteratorRecord , constructor , resultCapability , promiseResolve ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-performpromiserace) |
| 27.2.4.6 | Promise.reject ( r ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.reject) |
| 27.2.4.7 | Promise.resolve ( x ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.resolve) |
| 27.2.4.7.1 | PromiseResolve ( C , x ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-promise-resolve) |
| 27.2.4.8 | Promise.try ( callback , ... args ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.try) |
| 27.2.4.9 | Promise.withResolvers ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.withResolvers) |
| 27.2.4.10 | get Promise [ %Symbol.species% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-promise-%symbol.species%) |
| 27.2.5 | Properties of the Promise Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-promise-prototype-object) |
| 27.2.5.1 | Promise.prototype.catch ( onRejected ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.prototype.catch) |
| 27.2.5.2 | Promise.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.prototype.constructor) |
| 27.2.5.3 | Promise.prototype.finally ( onFinally ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.prototype.finally) |
| 27.2.5.4 | Promise.prototype.then ( onFulfilled , onRejected ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.prototype.then) |
| 27.2.5.4.1 | PerformPromiseThen ( promise , onFulfilled , onRejected [ , resultCapability ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-performpromisethen) |
| 27.2.5.5 | Promise.prototype [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-promise.prototype-%symbol.tostringtag%) |
| 27.2.6 | Properties of Promise Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-promise-instances) |

## Support

Feature-level support tracking with test script references.

### 27.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-promise-constructor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Promise constructor (executor), Promise.resolve, Promise.reject | Supported | [`Promise_Executor_Resolved.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Executor_Resolved.js)<br>[`Promise_Executor_Rejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Executor_Rejected.js)<br>[`Promise_Thenable_Resolve_Immediate.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_Resolve_Immediate.js)<br>[`Promise_Thenable_Resolve_Delayed.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_Resolve_Delayed.js)<br>[`Promise_Thenable_Reject.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_Reject.js)<br>[`Promise_Thenable_NonFunctionThen.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_NonFunctionThen.js)<br>[`Promise_Thenable_Nested.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_Nested.js) | Constructor accepts an executor delegate and supports resolving/rejecting, including thenable assimilation in Promise.resolve. |

### 27.2.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-promise.all))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Promise.all / allSettled / any / race | Supported | [`Promise_All_AllResolved.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_All_AllResolved.js)<br>[`Promise_All_OneRejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_All_OneRejected.js)<br>[`Promise_All_EmptyArray.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_All_EmptyArray.js)<br>[`Promise_AllSettled_MixedResults.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_AllSettled_MixedResults.js)<br>[`Promise_AllSettled_AllResolved.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_AllSettled_AllResolved.js)<br>[`Promise_AllSettled_AllRejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_AllSettled_AllRejected.js)<br>[`Promise_Any_FirstResolved.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Any_FirstResolved.js)<br>[`Promise_Any_AllRejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Any_AllRejected.js)<br>[`Promise_Race_FirstResolved.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Race_FirstResolved.js)<br>[`Promise_Race_FirstRejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Race_FirstRejected.js) |  |

### 27.2.4.9 ([tc39.es](https://tc39.es/ecma262/#sec-promise.withResolvers))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Promise.withResolvers() | Supported | [`Promise_WithResolvers_Resolve.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_WithResolvers_Resolve.js)<br>[`Promise_WithResolvers_Reject.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_WithResolvers_Reject.js)<br>[`Promise_WithResolvers_Idempotent.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_WithResolvers_Idempotent.js) |  |

### 27.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-promise-prototype-object))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Promise.prototype.then / catch / finally | Supported | [`Promise_Resolve_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_Then.js)<br>[`Promise_Reject_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Reject_Then.js)<br>[`Promise_Resolve_ThenFinally.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_ThenFinally.js)<br>[`Promise_Reject_FinallyCatch.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Reject_FinallyCatch.js)<br>[`Promise_Resolve_FinallyThen.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_FinallyThen.js)<br>[`Promise_Resolve_FinallyThrows.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_FinallyThrows.js)<br>[`Promise_Then_ReturnsResolvedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Then_ReturnsResolvedPromise.js)<br>[`Promise_Then_ReturnsRejectedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Then_ReturnsRejectedPromise.js)<br>[`Promise_Thenable_Returned_FromHandler.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_Returned_FromHandler.js)<br>[`Promise_Catch_ReturnsResolvedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Catch_ReturnsResolvedPromise.js)<br>[`Promise_Catch_ReturnsRejectedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Catch_ReturnsRejectedPromise.js)<br>[`Promise_Finally_ReturnsResolvedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Finally_ReturnsResolvedPromise.js)<br>[`Promise_Finally_ReturnsRejectedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Finally_ReturnsRejectedPromise.js)<br>[`Promise_Finally_ReturnsThenable_PassThrough_Fulfilled.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Finally_ReturnsThenable_PassThrough_Fulfilled.js)<br>[`Promise_Finally_ReturnsThenable_PassThrough_Rejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Finally_ReturnsThenable_PassThrough_Rejected.js)<br>[`Promise_Scheduling_StarvationTest.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Scheduling_StarvationTest.js) | Implements then/catch/finally with scheduling and thenable assimilation for returned values. Limitations remain around full spec-shaped prototype properties (constructor/toStringTag) and host rejection tracking. |

## Reference: Converted Spec Text

_Converted from `test_output/ecma262-27.2.html` via `scripts/ECMA262/convertEcmaExtractHtmlToMarkdown.js`._

<details>
<summary>Show converted ECMA-262 §27.2 text</summary>

<!-- BEGIN SPEC EXTRACT: test_output/ecma262-27.2.md -->

### 27.2 Promise Objects

A Promise is an object that is used as a placeholder for the eventual results of a deferred (and possibly asynchronous) computation.

Any Promise is in one of three mutually exclusive states: _fulfilled_, _rejected_, and _pending_:

-   A promise `p` is fulfilled if `p.then(f, r)` will immediately enqueue a [Job](executable-code-and-execution-contexts.html#job) to call the function `f`.
-   A promise `p` is rejected if `p.then(f, r)` will immediately enqueue a [Job](executable-code-and-execution-contexts.html#job) to call the function `r`.
-   A promise is pending if it is neither fulfilled nor rejected.

A promise is said to be _settled_ if it is not pending, i.e. if it is either fulfilled or rejected.

A promise is _resolved_ if it is settled or if it has been “locked in” to match the state of another promise. Attempting to resolve or reject a resolved promise has no effect. A promise is _unresolved_ if it is not resolved. An unresolved promise is always in the pending state. A resolved promise may be pending, fulfilled or rejected.

### 27.2.1 Promise Abstract Operations

### 27.2.1.1 PromiseCapability Records

A PromiseCapability Record is a [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) value used to encapsulate a Promise or promise-like object along with the functions that are capable of resolving or rejecting that promise. PromiseCapability Records are produced by the [NewPromiseCapability](control-abstraction-objects.html#sec-newpromisecapability) abstract operation.

PromiseCapability Records have the fields listed in [Table 88](control-abstraction-objects.html#table-promisecapability-record-fields).

Table 88: [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records) Fields

Field Name

Value

Meaning

`\[\[Promise\]\]`

an Object

An object that is usable as a promise.

`\[\[Resolve\]\]`

a [function object](ecmascript-data-types-and-values.html#function-object)

The function that is used to resolve the given promise.

`\[\[Reject\]\]`

a [function object](ecmascript-data-types-and-values.html#function-object)

The function that is used to reject the given promise.

### 27.2.1.1.1 IfAbruptRejectPromise ( `value`, `capability` )

IfAbruptRejectPromise is a shorthand for a sequence of algorithm steps that use a [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records). An algorithm step of the form:

1.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`value`, `capability`).

means the same thing as:

1.  [Assert](notational-conventions.html#assert): `value` is a [Completion Record](ecmascript-data-types-and-values.html#sec-completion-record-specification-type).
2.  If `value` is an [abrupt completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
	1.  Perform ? [Call](abstract-operations.html#sec-call)(`capability`.`\[\[Reject\]\]`, `undefined`, « `value`.`\[\[Value\]\]` »).
	2.  Return `capability`.`\[\[Promise\]\]`.
3.  Else,
	1.  Set `value` to ! `value`.

### 27.2.1.2 PromiseReaction Records

A PromiseReaction Record is a [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) value used to store information about how a promise should react when it becomes resolved or rejected with a given value. PromiseReaction Records are created by the [PerformPromiseThen](control-abstraction-objects.html#sec-performpromisethen) abstract operation, and are used by the [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) returned by [NewPromiseReactionJob](control-abstraction-objects.html#sec-newpromisereactionjob).

PromiseReaction Records have the fields listed in [Table 89](control-abstraction-objects.html#table-promisereaction-record-fields).

Table 89: [PromiseReaction Record](control-abstraction-objects.html#sec-promisereaction-records) Fields

Field Name

Value

Meaning

`\[\[Capability\]\]`

a [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records) or `undefined`

The capabilities of the promise for which this record provides a reaction handler.

`\[\[Type\]\]`

`fulfill` or `reject`

The `\[\[Type\]\]` is used when `\[\[Handler\]\]` is `empty` to allow for behaviour specific to the settlement type.

`\[\[Handler\]\]`

a [JobCallback Record](executable-code-and-execution-contexts.html#sec-jobcallback-records) or `empty`

The function that should be applied to the incoming value, and whose return value will govern what happens to the derived promise. If `\[\[Handler\]\]` is `empty`, a function that depends on the value of `\[\[Type\]\]` will be used instead.

### 27.2.1.3 CreateResolvingFunctions ( `promise` )

The abstract operation CreateResolvingFunctions takes argument `promise` (a Promise) and returns a [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) with fields `\[\[Resolve\]\]` (a [function object](ecmascript-data-types-and-values.html#function-object)) and `\[\[Reject\]\]` (a [function object](ecmascript-data-types-and-values.html#function-object)). It performs the following steps when called:

1.  Let `alreadyResolved` be the [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) { `\[\[Value\]\]`: `false` }.
2.  Let `resolveSteps` be a new [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with parameters (`resolution`) that captures `promise` and `alreadyResolved` and performs the following steps when called:
	1.  If `alreadyResolved`.`\[\[Value\]\]` is `true`, return `undefined`.
	2.  Set `alreadyResolved`.`\[\[Value\]\]` to `true`.
	3.  If [SameValue](abstract-operations.html#sec-samevalue)(`resolution`, `promise`) is `true`, then
		1.  Let `selfResolutionError` be a newly created `TypeError` object.
		2.  Perform [RejectPromise](control-abstraction-objects.html#sec-rejectpromise)(`promise`, `selfResolutionError`).
		3.  Return `undefined`.
	4.  If `resolution` [is not an Object](ecmascript-data-types-and-values.html#sec-object-type), then
		1.  Perform [FulfillPromise](control-abstraction-objects.html#sec-fulfillpromise)(`promise`, `resolution`).
		2.  Return `undefined`.
	5.  Let `then` be [Completion](notational-conventions.html#sec-completion-ao)([Get](abstract-operations.html#sec-get-o-p)(`resolution`, `"then"`)).
	6.  If `then` is an [abrupt completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
		1.  Perform [RejectPromise](control-abstraction-objects.html#sec-rejectpromise)(`promise`, `then`.`\[\[Value\]\]`).
		2.  Return `undefined`.
	7.  Let `thenAction` be `then`.`\[\[Value\]\]`.
	8.  If [IsCallable](abstract-operations.html#sec-iscallable)(`thenAction`) is `false`, then
		1.  Perform [FulfillPromise](control-abstraction-objects.html#sec-fulfillpromise)(`promise`, `resolution`).
		2.  Return `undefined`.
	9.  Let `thenJobCallback` be [HostMakeJobCallback](executable-code-and-execution-contexts.html#sec-hostmakejobcallback)(`thenAction`).
	10.  Let `job` be [NewPromiseResolveThenableJob](control-abstraction-objects.html#sec-newpromiseresolvethenablejob)(`promise`, `resolution`, `thenJobCallback`).
	11.  Perform [HostEnqueuePromiseJob](executable-code-and-execution-contexts.html#sec-hostenqueuepromisejob)(`job`.`\[\[Job\]\]`, `job`.`\[\[Realm\]\]`).
	12.  Return `undefined`.
3.  Let `resolve` be [CreateBuiltinFunction](ordinary-and-exotic-objects-behaviours.html#sec-createbuiltinfunction)(`resolveSteps`, 1, `""`, « »).
4.  Let `rejectSteps` be a new [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with parameters (`reason`) that captures `promise` and `alreadyResolved` and performs the following steps when called:
	1.  If `alreadyResolved`.`\[\[Value\]\]` is `true`, return `undefined`.
	2.  Set `alreadyResolved`.`\[\[Value\]\]` to `true`.
	3.  Perform [RejectPromise](control-abstraction-objects.html#sec-rejectpromise)(`promise`, `reason`).
	4.  Return `undefined`.
5.  Let `reject` be [CreateBuiltinFunction](ordinary-and-exotic-objects-behaviours.html#sec-createbuiltinfunction)(`rejectSteps`, 1, `""`, « »).
6.  Return the [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) { `\[\[Resolve\]\]`: `resolve`, `\[\[Reject\]\]`: `reject` }.

### 27.2.1.4 FulfillPromise ( `promise`, `value` )

The abstract operation FulfillPromise takes arguments `promise` (a Promise) and `value` (an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types)) and returns `unused`. It performs the following steps when called:

1.  [Assert](notational-conventions.html#assert): `promise`.`\[\[PromiseState\]\]` is `pending`.
2.  Let `reactions` be `promise`.`\[\[PromiseFulfillReactions\]\]`.
3.  Set `promise`.`\[\[PromiseResult\]\]` to `value`.
4.  Set `promise`.`\[\[PromiseFulfillReactions\]\]` to `undefined`.
5.  Set `promise`.`\[\[PromiseRejectReactions\]\]` to `undefined`.
6.  Set `promise`.`\[\[PromiseState\]\]` to `fulfilled`.
7.  Perform [TriggerPromiseReactions](control-abstraction-objects.html#sec-triggerpromisereactions)(`reactions`, `value`).
8.  Return `unused`.

### 27.2.1.5 NewPromiseCapability ( `C` )

The abstract operation NewPromiseCapability takes argument `C` (an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types)) and returns either a [normal completion containing](ecmascript-data-types-and-values.html#sec-completion-record-specification-type) a [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records) or a [throw completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type). It attempts to use `C` as a [constructor](ecmascript-data-types-and-values.html#constructor) in the fashion of the built-in Promise [constructor](ecmascript-data-types-and-values.html#constructor) to create a promise and extract its `resolve` and `reject` functions. The promise plus the `resolve` and `reject` functions are used to initialize a new [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records). It performs the following steps when called:

1.  If [IsConstructor](abstract-operations.html#sec-isconstructor)(`C`) is `false`, throw a `TypeError` exception.
2.  NOTE: `C` is assumed to be a [constructor](ecmascript-data-types-and-values.html#constructor) function that supports the parameter conventions of the Promise [constructor](ecmascript-data-types-and-values.html#constructor) (see [27.2.3.1](control-abstraction-objects.html#sec-promise-executor)).
3.  Let `resolvingFunctions` be the [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) { `\[\[Resolve\]\]`: `undefined`, `\[\[Reject\]\]`: `undefined` }.
4.  Let `executorClosure` be a new [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with parameters (`resolve`, `reject`) that captures `resolvingFunctions` and performs the following steps when called:
	1.  If `resolvingFunctions`.`\[\[Resolve\]\]` is not `undefined`, throw a `TypeError` exception.
	2.  If `resolvingFunctions`.`\[\[Reject\]\]` is not `undefined`, throw a `TypeError` exception.
	3.  Set `resolvingFunctions`.`\[\[Resolve\]\]` to `resolve`.
	4.  Set `resolvingFunctions`.`\[\[Reject\]\]` to `reject`.
	5.  Return [NormalCompletion](ecmascript-data-types-and-values.html#sec-normalcompletion)(`undefined`).
5.  Let `executor` be [CreateBuiltinFunction](ordinary-and-exotic-objects-behaviours.html#sec-createbuiltinfunction)(`executorClosure`, 2, `""`, « »).
6.  Let `promise` be ? [Construct](abstract-operations.html#sec-construct)(`C`, « `executor` »).
7.  If [IsCallable](abstract-operations.html#sec-iscallable)(`resolvingFunctions`.`\[\[Resolve\]\]`) is `false`, throw a `TypeError` exception.
8.  If [IsCallable](abstract-operations.html#sec-iscallable)(`resolvingFunctions`.`\[\[Reject\]\]`) is `false`, throw a `TypeError` exception.
9.  Return the [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records) { `\[\[Promise\]\]`: `promise`, `\[\[Resolve\]\]`: `resolvingFunctions`.`\[\[Resolve\]\]`, `\[\[Reject\]\]`: `resolvingFunctions`.`\[\[Reject\]\]` }.

Note

This abstract operation supports Promise subclassing, as it is generic on any [constructor](ecmascript-data-types-and-values.html#constructor) that calls a passed executor function argument in the same way as the Promise [constructor](ecmascript-data-types-and-values.html#constructor). It is used to generalize static methods of the Promise [constructor](ecmascript-data-types-and-values.html#constructor) to any subclass.

### 27.2.1.6 IsPromise ( `x` )

The abstract operation IsPromise takes argument `x` (an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types)) and returns a Boolean. It checks for the promise brand on an object. It performs the following steps when called:

1.  If `x` [is not an Object](ecmascript-data-types-and-values.html#sec-object-type), return `false`.
2.  If `x` does not have a `\[\[PromiseState\]\]` internal slot, return `false`.
3.  Return `true`.

### 27.2.1.7 RejectPromise ( `promise`, `reason` )

The abstract operation RejectPromise takes arguments `promise` (a Promise) and `reason` (an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types)) and returns `unused`. It performs the following steps when called:

1.  [Assert](notational-conventions.html#assert): `promise`.`\[\[PromiseState\]\]` is `pending`.
2.  Let `reactions` be `promise`.`\[\[PromiseRejectReactions\]\]`.
3.  Set `promise`.`\[\[PromiseResult\]\]` to `reason`.
4.  Set `promise`.`\[\[PromiseFulfillReactions\]\]` to `undefined`.
5.  Set `promise`.`\[\[PromiseRejectReactions\]\]` to `undefined`.
6.  Set `promise`.`\[\[PromiseState\]\]` to `rejected`.
7.  If `promise`.`\[\[PromiseIsHandled\]\]` is `false`, perform [HostPromiseRejectionTracker](control-abstraction-objects.html#sec-host-promise-rejection-tracker)(`promise`, `"reject"`).
8.  Perform [TriggerPromiseReactions](control-abstraction-objects.html#sec-triggerpromisereactions)(`reactions`, `reason`).
9.  Return `unused`.

### 27.2.1.8 TriggerPromiseReactions ( `reactions`, `argument` )

The abstract operation TriggerPromiseReactions takes arguments `reactions` (a [List](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) of [PromiseReaction Records](control-abstraction-objects.html#sec-promisereaction-records)) and `argument` (an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types)) and returns `unused`. It enqueues a new [Job](executable-code-and-execution-contexts.html#job) for each record in `reactions`. Each such [Job](executable-code-and-execution-contexts.html#job) processes the `\[\[Type\]\]` and `\[\[Handler\]\]` of the [PromiseReaction Record](control-abstraction-objects.html#sec-promisereaction-records), and if the `\[\[Handler\]\]` is not `empty`, calls it passing the given argument. If the `\[\[Handler\]\]` is `empty`, the behaviour is determined by the `\[\[Type\]\]`. It performs the following steps when called:

1.  For each element `reaction` of `reactions`, do
	1.  Let `job` be [NewPromiseReactionJob](control-abstraction-objects.html#sec-newpromisereactionjob)(`reaction`, `argument`).
	2.  Perform [HostEnqueuePromiseJob](executable-code-and-execution-contexts.html#sec-hostenqueuepromisejob)(`job`.`\[\[Job\]\]`, `job`.`\[\[Realm\]\]`).
2.  Return `unused`.

### 27.2.1.9 HostPromiseRejectionTracker ( `promise`, `operation` )

The [host-defined](overview.html#host-defined) abstract operation HostPromiseRejectionTracker takes arguments `promise` (a Promise) and `operation` (`"reject"` or `"handle"`) and returns `unused`. It allows [host environments](overview.html#host-environment) to track promise rejections.

The default implementation of HostPromiseRejectionTracker is to return `unused`.

Note 1

HostPromiseRejectionTracker is called in two scenarios:

-   When a promise is rejected without any handlers, it is called with its `operation` argument set to `"reject"`.
-   When a handler is added to a rejected promise for the first time, it is called with its `operation` argument set to `"handle"`.

A typical implementation of HostPromiseRejectionTracker might try to notify developers of unhandled rejections, while also being careful to notify them if such previous notifications are later invalidated by new handlers being attached.

Note 2

If `operation` is `"handle"`, an implementation should not hold a reference to `promise` in a way that would interfere with garbage collection. An implementation may hold a reference to `promise` if `operation` is `"reject"`, since it is expected that rejections will be rare and not on hot code paths.

### 27.2.2 Promise Jobs

### 27.2.2.1 NewPromiseReactionJob ( `reaction`, `argument` )

The abstract operation NewPromiseReactionJob takes arguments `reaction` (a [PromiseReaction Record](control-abstraction-objects.html#sec-promisereaction-records)) and `argument` (an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types)) and returns a [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) with fields `\[\[Job\]\]` (a [Job](executable-code-and-execution-contexts.html#job) [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure)) and `\[\[Realm\]\]` (a [Realm Record](executable-code-and-execution-contexts.html#realm-record) or `null`). It returns a new [Job](executable-code-and-execution-contexts.html#job) [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) that applies the appropriate handler to the incoming value, and uses the handler's return value to resolve or reject the derived promise associated with that handler. It performs the following steps when called:

1.  Let `job` be a new [Job](executable-code-and-execution-contexts.html#job) [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with no parameters that captures `reaction` and `argument` and performs the following steps when called:
	1.  Let `promiseCapability` be `reaction`.`\[\[Capability\]\]`.
	2.  Let `type` be `reaction`.`\[\[Type\]\]`.
	3.  Let `handler` be `reaction`.`\[\[Handler\]\]`.
	4.  If `handler` is `empty`, then
		1.  If `type` is `fulfill`, then
			1.  Let `handlerResult` be [NormalCompletion](ecmascript-data-types-and-values.html#sec-normalcompletion)(`argument`).
		2.  Else,
			1.  [Assert](notational-conventions.html#assert): `type` is `reject`.
			2.  Let `handlerResult` be [ThrowCompletion](ecmascript-data-types-and-values.html#sec-throwcompletion)(`argument`).
	5.  Else,
		1.  Let `handlerResult` be [Completion](notational-conventions.html#sec-completion-ao)([HostCallJobCallback](executable-code-and-execution-contexts.html#sec-hostcalljobcallback)(`handler`, `undefined`, « `argument` »)).
	6.  If `promiseCapability` is `undefined`, then
		1.  [Assert](notational-conventions.html#assert): `handlerResult` is not an [abrupt completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type).
		2.  Return `empty`.
	7.  [Assert](notational-conventions.html#assert): `promiseCapability` is a [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records).
	8.  If `handlerResult` is an [abrupt completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
		1.  Return ? [Call](abstract-operations.html#sec-call)(`promiseCapability`.`\[\[Reject\]\]`, `undefined`, « `handlerResult`.`\[\[Value\]\]` »).
	9.  Else,
		1.  Return ? [Call](abstract-operations.html#sec-call)(`promiseCapability`.`\[\[Resolve\]\]`, `undefined`, « `handlerResult`.`\[\[Value\]\]` »).
2.  Let `handlerRealm` be `null`.
3.  If `reaction`.`\[\[Handler\]\]` is not `empty`, then
	1.  Let `getHandlerRealmResult` be [Completion](notational-conventions.html#sec-completion-ao)([GetFunctionRealm](abstract-operations.html#sec-getfunctionrealm)(`reaction`.`\[\[Handler\]\]`.`\[\[Callback\]\]`)).
	2.  If `getHandlerRealmResult` is a [normal completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), set `handlerRealm` to `getHandlerRealmResult`.`\[\[Value\]\]`.
	3.  Else, set `handlerRealm` to [the current Realm Record](executable-code-and-execution-contexts.html#current-realm).
	4.  NOTE: `handlerRealm` is never `null` unless the handler is `undefined`. When the handler is a revoked Proxy and no ECMAScript code runs, `handlerRealm` is used to create error objects.
4.  Return the [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) { `\[\[Job\]\]`: `job`, `\[\[Realm\]\]`: `handlerRealm` }.

### 27.2.2.2 NewPromiseResolveThenableJob ( `promiseToResolve`, `thenable`, `then` )

The abstract operation NewPromiseResolveThenableJob takes arguments `promiseToResolve` (a Promise), `thenable` (an Object), and `then` (a [JobCallback Record](executable-code-and-execution-contexts.html#sec-jobcallback-records)) and returns a [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) with fields `\[\[Job\]\]` (a [Job](executable-code-and-execution-contexts.html#job) [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure)) and `\[\[Realm\]\]` (a [Realm Record](executable-code-and-execution-contexts.html#realm-record)). It performs the following steps when called:

1.  Let `job` be a new [Job](executable-code-and-execution-contexts.html#job) [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with no parameters that captures `promiseToResolve`, `thenable`, and `then` and performs the following steps when called:
	1.  Let `resolvingFunctions` be [CreateResolvingFunctions](control-abstraction-objects.html#sec-createresolvingfunctions)(`promiseToResolve`).
	2.  Let `thenCallResult` be [Completion](notational-conventions.html#sec-completion-ao)([HostCallJobCallback](executable-code-and-execution-contexts.html#sec-hostcalljobcallback)(`then`, `thenable`, « `resolvingFunctions`.`\[\[Resolve\]\]`, `resolvingFunctions`.`\[\[Reject\]\]` »)).
	3.  If `thenCallResult` is an [abrupt completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
		1.  Return ? [Call](abstract-operations.html#sec-call)(`resolvingFunctions`.`\[\[Reject\]\]`, `undefined`, « `thenCallResult`.`\[\[Value\]\]` »).

	4.  Return ! `thenCallResult`.
2.  Let `getThenRealmResult` be [Completion](notational-conventions.html#sec-completion-ao)([GetFunctionRealm](abstract-operations.html#sec-getfunctionrealm)(`then`.`\[\[Callback\]\]`)).
3.  If `getThenRealmResult` is a [normal completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), let `thenRealm` be `getThenRealmResult`.`\[\[Value\]\]`.
4.  Else, let `thenRealm` be [the current Realm Record](executable-code-and-execution-contexts.html#current-realm).
5.  NOTE: `thenRealm` is never `null`. When `then`.`\[\[Callback\]\]` is a revoked Proxy and no code runs, `thenRealm` is used to create error objects.
6.  Return the [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) { `\[\[Job\]\]`: `job`, `\[\[Realm\]\]`: `thenRealm` }.

Note

This [Job](executable-code-and-execution-contexts.html#job) uses the supplied thenable and its `then` method to resolve the given promise. This process must take place as a [Job](executable-code-and-execution-contexts.html#job) to ensure that the evaluation of the `then` method occurs after evaluation of any surrounding code has completed.

### 27.2.3 The Promise Constructor

The Promise [constructor](ecmascript-data-types-and-values.html#constructor):

-   is %Promise%.
-   is the initial value of the `"Promise"` property of the [global object](global-object.html#sec-global-object).
-   creates and initializes a new Promise when called as a [constructor](ecmascript-data-types-and-values.html#constructor).
-   is not intended to be called as a function and will throw an exception when called in that manner.
-   may be used as the value in an `extends` clause of a class definition. Subclass [constructors](ecmascript-data-types-and-values.html#constructor) that intend to inherit the specified Promise behaviour must include a `super` call to the Promise [constructor](ecmascript-data-types-and-values.html#constructor) to create and initialize the subclass instance with the internal state necessary to support the `Promise` and `Promise.prototype` built-in methods.

### 27.2.3.1 Promise ( `executor` )

This function performs the following steps when called:

1.  If NewTarget is `undefined`, throw a `TypeError` exception.
2.  If [IsCallable](abstract-operations.html#sec-iscallable)(`executor`) is `false`, throw a `TypeError` exception.
3.  Let `promise` be ? [OrdinaryCreateFromConstructor](ordinary-and-exotic-objects-behaviours.html#sec-ordinarycreatefromconstructor)(NewTarget, `"%Promise.prototype%"`, « `\[\[PromiseState\]\]`, `\[\[PromiseResult\]\]`, `\[\[PromiseFulfillReactions\]\]`, `\[\[PromiseRejectReactions\]\]`, `\[\[PromiseIsHandled\]\]` »).
4.  Set `promise`.`\[\[PromiseState\]\]` to `pending`.
5.  Set `promise`.`\[\[PromiseResult\]\]` to `empty`.
6.  Set `promise`.`\[\[PromiseFulfillReactions\]\]` to a new empty [List](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type).
7.  Set `promise`.`\[\[PromiseRejectReactions\]\]` to a new empty [List](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type).
8.  Set `promise`.`\[\[PromiseIsHandled\]\]` to `false`.
9.  Let `resolvingFunctions` be [CreateResolvingFunctions](control-abstraction-objects.html#sec-createresolvingfunctions)(`promise`).
10.  Let `completion` be [Completion](notational-conventions.html#sec-completion-ao)([Call](abstract-operations.html#sec-call)(`executor`, `undefined`, « `resolvingFunctions`.`\[\[Resolve\]\]`, `resolvingFunctions`.`\[\[Reject\]\]` »)).
11.  If `completion` is an [abrupt completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
	 1.  Perform ? [Call](abstract-operations.html#sec-call)(`resolvingFunctions`.`\[\[Reject\]\]`, `undefined`, « `completion`.`\[\[Value\]\]` »).
12.  Return `promise`.

Note

The `executor` argument must be a [function object](ecmascript-data-types-and-values.html#function-object). It is called for initiating and reporting completion of the possibly deferred action represented by this Promise. The executor is called with two arguments: `resolve` and `reject`. These are functions that may be used by the `executor` function to report eventual completion or failure of the deferred computation. Returning from the executor function does not mean that the deferred action has been completed but only that the request to eventually perform the deferred action has been accepted.

The `resolve` function that is passed to an `executor` function accepts a single argument. The `executor` code may eventually call the `resolve` function to indicate that it wishes to resolve the associated Promise. The argument passed to the `resolve` function represents the eventual value of the deferred action and can be either the actual fulfillment value or another promise which will provide the value if it is fulfilled.

The `reject` function that is passed to an `executor` function accepts a single argument. The `executor` code may eventually call the `reject` function to indicate that the associated Promise is rejected and will never be fulfilled. The argument passed to the `reject` function is used as the rejection value of the promise. Typically it will be an Error object.

The resolve and reject functions passed to an `executor` function by the Promise [constructor](ecmascript-data-types-and-values.html#constructor) have the capability to actually resolve and reject the associated promise. Subclasses may have different [constructor](ecmascript-data-types-and-values.html#constructor) behaviour that passes in customized values for resolve and reject.

### 27.2.4 Properties of the Promise Constructor

The Promise [constructor](ecmascript-data-types-and-values.html#constructor):

-   has a `\[\[Prototype\]\]` internal slot whose value is [%Function.prototype%](fundamental-objects.html#sec-properties-of-the-function-prototype-object).
-   has the following properties:

### 27.2.4.1 Promise.all ( `iterable` )

This function returns a new promise which is fulfilled with an array of fulfillment values for the passed promises, or rejects with the reason of the first passed promise that rejects. It resolves all elements of the passed [iterable](control-abstraction-objects.html#sec-iterable-interface) to promises as it runs this algorithm.

1.  Let `C` be the `this` value.
2.  Let `promiseCapability` be ? [NewPromiseCapability](control-abstraction-objects.html#sec-newpromisecapability)(`C`).
3.  Let `promiseResolve` be [Completion](notational-conventions.html#sec-completion-ao)([GetPromiseResolve](control-abstraction-objects.html#sec-getpromiseresolve)(`C`)).
4.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`promiseResolve`, `promiseCapability`).
5.  Let `iteratorRecord` be [Completion](notational-conventions.html#sec-completion-ao)([GetIterator](abstract-operations.html#sec-getiterator)(`iterable`, `sync`)).
6.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`iteratorRecord`, `promiseCapability`).
7.  Let `result` be [Completion](notational-conventions.html#sec-completion-ao)([PerformPromiseAll](control-abstraction-objects.html#sec-performpromiseall)(`iteratorRecord`, `C`, `promiseCapability`, `promiseResolve`)).
8.  If `result` is an [abrupt completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
	1.  If `iteratorRecord`.`\[\[Done\]\]` is `false`, set `result` to [Completion](notational-conventions.html#sec-completion-ao)([IteratorClose](abstract-operations.html#sec-iteratorclose)(`iteratorRecord`, `result`)).
	2.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`result`, `promiseCapability`).
9.  Return ! `result`.

Note

This function requires its `this` value to be a [constructor](ecmascript-data-types-and-values.html#constructor) function that supports the parameter conventions of the Promise [constructor](ecmascript-data-types-and-values.html#constructor).

### 27.2.4.1.1 GetPromiseResolve ( `promiseConstructor` )

The abstract operation GetPromiseResolve takes argument `promiseConstructor` (a [constructor](ecmascript-data-types-and-values.html#constructor)) and returns either a [normal completion containing](ecmascript-data-types-and-values.html#sec-completion-record-specification-type) a [function object](ecmascript-data-types-and-values.html#function-object) or a [throw completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type). It performs the following steps when called:

1.  Let `promiseResolve` be ? [Get](abstract-operations.html#sec-get-o-p)(`promiseConstructor`, `"resolve"`).
2.  If [IsCallable](abstract-operations.html#sec-iscallable)(`promiseResolve`) is `false`, throw a `TypeError` exception.
3.  Return `promiseResolve`.

### 27.2.4.1.2 PerformPromiseAll ( `iteratorRecord`, `constructor`, `resultCapability`, `promiseResolve` )

The abstract operation PerformPromiseAll takes arguments `iteratorRecord` (an [Iterator Record](abstract-operations.html#sec-iterator-records)), `constructor` (a [constructor](ecmascript-data-types-and-values.html#constructor)), `resultCapability` (a [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records)), and `promiseResolve` (a [function object](ecmascript-data-types-and-values.html#function-object)) and returns either a [normal completion containing](ecmascript-data-types-and-values.html#sec-completion-record-specification-type) an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types) or a [throw completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type). It performs the following steps when called:

1.  Let `values` be a new empty [List](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type).
2.  NOTE: `remainingElementsCount` starts at 1 instead of 0 to ensure `resultCapability`.`\[\[Resolve\]\]` is only called once, even in the presence of a misbehaving `"then"` which calls the passed callback before the input [iterator](control-abstraction-objects.html#sec-iterator-interface) is exhausted.
3.  Let `remainingElementsCount` be the [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) { `\[\[Value\]\]`: 1 }.
4.  Let `index` be 0.
5.  Repeat,
	1.  Let `next` be ? [IteratorStepValue](abstract-operations.html#sec-iteratorstepvalue)(`iteratorRecord`).
	2.  If `next` is `done`, then
		1.  Set `remainingElementsCount`.`\[\[Value\]\]` to `remainingElementsCount`.`\[\[Value\]\]` - 1.
		2.  If `remainingElementsCount`.`\[\[Value\]\]` = 0, then
			1.  Let `valuesArray` be [CreateArrayFromList](abstract-operations.html#sec-createarrayfromlist)(`values`).
			2.  Perform ? [Call](abstract-operations.html#sec-call)(`resultCapability`.`\[\[Resolve\]\]`, `undefined`, « `valuesArray` »).
		3.  Return `resultCapability`.`\[\[Promise\]\]`.
	3.  Append `undefined` to `values`.
	4.  Let `nextPromise` be ? [Call](abstract-operations.html#sec-call)(`promiseResolve`, `constructor`, « `next` »).
	5.  Let `fulfilledSteps` be a new [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with parameters (`value`) that captures `values`, `resultCapability`, and `remainingElementsCount` and performs the following steps when called:
		1.  Let `F` be the [active function object](executable-code-and-execution-contexts.html#active-function-object).
		2.  If `F`.`\[\[AlreadyCalled\]\]` is `true`, return `undefined`.
		3.  Set `F`.`\[\[AlreadyCalled\]\]` to `true`.
		4.  Let `thisIndex` be `F`.`\[\[Index\]\]`.
		5.  Set `values`\[`thisIndex`\] to `value`.
		6.  Set `remainingElementsCount`.`\[\[Value\]\]` to `remainingElementsCount`.`\[\[Value\]\]` - 1.
		7.  If `remainingElementsCount`.`\[\[Value\]\]` = 0, then
			1.  Let `valuesArray` be [CreateArrayFromList](abstract-operations.html#sec-createarrayfromlist)(`values`).
			2.  Return ? [Call](abstract-operations.html#sec-call)(`resultCapability`.`\[\[Resolve\]\]`, `undefined`, « `valuesArray` »).
		8.  Return `undefined`.
	6.  Let `onFulfilled` be [CreateBuiltinFunction](ordinary-and-exotic-objects-behaviours.html#sec-createbuiltinfunction)(`fulfilledSteps`, 1, `""`, « `\[\[AlreadyCalled\]\]`, `\[\[Index\]\]` »).
	7.  Set `onFulfilled`.`\[\[AlreadyCalled\]\]` to `false`.
	8.  Set `onFulfilled`.`\[\[Index\]\]` to `index`.
	9.  Set `index` to `index` + 1.
	10.  Set `remainingElementsCount`.`\[\[Value\]\]` to `remainingElementsCount`.`\[\[Value\]\]` + 1.
	11.  Perform ? [Invoke](abstract-operations.html#sec-invoke)(`nextPromise`, `"then"`, « `onFulfilled`, `resultCapability`.`\[\[Reject\]\]` »).

### 27.2.4.2 Promise.allSettled ( `iterable` )

This function returns a promise that is fulfilled with an array of promise state snapshots, but only after all the original promises have settled, i.e. become either fulfilled or rejected. It resolves all elements of the passed [iterable](control-abstraction-objects.html#sec-iterable-interface) to promises as it runs this algorithm.

1.  Let `C` be the `this` value.
2.  Let `promiseCapability` be ? [NewPromiseCapability](control-abstraction-objects.html#sec-newpromisecapability)(`C`).
3.  Let `promiseResolve` be [Completion](notational-conventions.html#sec-completion-ao)([GetPromiseResolve](control-abstraction-objects.html#sec-getpromiseresolve)(`C`)).
4.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`promiseResolve`, `promiseCapability`).
5.  Let `iteratorRecord` be [Completion](notational-conventions.html#sec-completion-ao)([GetIterator](abstract-operations.html#sec-getiterator)(`iterable`, `sync`)).
6.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`iteratorRecord`, `promiseCapability`).
7.  Let `result` be [Completion](notational-conventions.html#sec-completion-ao)([PerformPromiseAllSettled](control-abstraction-objects.html#sec-performpromiseallsettled)(`iteratorRecord`, `C`, `promiseCapability`, `promiseResolve`)).
8.  If `result` is an [abrupt completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
	1.  If `iteratorRecord`.`\[\[Done\]\]` is `false`, set `result` to [Completion](notational-conventions.html#sec-completion-ao)([IteratorClose](abstract-operations.html#sec-iteratorclose)(`iteratorRecord`, `result`)).
	2.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`result`, `promiseCapability`).
9.  Return ! `result`.

Note

This function requires its `this` value to be a [constructor](ecmascript-data-types-and-values.html#constructor) function that supports the parameter conventions of the Promise [constructor](ecmascript-data-types-and-values.html#constructor).

### 27.2.4.2.1 PerformPromiseAllSettled ( `iteratorRecord`, `constructor`, `resultCapability`, `promiseResolve` )

The abstract operation PerformPromiseAllSettled takes arguments `iteratorRecord` (an [Iterator Record](abstract-operations.html#sec-iterator-records)), `constructor` (a [constructor](ecmascript-data-types-and-values.html#constructor)), `resultCapability` (a [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records)), and `promiseResolve` (a [function object](ecmascript-data-types-and-values.html#function-object)) and returns either a [normal completion containing](ecmascript-data-types-and-values.html#sec-completion-record-specification-type) an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types) or a [throw completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type). It performs the following steps when called:

1.  Let `values` be a new empty [List](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type).
2.  NOTE: `remainingElementsCount` starts at 1 instead of 0 to ensure `resultCapability`.`\[\[Resolve\]\]` is only called once, even in the presence of a misbehaving `"then"` which calls one of the passed callbacks before the input [iterator](control-abstraction-objects.html#sec-iterator-interface) is exhausted.
3.  Let `remainingElementsCount` be the [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) { `\[\[Value\]\]`: 1 }.
4.  Let `index` be 0.
5.  Repeat,
	1.  Let `next` be ? [IteratorStepValue](abstract-operations.html#sec-iteratorstepvalue)(`iteratorRecord`).
	2.  If `next` is `done`, then
		1.  Set `remainingElementsCount`.`\[\[Value\]\]` to `remainingElementsCount`.`\[\[Value\]\]` - 1.
		2.  If `remainingElementsCount`.`\[\[Value\]\]` = 0, then
			1.  Let `valuesArray` be [CreateArrayFromList](abstract-operations.html#sec-createarrayfromlist)(`values`).
			2.  Perform ? [Call](abstract-operations.html#sec-call)(`resultCapability`.`\[\[Resolve\]\]`, `undefined`, « `valuesArray` »).
		3.  Return `resultCapability`.`\[\[Promise\]\]`.
	3.  Append `undefined` to `values`.
	4.  Let `nextPromise` be ? [Call](abstract-operations.html#sec-call)(`promiseResolve`, `constructor`, « `next` »).
	5.  Let `alreadyCalled` be the [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) { `\[\[Value\]\]`: `false` }.
	6.  Let `fulfilledSteps` be a new [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with parameters (`value`) that captures `values`, `resultCapability`, and `remainingElementsCount` and performs the following steps when called:
		1.  Let `F` be the [active function object](executable-code-and-execution-contexts.html#active-function-object).
		2.  If `F`.`\[\[AlreadyCalled\]\]`.`\[\[Value\]\]` is `true`, return `undefined`.
		3.  Set `F`.`\[\[AlreadyCalled\]\]`.`\[\[Value\]\]` to `true`.
		4.  Let `obj` be [OrdinaryObjectCreate](ordinary-and-exotic-objects-behaviours.html#sec-ordinaryobjectcreate)([%Object.prototype%](fundamental-objects.html#sec-properties-of-the-object-prototype-object)).
		5.  Perform ! [CreateDataPropertyOrThrow](abstract-operations.html#sec-createdatapropertyorthrow)(`obj`, `"status"`, `"fulfilled"`).
		6.  Perform ! [CreateDataPropertyOrThrow](abstract-operations.html#sec-createdatapropertyorthrow)(`obj`, `"value"`, `value`).
		7.  Let `thisIndex` be `F`.`\[\[Index\]\]`.
		8.  Set `values`\[`thisIndex`\] to `obj`.
		9.  Set `remainingElementsCount`.`\[\[Value\]\]` to `remainingElementsCount`.`\[\[Value\]\]` - 1.
		10.  If `remainingElementsCount`.`\[\[Value\]\]` = 0, then
			 1.  Let `valuesArray` be [CreateArrayFromList](abstract-operations.html#sec-createarrayfromlist)(`values`).
			 2.  Return ? [Call](abstract-operations.html#sec-call)(`resultCapability`.`\[\[Resolve\]\]`, `undefined`, « `valuesArray` »).
		11.  Return `undefined`.
	7.  Let `onFulfilled` be [CreateBuiltinFunction](ordinary-and-exotic-objects-behaviours.html#sec-createbuiltinfunction)(`fulfilledSteps`, 1, `""`, « `\[\[AlreadyCalled\]\]`, `\[\[Index\]\]` »).
	8.  Set `onFulfilled`.`\[\[AlreadyCalled\]\]` to `alreadyCalled`.
	9.  Set `onFulfilled`.`\[\[Index\]\]` to `index`.
	10.  Let `rejectedSteps` be a new [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with parameters (`error`) that captures `values`, `resultCapability`, and `remainingElementsCount` and performs the following steps when called:
		 1.  Let `F` be the [active function object](executable-code-and-execution-contexts.html#active-function-object).
		 2.  If `F`.`\[\[AlreadyCalled\]\]`.`\[\[Value\]\]` is `true`, return `undefined`.
		 3.  Set `F`.`\[\[AlreadyCalled\]\]`.`\[\[Value\]\]` to `true`.
		 4.  Let `obj` be [OrdinaryObjectCreate](ordinary-and-exotic-objects-behaviours.html#sec-ordinaryobjectcreate)([%Object.prototype%](fundamental-objects.html#sec-properties-of-the-object-prototype-object)).
		 5.  Perform ! [CreateDataPropertyOrThrow](abstract-operations.html#sec-createdatapropertyorthrow)(`obj`, `"status"`, `"rejected"`).
		 6.  Perform ! [CreateDataPropertyOrThrow](abstract-operations.html#sec-createdatapropertyorthrow)(`obj`, `"reason"`, `error`).
		 7.  Let `thisIndex` be `F`.`\[\[Index\]\]`.
		 8.  Set `values`\[`thisIndex`\] to `obj`.
		 9.  Set `remainingElementsCount`.`\[\[Value\]\]` to `remainingElementsCount`.`\[\[Value\]\]` - 1.
		 10.  If `remainingElementsCount`.`\[\[Value\]\]` = 0, then
			  1.  Let `valuesArray` be [CreateArrayFromList](abstract-operations.html#sec-createarrayfromlist)(`values`).
			  2.  Return ? [Call](abstract-operations.html#sec-call)(`resultCapability`.`\[\[Resolve\]\]`, `undefined`, « `valuesArray` »).
		 11.  Return `undefined`.
	11.  Let `onRejected` be [CreateBuiltinFunction](ordinary-and-exotic-objects-behaviours.html#sec-createbuiltinfunction)(`rejectedSteps`, 1, `""`, « `\[\[AlreadyCalled\]\]`, `\[\[Index\]\]` »).
	12.  Set `onRejected`.`\[\[AlreadyCalled\]\]` to `alreadyCalled`.
	13.  Set `onRejected`.`\[\[Index\]\]` to `index`.
	14.  Set `index` to `index` + 1.
	15.  Set `remainingElementsCount`.`\[\[Value\]\]` to `remainingElementsCount`.`\[\[Value\]\]` + 1.
	16.  Perform ? [Invoke](abstract-operations.html#sec-invoke)(`nextPromise`, `"then"`, « `onFulfilled`, `onRejected` »).

### 27.2.4.3 Promise.any ( `iterable` )

This function returns a promise that is fulfilled by the first given promise to be fulfilled, or rejected with an `AggregateError` holding the rejection reasons if all of the given promises are rejected. It resolves all elements of the passed [iterable](control-abstraction-objects.html#sec-iterable-interface) to promises as it runs this algorithm.

1.  Let `C` be the `this` value.
2.  Let `promiseCapability` be ? [NewPromiseCapability](control-abstraction-objects.html#sec-newpromisecapability)(`C`).
3.  Let `promiseResolve` be [Completion](notational-conventions.html#sec-completion-ao)([GetPromiseResolve](control-abstraction-objects.html#sec-getpromiseresolve)(`C`)).
4.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`promiseResolve`, `promiseCapability`).
5.  Let `iteratorRecord` be [Completion](notational-conventions.html#sec-completion-ao)([GetIterator](abstract-operations.html#sec-getiterator)(`iterable`, `sync`)).
6.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`iteratorRecord`, `promiseCapability`).
7.  Let `result` be [Completion](notational-conventions.html#sec-completion-ao)([PerformPromiseAny](control-abstraction-objects.html#sec-performpromiseany)(`iteratorRecord`, `C`, `promiseCapability`, `promiseResolve`)).
8.  If `result` is an [abrupt completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
	1.  If `iteratorRecord`.`\[\[Done\]\]` is `false`, set `result` to [Completion](notational-conventions.html#sec-completion-ao)([IteratorClose](abstract-operations.html#sec-iteratorclose)(`iteratorRecord`, `result`)).
	2.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`result`, `promiseCapability`).
9.  Return ! `result`.

Note

This function requires its `this` value to be a [constructor](ecmascript-data-types-and-values.html#constructor) function that supports the parameter conventions of the `Promise` [constructor](ecmascript-data-types-and-values.html#constructor).

### 27.2.4.3.1 PerformPromiseAny ( `iteratorRecord`, `constructor`, `resultCapability`, `promiseResolve` )

The abstract operation PerformPromiseAny takes arguments `iteratorRecord` (an [Iterator Record](abstract-operations.html#sec-iterator-records)), `constructor` (a [constructor](ecmascript-data-types-and-values.html#constructor)), `resultCapability` (a [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records)), and `promiseResolve` (a [function object](ecmascript-data-types-and-values.html#function-object)) and returns either a [normal completion containing](ecmascript-data-types-and-values.html#sec-completion-record-specification-type) an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types) or a [throw completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type). It performs the following steps when called:

1.  Let `errors` be a new empty [List](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type).
2.  NOTE: `remainingElementsCount` starts at 1 instead of 0 to ensure `resultCapability`.`\[\[Reject\]\]` is only called once, even in the presence of a misbehaving `"then"` which calls the passed callback before the input [iterator](control-abstraction-objects.html#sec-iterator-interface) is exhausted.
3.  Let `remainingElementsCount` be the [Record](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) { `\[\[Value\]\]`: 1 }.
4.  Let `index` be 0.
5.  Repeat,
	1.  Let `next` be ? [IteratorStepValue](abstract-operations.html#sec-iteratorstepvalue)(`iteratorRecord`).
	2.  If `next` is `done`, then
		1.  Set `remainingElementsCount`.`\[\[Value\]\]` to `remainingElementsCount`.`\[\[Value\]\]` - 1.
		2.  If `remainingElementsCount`.`\[\[Value\]\]` = 0, then
			1.  Let `aggregateError` be a newly created `AggregateError` object.
			2.  Perform ! [DefinePropertyOrThrow](abstract-operations.html#sec-definepropertyorthrow)(`aggregateError`, `"errors"`, PropertyDescriptor { `\[\[Configurable\]\]`: `true`, `\[\[Enumerable\]\]`: `false`, `\[\[Writable\]\]`: `true`, `\[\[Value\]\]`: [CreateArrayFromList](abstract-operations.html#sec-createarrayfromlist)(`errors`) }).
			3.  Perform ? [Call](abstract-operations.html#sec-call)(`resultCapability`.`\[\[Reject\]\]`, `undefined`, « `aggregateError` »).
		3.  Return `resultCapability`.`\[\[Promise\]\]`.
	3.  Append `undefined` to `errors`.
	4.  Let `nextPromise` be ? [Call](abstract-operations.html#sec-call)(`promiseResolve`, `constructor`, « `next` »).
	5.  Let `rejectedSteps` be a new [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with parameters (`error`) that captures `errors`, `resultCapability`, and `remainingElementsCount` and performs the following steps when called:
		1.  Let `F` be the [active function object](executable-code-and-execution-contexts.html#active-function-object).
		2.  If `F`.`\[\[AlreadyCalled\]\]` is `true`, return `undefined`.
		3.  Set `F`.`\[\[AlreadyCalled\]\]` to `true`.
		4.  Let `thisIndex` be `F`.`\[\[Index\]\]`.
		5.  Set `errors`\[`thisIndex`\] to `error`.
		6.  Set `remainingElementsCount`.`\[\[Value\]\]` to `remainingElementsCount`.`\[\[Value\]\]` - 1.
		7.  If `remainingElementsCount`.`\[\[Value\]\]` = 0, then
			1.  Let `aggregateError` be a newly created `AggregateError` object.
			2.  Perform ! [DefinePropertyOrThrow](abstract-operations.html#sec-definepropertyorthrow)(`aggregateError`, `"errors"`, PropertyDescriptor { `\[\[Configurable\]\]`: `true`, `\[\[Enumerable\]\]`: `false`, `\[\[Writable\]\]`: `true`, `\[\[Value\]\]`: [CreateArrayFromList](abstract-operations.html#sec-createarrayfromlist)(`errors`) }).
			3.  Return ? [Call](abstract-operations.html#sec-call)(`resultCapability`.`\[\[Reject\]\]`, `undefined`, « `aggregateError` »).
		8.  Return `undefined`.
	6.  Let `onRejected` be [CreateBuiltinFunction](ordinary-and-exotic-objects-behaviours.html#sec-createbuiltinfunction)(`rejectedSteps`, 1, `""`, « `\[\[AlreadyCalled\]\]`, `\[\[Index\]\]` »).
	7.  Set `onRejected`.`\[\[AlreadyCalled\]\]` to `false`.
	8.  Set `onRejected`.`\[\[Index\]\]` to `index`.
	9.  Set `index` to `index` + 1.
	10.  Set `remainingElementsCount`.`\[\[Value\]\]` to `remainingElementsCount`.`\[\[Value\]\]` + 1.
	11.  Perform ? [Invoke](abstract-operations.html#sec-invoke)(`nextPromise`, `"then"`, « `resultCapability`.`\[\[Resolve\]\]`, `onRejected` »).

### 27.2.4.4 Promise.prototype

The initial value of `Promise.prototype` is the [Promise prototype object](control-abstraction-objects.html#sec-properties-of-the-promise-prototype-object).

This property has the attributes { `\[\[Writable\]\]`: `false`, `\[\[Enumerable\]\]`: `false`, `\[\[Configurable\]\]`: `false` }.


### 27.2.4.5 Promise.race ( `iterable` )

This function returns a new promise which is settled in the same way as the first passed promise to settle. It resolves all elements of the passed `iterable` to promises as it runs this algorithm.

1.  Let `C` be the `this` value.
2.  Let `promiseCapability` be ? [NewPromiseCapability](control-abstraction-objects.html#sec-newpromisecapability)(`C`).
3.  Let `promiseResolve` be [Completion](notational-conventions.html#sec-completion-ao)([GetPromiseResolve](control-abstraction-objects.html#sec-getpromiseresolve)(`C`)).
4.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`promiseResolve`, `promiseCapability`).
5.  Let `iteratorRecord` be [Completion](notational-conventions.html#sec-completion-ao)([GetIterator](abstract-operations.html#sec-getiterator)(`iterable`, `sync`)).
6.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`iteratorRecord`, `promiseCapability`).
7.  Let `result` be [Completion](notational-conventions.html#sec-completion-ao)([PerformPromiseRace](control-abstraction-objects.html#sec-performpromiserace)(`iteratorRecord`, `C`, `promiseCapability`, `promiseResolve`)).
8.  If `result` is an [abrupt completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
	1.  If `iteratorRecord`.`\[\[Done\]\]` is `false`, set `result` to [Completion](notational-conventions.html#sec-completion-ao)([IteratorClose](abstract-operations.html#sec-iteratorclose)(`iteratorRecord`, `result`)).
	2.  [IfAbruptRejectPromise](control-abstraction-objects.html#sec-ifabruptrejectpromise)(`result`, `promiseCapability`).
9.  Return ! `result`.

Note 1

If the `iterable` argument yields no values or if none of the promises yielded by `iterable` ever settle, then the pending promise returned by this method will never be settled.

Note 2

This function expects its `this` value to be a [constructor](ecmascript-data-types-and-values.html#constructor) function that supports the parameter conventions of the Promise [constructor](ecmascript-data-types-and-values.html#constructor). It also expects that its `this` value provides a `resolve` method.

### 27.2.4.5.1 PerformPromiseRace ( `iteratorRecord`, `constructor`, `resultCapability`, `promiseResolve` )

The abstract operation PerformPromiseRace takes arguments `iteratorRecord` (an [Iterator Record](abstract-operations.html#sec-iterator-records)), `constructor` (a [constructor](ecmascript-data-types-and-values.html#constructor)), `resultCapability` (a [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records)), and `promiseResolve` (a [function object](ecmascript-data-types-and-values.html#function-object)) and returns either a [normal completion containing](ecmascript-data-types-and-values.html#sec-completion-record-specification-type) an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types) or a [throw completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type). It performs the following steps when called:

1.  Repeat,
	1.  Let `next` be ? [IteratorStepValue](abstract-operations.html#sec-iteratorstepvalue)(`iteratorRecord`).
	2.  If `next` is `done`, then
		1.  Return `resultCapability`.`\[\[Promise\]\]`.
	3.  Let `nextPromise` be ? [Call](abstract-operations.html#sec-call)(`promiseResolve`, `constructor`, « `next` »).
	4.  Perform ? [Invoke](abstract-operations.html#sec-invoke)(`nextPromise`, `"then"`, « `resultCapability`.`\[\[Resolve\]\]`, `resultCapability`.`\[\[Reject\]\]` »).

### 27.2.4.6 Promise.reject ( `r` )

This function returns a new promise rejected with the passed argument.

1.  Let `C` be the `this` value.
2.  Let `promiseCapability` be ? [NewPromiseCapability](control-abstraction-objects.html#sec-newpromisecapability)(`C`).
3.  Perform ? [Call](abstract-operations.html#sec-call)(`promiseCapability`.`\[\[Reject\]\]`, `undefined`, « `r` »).
4.  Return `promiseCapability`.`\[\[Promise\]\]`.

Note

This function expects its `this` value to be a [constructor](ecmascript-data-types-and-values.html#constructor) function that supports the parameter conventions of the Promise [constructor](ecmascript-data-types-and-values.html#constructor).

### 27.2.4.7 Promise.resolve ( `x` )

This function returns either a new promise resolved with the passed argument, or the argument itself if the argument is a promise produced by this [constructor](ecmascript-data-types-and-values.html#constructor).

1.  Let `C` be the `this` value.
2.  If `C` [is not an Object](ecmascript-data-types-and-values.html#sec-object-type), throw a `TypeError` exception.
3.  Return ? [PromiseResolve](control-abstraction-objects.html#sec-promise-resolve)(`C`, `x`).

Note

This function expects its `this` value to be a [constructor](ecmascript-data-types-and-values.html#constructor) function that supports the parameter conventions of the Promise [constructor](ecmascript-data-types-and-values.html#constructor).

### 27.2.4.7.1 PromiseResolve ( `C`, `x` )

The abstract operation PromiseResolve takes arguments `C` (an Object) and `x` (an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types)) and returns either a [normal completion containing](ecmascript-data-types-and-values.html#sec-completion-record-specification-type) an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types) or a [throw completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type). It returns a new promise resolved with `x`. It performs the following steps when called:

1.  If [IsPromise](control-abstraction-objects.html#sec-ispromise)(`x`) is `true`, then
	1.  Let `xConstructor` be ? [Get](abstract-operations.html#sec-get-o-p)(`x`, `"constructor"`).
	2.  If [SameValue](abstract-operations.html#sec-samevalue)(`xConstructor`, `C`) is `true`, return `x`.
2.  Let `promiseCapability` be ? [NewPromiseCapability](control-abstraction-objects.html#sec-newpromisecapability)(`C`).
3.  Perform ? [Call](abstract-operations.html#sec-call)(`promiseCapability`.`\[\[Resolve\]\]`, `undefined`, « `x` »).
4.  Return `promiseCapability`.`\[\[Promise\]\]`.

### 27.2.4.8 Promise.try ( `callback`, ...`args` )

This function performs the following steps when called:

1.  Let `C` be the `this` value.
2.  If `C` [is not an Object](ecmascript-data-types-and-values.html#sec-object-type), throw a `TypeError` exception.
3.  Let `promiseCapability` be ? [NewPromiseCapability](control-abstraction-objects.html#sec-newpromisecapability)(`C`).
4.  Let `status` be [Completion](notational-conventions.html#sec-completion-ao)([Call](abstract-operations.html#sec-call)(`callback`, `undefined`, `args`)).
5.  If `status` is an [abrupt completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
	1.  Perform ? [Call](abstract-operations.html#sec-call)(`promiseCapability`.`\[\[Reject\]\]`, `undefined`, « `status`.`\[\[Value\]\]` »).
6.  Else,
	1.  Perform ? [Call](abstract-operations.html#sec-call)(`promiseCapability`.`\[\[Resolve\]\]`, `undefined`, « `status`.`\[\[Value\]\]` »).
7.  Return `promiseCapability`.`\[\[Promise\]\]`.

Note

This function expects its `this` value to be a [constructor](ecmascript-data-types-and-values.html#constructor) function that supports the parameter conventions of the Promise [constructor](ecmascript-data-types-and-values.html#constructor).

### 27.2.4.9 Promise.withResolvers ( )

This function returns an object with three properties: a new promise together with the `resolve` and `reject` functions associated with it.

1.  Let `C` be the `this` value.
2.  Let `promiseCapability` be ? [NewPromiseCapability](control-abstraction-objects.html#sec-newpromisecapability)(`C`).
3.  Let `obj` be [OrdinaryObjectCreate](ordinary-and-exotic-objects-behaviours.html#sec-ordinaryobjectcreate)([%Object.prototype%](fundamental-objects.html#sec-properties-of-the-object-prototype-object)).
4.  Perform ! [CreateDataPropertyOrThrow](abstract-operations.html#sec-createdatapropertyorthrow)(`obj`, `"promise"`, `promiseCapability`.`\[\[Promise\]\]`).
5.  Perform ! [CreateDataPropertyOrThrow](abstract-operations.html#sec-createdatapropertyorthrow)(`obj`, `"resolve"`, `promiseCapability`.`\[\[Resolve\]\]`).
6.  Perform ! [CreateDataPropertyOrThrow](abstract-operations.html#sec-createdatapropertyorthrow)(`obj`, `"reject"`, `promiseCapability`.`\[\[Reject\]\]`).
7.  Return `obj`.

### 27.2.4.10 get Promise \[ %Symbol.species% \]

`Promise[%Symbol.species%]` is an [accessor property](ecmascript-data-types-and-values.html#sec-object-type) whose set accessor function is `undefined`. Its get accessor function performs the following steps when called:

1.  Return the `this` value.

The value of the `"name"` property of this function is `"get \[Symbol.species\]"`.

Note

Promise prototype methods normally use their `this` value's [constructor](ecmascript-data-types-and-values.html#constructor) to create a derived object. However, a subclass [constructor](ecmascript-data-types-and-values.html#constructor) may over-ride that default behaviour by redefining its [%Symbol.species%](ecmascript-data-types-and-values.html#sec-well-known-symbols) property.

### 27.2.5 Properties of the Promise Prototype Object

The Promise prototype object:

-   is %Promise.prototype%.
-   has a `\[\[Prototype\]\]` internal slot whose value is [%Object.prototype%](fundamental-objects.html#sec-properties-of-the-object-prototype-object).
-   is an [ordinary object](ecmascript-data-types-and-values.html#ordinary-object).
-   does not have a `\[\[PromiseState\]\]` internal slot or any of the other internal slots of Promise instances.

### 27.2.5.1 Promise.prototype.catch ( `onRejected` )

This method performs the following steps when called:

1.  Let `promise` be the `this` value.
2.  Return ? [Invoke](abstract-operations.html#sec-invoke)(`promise`, `"then"`, « `undefined`, `onRejected` »).

### 27.2.5.2 Promise.prototype.constructor

The initial value of `Promise.prototype.constructor` is [%Promise%](control-abstraction-objects.html#sec-promise-constructor).

### 27.2.5.3 Promise.prototype.finally ( `onFinally` )

This method performs the following steps when called:

1.  Let `promise` be the `this` value.
2.  If `promise` [is not an Object](ecmascript-data-types-and-values.html#sec-object-type), throw a `TypeError` exception.
3.  Let `C` be ? [SpeciesConstructor](abstract-operations.html#sec-speciesconstructor)(`promise`, [%Promise%](control-abstraction-objects.html#sec-promise-constructor)).
4.  [Assert](notational-conventions.html#assert): [IsConstructor](abstract-operations.html#sec-isconstructor)(`C`) is `true`.
5.  If [IsCallable](abstract-operations.html#sec-iscallable)(`onFinally`) is `false`, then
	1.  Let `thenFinally` be `onFinally`.
	2.  Let `catchFinally` be `onFinally`.
6.  Else,
	1.  Let `thenFinallyClosure` be a new [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with parameters (`value`) that captures `onFinally` and `C` and performs the following steps when called:
		1.  Let `result` be ? [Call](abstract-operations.html#sec-call)(`onFinally`, `undefined`).
		2.  Let `p` be ? [PromiseResolve](control-abstraction-objects.html#sec-promise-resolve)(`C`, `result`).
		3.  Let `returnValue` be a new [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with no parameters that captures `value` and performs the following steps when called:
			1.  Return [NormalCompletion](ecmascript-data-types-and-values.html#sec-normalcompletion)(`value`).
		4.  Let `valueThunk` be [CreateBuiltinFunction](ordinary-and-exotic-objects-behaviours.html#sec-createbuiltinfunction)(`returnValue`, 0, `""`, « »).
		5.  Return ? [Invoke](abstract-operations.html#sec-invoke)(`p`, `"then"`, « `valueThunk` »).
	2.  Let `thenFinally` be [CreateBuiltinFunction](ordinary-and-exotic-objects-behaviours.html#sec-createbuiltinfunction)(`thenFinallyClosure`, 1, `""`, « »).
	3.  Let `catchFinallyClosure` be a new [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with parameters (`reason`) that captures `onFinally` and `C` and performs the following steps when called:
		1.  Let `result` be ? [Call](abstract-operations.html#sec-call)(`onFinally`, `undefined`).
		2.  Let `p` be ? [PromiseResolve](control-abstraction-objects.html#sec-promise-resolve)(`C`, `result`).
		3.  Let `throwReason` be a new [Abstract Closure](ecmascript-data-types-and-values.html#sec-abstract-closure) with no parameters that captures `reason` and performs the following steps when called:
			1.  Return [ThrowCompletion](ecmascript-data-types-and-values.html#sec-throwcompletion)(`reason`).
		4.  Let `thrower` be [CreateBuiltinFunction](ordinary-and-exotic-objects-behaviours.html#sec-createbuiltinfunction)(`throwReason`, 0, `""`, « »).
		5.  Return ? [Invoke](abstract-operations.html#sec-invoke)(`p`, `"then"`, « `thrower` »).
	4.  Let `catchFinally` be [CreateBuiltinFunction](ordinary-and-exotic-objects-behaviours.html#sec-createbuiltinfunction)(`catchFinallyClosure`, 1, `""`, « »).
7.  Return ? [Invoke](abstract-operations.html#sec-invoke)(`promise`, `"then"`, « `thenFinally`, `catchFinally` »).

### 27.2.5.4 Promise.prototype.then ( `onFulfilled`, `onRejected` )

This method performs the following steps when called:

1.  Let `promise` be the `this` value.
2.  If [IsPromise](control-abstraction-objects.html#sec-ispromise)(`promise`) is `false`, throw a `TypeError` exception.
3.  Let `C` be ? [SpeciesConstructor](abstract-operations.html#sec-speciesconstructor)(`promise`, [%Promise%](control-abstraction-objects.html#sec-promise-constructor)).
4.  Let `resultCapability` be ? [NewPromiseCapability](control-abstraction-objects.html#sec-newpromisecapability)(`C`).
5.  Return [PerformPromiseThen](control-abstraction-objects.html#sec-performpromisethen)(`promise`, `onFulfilled`, `onRejected`, `resultCapability`).

### 27.2.5.4.1 PerformPromiseThen ( `promise`, `onFulfilled`, `onRejected` \[ , `resultCapability` \] )

The abstract operation PerformPromiseThen takes arguments `promise` (a Promise), `onFulfilled` (an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types)), and `onRejected` (an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types)) and optional argument `resultCapability` (a [PromiseCapability Record](control-abstraction-objects.html#sec-promisecapability-records)) and returns an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types). It performs the “then” operation on `promise` using `onFulfilled` and `onRejected` as its settlement actions. If `resultCapability` is passed, the result is stored by updating `resultCapability`'s promise. If it is not passed, then PerformPromiseThen is being called by a specification-internal operation where the result does not matter. It performs the following steps when called:

1.  [Assert](notational-conventions.html#assert): [IsPromise](control-abstraction-objects.html#sec-ispromise)(`promise`) is `true`.
2.  If `resultCapability` is not present, then
	1.  Set `resultCapability` to `undefined`.
3.  If [IsCallable](abstract-operations.html#sec-iscallable)(`onFulfilled`) is `false`, then
	1.  Let `onFulfilledJobCallback` be `empty`.
4.  Else,
	1.  Let `onFulfilledJobCallback` be [HostMakeJobCallback](executable-code-and-execution-contexts.html#sec-hostmakejobcallback)(`onFulfilled`).
5.  If [IsCallable](abstract-operations.html#sec-iscallable)(`onRejected`) is `false`, then
	1.  Let `onRejectedJobCallback` be `empty`.
6.  Else,
	1.  Let `onRejectedJobCallback` be [HostMakeJobCallback](executable-code-and-execution-contexts.html#sec-hostmakejobcallback)(`onRejected`).
7.  Let `fulfillReaction` be the [PromiseReaction Record](control-abstraction-objects.html#sec-promisereaction-records) { `\[\[Capability\]\]`: `resultCapability`, `\[\[Type\]\]`: `fulfill`, `\[\[Handler\]\]`: `onFulfilledJobCallback` }.
8.  Let `rejectReaction` be the [PromiseReaction Record](control-abstraction-objects.html#sec-promisereaction-records) { `\[\[Capability\]\]`: `resultCapability`, `\[\[Type\]\]`: `reject`, `\[\[Handler\]\]`: `onRejectedJobCallback` }.
9.  If `promise`.`\[\[PromiseState\]\]` is `pending`, then
	1.  Append `fulfillReaction` to `promise`.`\[\[PromiseFulfillReactions\]\]`.
	2.  Append `rejectReaction` to `promise`.`\[\[PromiseRejectReactions\]\]`.
10.  Else if `promise`.`\[\[PromiseState\]\]` is `fulfilled`, then
	 1.  Let `value` be `promise`.`\[\[PromiseResult\]\]`.
	 2.  Let `fulfillJob` be [NewPromiseReactionJob](control-abstraction-objects.html#sec-newpromisereactionjob)(`fulfillReaction`, `value`).
	 3.  Perform [HostEnqueuePromiseJob](executable-code-and-execution-contexts.html#sec-hostenqueuepromisejob)(`fulfillJob`.`\[\[Job\]\]`, `fulfillJob`.`\[\[Realm\]\]`).
11.  Else,
	 1.  [Assert](notational-conventions.html#assert): `promise`.`\[\[PromiseState\]\]` is `rejected`.
	 2.  Let `reason` be `promise`.`\[\[PromiseResult\]\]`.
	 3.  If `promise`.`\[\[PromiseIsHandled\]\]` is `false`, perform [HostPromiseRejectionTracker](control-abstraction-objects.html#sec-host-promise-rejection-tracker)(`promise`, `"handle"`).
	 4.  Let `rejectJob` be [NewPromiseReactionJob](control-abstraction-objects.html#sec-newpromisereactionjob)(`rejectReaction`, `reason`).
	 5.  Perform [HostEnqueuePromiseJob](executable-code-and-execution-contexts.html#sec-hostenqueuepromisejob)(`rejectJob`.`\[\[Job\]\]`, `rejectJob`.`\[\[Realm\]\]`).
12.  Set `promise`.`\[\[PromiseIsHandled\]\]` to `true`.
13.  If `resultCapability` is `undefined`, then
	 1.  Return `undefined`.
14.  Else,
	 1.  Return `resultCapability`.`\[\[Promise\]\]`.

### 27.2.5.5 Promise.prototype \[ %Symbol.toStringTag% \]

The initial value of the [%Symbol.toStringTag%](ecmascript-data-types-and-values.html#sec-well-known-symbols) property is the String value `"Promise"`.

This property has the attributes { `\[\[Writable\]\]`: `false`, `\[\[Enumerable\]\]`: `false`, `\[\[Configurable\]\]`: `true` }.

### 27.2.6 Properties of Promise Instances

Promise instances are [ordinary objects](ecmascript-data-types-and-values.html#ordinary-object) that inherit properties from the [Promise prototype object](control-abstraction-objects.html#sec-properties-of-the-promise-prototype-object) (the intrinsic, [%Promise.prototype%](control-abstraction-objects.html#sec-properties-of-the-promise-prototype-object)). Promise instances are initially created with the internal slots described in [Table 90](control-abstraction-objects.html#table-internal-slots-of-promise-instances).

Table 90: Internal Slots of Promise Instances

Internal Slot

Type

Description

`\[\[PromiseState\]\]`

`pending`, `fulfilled`, or `rejected`

Governs how a promise will react to incoming calls to its `then` method.

`\[\[PromiseResult\]\]`

an [ECMAScript language value](ecmascript-data-types-and-values.html#sec-ecmascript-language-types) or `empty`

The value with which the promise has been fulfilled or rejected, if any. `empty` if and only if the `\[\[PromiseState\]\]` is `pending`.

`\[\[PromiseFulfillReactions\]\]`

a [List](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) of [PromiseReaction Records](control-abstraction-objects.html#sec-promisereaction-records)

[Records](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) to be processed when/if the promise transitions from the `pending` state to the `fulfilled` state.

`\[\[PromiseRejectReactions\]\]`

a [List](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) of [PromiseReaction Records](control-abstraction-objects.html#sec-promisereaction-records)

[Records](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) to be processed when/if the promise transitions from the `pending` state to the `rejected` state.

`\[\[PromiseIsHandled\]\]`

a Boolean

Indicates whether the promise has ever had a fulfillment or rejection handler; used in unhandled rejection tracking.

<!-- END SPEC EXTRACT -->

</details>

