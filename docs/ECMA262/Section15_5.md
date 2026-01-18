<!-- AUTO-GENERATED: splitEcma262SectionsIntoSubsections.ps1 -->

# Section 15.5: Generator Function Definitions

[Back to Section15](Section15.md) | [Back to Index](Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.5 | Generator Function Definitions | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.5.1 | Static Semantics: Early Errors | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions-static-semantics-early-errors) |
| 15.5.2 | Runtime Semantics: EvaluateGeneratorBody | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluategeneratorbody) |
| 15.5.3 | Runtime Semantics: InstantiateGeneratorFunctionObject | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiategeneratorfunctionobject) |
| 15.5.4 | Runtime Semantics: InstantiateGeneratorFunctionExpression | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiategeneratorfunctionexpression) |
| 15.5.5 | Runtime Semantics: Evaluation | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions-runtime-semantics-evaluation) |

## Appendix: Extracted Spec Text (Converted)

This appendix is generated from a locally extracted tc39.es HTML fragment. It may be overwritten if this file is regenerated.

### 15.5 Generator Function Definitions

#### Syntax

[GeneratorDeclaration](ecmascript-language-functions-and-classes.html#prod-GeneratorDeclaration)\[Yield, Await, Default\] : function \* [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier)\[?Yield, ?Await\] ( [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters)\[+Yield, ~Await\] ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) } \[+Default\] function \* ( [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters)\[+Yield, ~Await\] ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) } [GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression) : function \* [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier)\[+Yield, ~Await\]opt ( [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters)\[+Yield, ~Await\] ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) } [GeneratorMethod](ecmascript-language-functions-and-classes.html#prod-GeneratorMethod)\[Yield, Await\] : \* [ClassElementName](ecmascript-language-functions-and-classes.html#prod-ClassElementName)\[?Yield, ?Await\] ( [UniqueFormalParameters](ecmascript-language-functions-and-classes.html#prod-UniqueFormalParameters)\[+Yield, ~Await\] ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) } [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) : [FunctionBody](ecmascript-language-functions-and-classes.html#prod-FunctionBody)\[+Yield, ~Await\] [YieldExpression](ecmascript-language-functions-and-classes.html#prod-YieldExpression)\[In, Await\] : yield yield \[no [LineTerminator](ecmascript-language-lexical-grammar.html#prod-LineTerminator) here\] [AssignmentExpression](ecmascript-language-expressions.html#prod-AssignmentExpression)\[?In, +Yield, ?Await\] yield \[no [LineTerminator](ecmascript-language-lexical-grammar.html#prod-LineTerminator) here\] \* [AssignmentExpression](ecmascript-language-expressions.html#prod-AssignmentExpression)\[?In, +Yield, ?Await\] Note 1

The syntactic context immediately following `yield` requires use of the [InputElementRegExpOrTemplateTail](ecmascript-language-lexical-grammar.html#prod-InputElementRegExpOrTemplateTail) lexical goal.

Note 2

[YieldExpression](ecmascript-language-functions-and-classes.html#prod-YieldExpression) cannot be used within the [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) of a generator function because any expressions that are part of [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) are evaluated before the resulting Generator is in a resumable state.

Note 3

[Abstract operations](notational-conventions.html#sec-algorithm-conventions-abstract-operations) relating to Generators are defined in [27.5.3](control-abstraction-objects.html#sec-generator-abstract-operations).

### 15.5.1 Static Semantics: Early Errors

[GeneratorMethod](ecmascript-language-functions-and-classes.html#prod-GeneratorMethod) : \* [ClassElementName](ecmascript-language-functions-and-classes.html#prod-ClassElementName) ( [UniqueFormalParameters](ecmascript-language-functions-and-classes.html#prod-UniqueFormalParameters) ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) }

-   It is a Syntax Error if [HasDirectSuper](ecmascript-language-functions-and-classes.html#sec-static-semantics-hasdirectsuper) of [GeneratorMethod](ecmascript-language-functions-and-classes.html#prod-GeneratorMethod) is `true`.
-   It is a Syntax Error if [UniqueFormalParameters](ecmascript-language-functions-and-classes.html#prod-UniqueFormalParameters) [Contains](syntax-directed-operations.html#sec-static-semantics-contains) [YieldExpression](ecmascript-language-functions-and-classes.html#prod-YieldExpression) is `true`.
-   It is a Syntax Error if [FunctionBodyContainsUseStrict](ecmascript-language-functions-and-classes.html#sec-static-semantics-functionbodycontainsusestrict) of [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) is `true` and [IsSimpleParameterList](ecmascript-language-functions-and-classes.html#sec-static-semantics-issimpleparameterlist) of [UniqueFormalParameters](ecmascript-language-functions-and-classes.html#prod-UniqueFormalParameters) is `false`.
-   It is a Syntax Error if any element of the [BoundNames](syntax-directed-operations.html#sec-static-semantics-boundnames) of [UniqueFormalParameters](ecmascript-language-functions-and-classes.html#prod-UniqueFormalParameters) also occurs in the [LexicallyDeclaredNames](syntax-directed-operations.html#sec-static-semantics-lexicallydeclarednames) of [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody).

[GeneratorDeclaration](ecmascript-language-functions-and-classes.html#prod-GeneratorDeclaration) : function \* [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier) ( [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) } function \* ( [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) } [GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression) : function \* [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier)opt ( [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) }

-   If [IsStrict](ecmascript-language-source-code.html#sec-isstrict)([FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters)) is `true`, the Early Error rules for [UniqueFormalParameters](ecmascript-language-functions-and-classes.html#prod-UniqueFormalParameters) : [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) are applied.
-   If [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier) is present and [IsStrict](ecmascript-language-source-code.html#sec-isstrict)([BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier)) is `true`, it is a Syntax Error if the [StringValue](ecmascript-language-expressions.html#sec-static-semantics-stringvalue) of [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier) is either `"eval"` or `"arguments"`.
-   It is a Syntax Error if [FunctionBodyContainsUseStrict](ecmascript-language-functions-and-classes.html#sec-static-semantics-functionbodycontainsusestrict) of [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) is `true` and [IsSimpleParameterList](ecmascript-language-functions-and-classes.html#sec-static-semantics-issimpleparameterlist) of [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) is `false`.
-   It is a Syntax Error if any element of the [BoundNames](syntax-directed-operations.html#sec-static-semantics-boundnames) of [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) also occurs in the [LexicallyDeclaredNames](syntax-directed-operations.html#sec-static-semantics-lexicallydeclarednames) of [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody).
-   It is a Syntax Error if [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) [Contains](syntax-directed-operations.html#sec-static-semantics-contains) [YieldExpression](ecmascript-language-functions-and-classes.html#prod-YieldExpression) is `true`.
-   It is a Syntax Error if [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) [Contains](syntax-directed-operations.html#sec-static-semantics-contains) [SuperProperty](ecmascript-language-expressions.html#prod-SuperProperty) is `true`.
-   It is a Syntax Error if [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) [Contains](syntax-directed-operations.html#sec-static-semantics-contains) [SuperProperty](ecmascript-language-expressions.html#prod-SuperProperty) is `true`.
-   It is a Syntax Error if [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) [Contains](syntax-directed-operations.html#sec-static-semantics-contains) [SuperCall](ecmascript-language-expressions.html#prod-SuperCall) is `true`.
-   It is a Syntax Error if [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) [Contains](syntax-directed-operations.html#sec-static-semantics-contains) [SuperCall](ecmascript-language-expressions.html#prod-SuperCall) is `true`.

### 15.5.2 Runtime Semantics: EvaluateGeneratorBody

The [syntax-directed operation](notational-conventions.html#sec-algorithm-conventions-syntax-directed-operations) EvaluateGeneratorBody takes arguments `functionObject` (an ECMAScript [function object](ecmascript-data-types-and-values.html#function-object)) and `argumentsList` (a [List](ecmascript-data-types-and-values.html#sec-list-and-record-specification-type) of [ECMAScript language values](ecmascript-data-types-and-values.html#sec-ecmascript-language-types)) and returns a [throw completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type) or a [return completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type). It is defined piecewise over the following productions:

[GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) : [FunctionBody](ecmascript-language-functions-and-classes.html#prod-FunctionBody)

1.  Perform ?\u00a0[FunctionDeclarationInstantiation](ordinary-and-exotic-objects-behaviours.html#sec-functiondeclarationinstantiation)(`functionObject`, `argumentsList`).
2.  Let `G` be ?\u00a0[OrdinaryCreateFromConstructor](ordinary-and-exotic-objects-behaviours.html#sec-ordinarycreatefromconstructor)(`functionObject`, `"%GeneratorPrototype%"`, \u00ab `\[\[GeneratorState\]\]`, `\[\[GeneratorContext\]\]`, `\[\[GeneratorBrand\]\]`\u00a0\u00bb).
3.  Set `G`.`\[\[GeneratorBrand\]\]` to `empty`.
4.  Set `G`.`\[\[GeneratorState\]\]` to `suspended-start`.
5.  Perform [GeneratorStart](control-abstraction-objects.html#sec-generatorstart)(`G`, [FunctionBody](ecmascript-language-functions-and-classes.html#prod-FunctionBody)).
6.  Return [ReturnCompletion](ecmascript-data-types-and-values.html#sec-returncompletion)(`G`).

### 15.5.3 Runtime Semantics: InstantiateGeneratorFunctionObject

The [syntax-directed operation](notational-conventions.html#sec-algorithm-conventions-syntax-directed-operations) InstantiateGeneratorFunctionObject takes arguments `env` (an [Environment Record](executable-code-and-execution-contexts.html#sec-environment-records)) and `privateEnv` (a [PrivateEnvironment Record](executable-code-and-execution-contexts.html#privateenvironment-record) or `null`) and returns an ECMAScript [function object](ecmascript-data-types-and-values.html#function-object). It is defined piecewise over the following productions:

[GeneratorDeclaration](ecmascript-language-functions-and-classes.html#prod-GeneratorDeclaration) : function \* [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier) ( [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) }

1.  Let `name` be the [StringValue](ecmascript-language-expressions.html#sec-static-semantics-stringvalue) of [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier).
2.  Let `sourceText` be the [source text matched by](notational-conventions.html#sec-algorithm-conventions-syntax-directed-operations) [GeneratorDeclaration](ecmascript-language-functions-and-classes.html#prod-GeneratorDeclaration).
3.  Let `F` be [OrdinaryFunctionCreate](ordinary-and-exotic-objects-behaviours.html#sec-ordinaryfunctioncreate)([%GeneratorFunction.prototype%](control-abstraction-objects.html#sec-properties-of-the-generatorfunction-prototype-object), `sourceText`, [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters), [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody), `non-lexical-this`, `env`, `privateEnv`).
4.  Perform [SetFunctionName](ordinary-and-exotic-objects-behaviours.html#sec-setfunctionname)(`F`, `name`).
5.  Let `prototype` be [OrdinaryObjectCreate](ordinary-and-exotic-objects-behaviours.html#sec-ordinaryobjectcreate)([%GeneratorPrototype%](control-abstraction-objects.html#sec-properties-of-generator-prototype)).
6.  Perform !\u00a0[DefinePropertyOrThrow](abstract-operations.html#sec-definepropertyorthrow)(`F`, `"prototype"`, PropertyDescriptor { `\[\[Value\]\]`: `prototype`, `\[\[Writable\]\]`: `true`, `\[\[Enumerable\]\]`: `false`, `\[\[Configurable\]\]`: `false`\u00a0}).
7.  Return `F`.

[GeneratorDeclaration](ecmascript-language-functions-and-classes.html#prod-GeneratorDeclaration) : function \* ( [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) }

1.  Let `sourceText` be the [source text matched by](notational-conventions.html#sec-algorithm-conventions-syntax-directed-operations) [GeneratorDeclaration](ecmascript-language-functions-and-classes.html#prod-GeneratorDeclaration).
2.  Let `F` be [OrdinaryFunctionCreate](ordinary-and-exotic-objects-behaviours.html#sec-ordinaryfunctioncreate)([%GeneratorFunction.prototype%](control-abstraction-objects.html#sec-properties-of-the-generatorfunction-prototype-object), `sourceText`, [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters), [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody), `non-lexical-this`, `env`, `privateEnv`).
3.  Perform [SetFunctionName](ordinary-and-exotic-objects-behaviours.html#sec-setfunctionname)(`F`, `"default"`).
4.  Let `prototype` be [OrdinaryObjectCreate](ordinary-and-exotic-objects-behaviours.html#sec-ordinaryobjectcreate)([%GeneratorPrototype%](control-abstraction-objects.html#sec-properties-of-generator-prototype)).
5.  Perform !\u00a0[DefinePropertyOrThrow](abstract-operations.html#sec-definepropertyorthrow)(`F`, `"prototype"`, PropertyDescriptor { `\[\[Value\]\]`: `prototype`, `\[\[Writable\]\]`: `true`, `\[\[Enumerable\]\]`: `false`, `\[\[Configurable\]\]`: `false`\u00a0}).
6.  Return `F`.

Note

An anonymous [GeneratorDeclaration](ecmascript-language-functions-and-classes.html#prod-GeneratorDeclaration) can only occur as part of an `export default` declaration, and its function code is therefore always [strict mode code](ecmascript-language-source-code.html#sec-strict-mode-code).

### 15.5.4 Runtime Semantics: InstantiateGeneratorFunctionExpression

The [syntax-directed operation](notational-conventions.html#sec-algorithm-conventions-syntax-directed-operations) InstantiateGeneratorFunctionExpression takes optional argument `name` (a [property key](ecmascript-data-types-and-values.html#property-key) or a [Private Name](ecmascript-data-types-and-values.html#sec-private-names)) and returns an ECMAScript [function object](ecmascript-data-types-and-values.html#function-object). It is defined piecewise over the following productions:

[GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression) : function \* ( [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) }

1.  If `name` is not present, set `name` to `""`.
2.  Let `env` be the LexicalEnvironment of the [running execution context](executable-code-and-execution-contexts.html#running-execution-context).
3.  Let `privateEnv` be the [running execution context](executable-code-and-execution-contexts.html#running-execution-context)'s PrivateEnvironment.
4.  Let `sourceText` be the [source text matched by](notational-conventions.html#sec-algorithm-conventions-syntax-directed-operations) [GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression).
5.  Let `closure` be [OrdinaryFunctionCreate](ordinary-and-exotic-objects-behaviours.html#sec-ordinaryfunctioncreate)([%GeneratorFunction.prototype%](control-abstraction-objects.html#sec-properties-of-the-generatorfunction-prototype-object), `sourceText`, [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters), [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody), `non-lexical-this`, `env`, `privateEnv`).
6.  Perform [SetFunctionName](ordinary-and-exotic-objects-behaviours.html#sec-setfunctionname)(`closure`, `name`).
7.  Let `prototype` be [OrdinaryObjectCreate](ordinary-and-exotic-objects-behaviours.html#sec-ordinaryobjectcreate)([%GeneratorPrototype%](control-abstraction-objects.html#sec-properties-of-generator-prototype)).
8.  Perform !\u00a0[DefinePropertyOrThrow](abstract-operations.html#sec-definepropertyorthrow)(`closure`, `"prototype"`, PropertyDescriptor { `\[\[Value\]\]`: `prototype`, `\[\[Writable\]\]`: `true`, `\[\[Enumerable\]\]`: `false`, `\[\[Configurable\]\]`: `false`\u00a0}).
9.  Return `closure`.

[GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression) : function \* [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier) ( [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) }

1.  [Assert](notational-conventions.html#assert): `name` is not present.
2.  Set `name` to the [StringValue](ecmascript-language-expressions.html#sec-static-semantics-stringvalue) of [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier).
3.  Let `outerEnv` be the [running execution context](executable-code-and-execution-contexts.html#running-execution-context)'s LexicalEnvironment.
4.  Let `funcEnv` be [NewDeclarativeEnvironment](executable-code-and-execution-contexts.html#sec-newdeclarativeenvironment)(`outerEnv`).
5.  Perform !\u00a0`funcEnv`.CreateImmutableBinding(`name`, `false`).
6.  Let `privateEnv` be the [running execution context](executable-code-and-execution-contexts.html#running-execution-context)'s PrivateEnvironment.
7.  Let `sourceText` be the [source text matched by](notational-conventions.html#sec-algorithm-conventions-syntax-directed-operations) [GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression).
8.  Let `closure` be [OrdinaryFunctionCreate](ordinary-and-exotic-objects-behaviours.html#sec-ordinaryfunctioncreate)([%GeneratorFunction.prototype%](control-abstraction-objects.html#sec-properties-of-the-generatorfunction-prototype-object), `sourceText`, [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters), [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody), `non-lexical-this`, `funcEnv`, `privateEnv`).
9.  Perform [SetFunctionName](ordinary-and-exotic-objects-behaviours.html#sec-setfunctionname)(`closure`, `name`).
10.  Let `prototype` be [OrdinaryObjectCreate](ordinary-and-exotic-objects-behaviours.html#sec-ordinaryobjectcreate)([%GeneratorPrototype%](control-abstraction-objects.html#sec-properties-of-generator-prototype)).
11.  Perform !\u00a0[DefinePropertyOrThrow](abstract-operations.html#sec-definepropertyorthrow)(`closure`, `"prototype"`, PropertyDescriptor { `\[\[Value\]\]`: `prototype`, `\[\[Writable\]\]`: `true`, `\[\[Enumerable\]\]`: `false`, `\[\[Configurable\]\]`: `false`\u00a0}).
12.  Perform !\u00a0`funcEnv`.InitializeBinding(`name`, `closure`).
13.  Return `closure`.

Note

The [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier) in a [GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression) can be referenced from inside the [GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression)'s [FunctionBody](ecmascript-language-functions-and-classes.html#prod-FunctionBody) to allow the generator code to call itself recursively. However, unlike in a [GeneratorDeclaration](ecmascript-language-functions-and-classes.html#prod-GeneratorDeclaration), the [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier) in a [GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression) cannot be referenced from and does not affect the scope enclosing the [GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression).

### 15.5.5 Runtime Semantics: Evaluation

[GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression) : function \* [BindingIdentifier](ecmascript-language-expressions.html#prod-BindingIdentifier)opt ( [FormalParameters](ecmascript-language-functions-and-classes.html#prod-FormalParameters) ) { [GeneratorBody](ecmascript-language-functions-and-classes.html#prod-GeneratorBody) }

1.  Return [InstantiateGeneratorFunctionExpression](ecmascript-language-functions-and-classes.html#sec-runtime-semantics-instantiategeneratorfunctionexpression) of [GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression).

[YieldExpression](ecmascript-language-functions-and-classes.html#prod-YieldExpression) : yield

1.  Return ?\u00a0[Yield](control-abstraction-objects.html#sec-yield)(`undefined`).

[YieldExpression](ecmascript-language-functions-and-classes.html#prod-YieldExpression) : yield [AssignmentExpression](ecmascript-language-expressions.html#prod-AssignmentExpression)

1.  Let `exprRef` be ?\u00a0[Evaluation](syntax-directed-operations.html#sec-evaluation) of [AssignmentExpression](ecmascript-language-expressions.html#prod-AssignmentExpression).
2.  Let `value` be ?\u00a0[GetValue](ecmascript-data-types-and-values.html#sec-getvalue)(`exprRef`).
3.  Return ?\u00a0[Yield](control-abstraction-objects.html#sec-yield)(`value`).

[YieldExpression](ecmascript-language-functions-and-classes.html#prod-YieldExpression) : yield \* [AssignmentExpression](ecmascript-language-expressions.html#prod-AssignmentExpression)

1.  Let `generatorKind` be [GetGeneratorKind](control-abstraction-objects.html#sec-getgeneratorkind)().
2.  [Assert](notational-conventions.html#assert): `generatorKind` is either `sync` or `async`.
3.  Let `exprRef` be ?\u00a0[Evaluation](syntax-directed-operations.html#sec-evaluation) of [AssignmentExpression](ecmascript-language-expressions.html#prod-AssignmentExpression).
4.  Let `value` be ?\u00a0[GetValue](ecmascript-data-types-and-values.html#sec-getvalue)(`exprRef`).
5.  Let `iteratorRecord` be ?\u00a0[GetIterator](abstract-operations.html#sec-getiterator)(`value`, `generatorKind`).
6.  Let `iterator` be `iteratorRecord`.`\[\[Iterator\]\]`.
7.  Let `received` be [NormalCompletion](ecmascript-data-types-and-values.html#sec-normalcompletion)(`undefined`).
8.  Repeat,
	1.  If `received` is a [normal completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
		1.  Let `innerResult` be ?\u00a0[Call](abstract-operations.html#sec-call)(`iteratorRecord`.`\[\[NextMethod\]\]`, `iteratorRecord`.`\[\[Iterator\]\]`, \u00ab `received`.`\[\[Value\]\]`\u00a0\u00bb).
		2.  If `generatorKind` is `async`, set `innerResult` to ?\u00a0[Await](control-abstraction-objects.html#await)(`innerResult`).
		3.  If `innerResult` [is not an Object](ecmascript-data-types-and-values.html#sec-object-type), throw a `TypeError` exception.
		4.  Let `done` be ?\u00a0[IteratorComplete](abstract-operations.html#sec-iteratorcomplete)(`innerResult`).
		5.  If `done` is `true`, then
			1.  Return ?\u00a0[IteratorValue](abstract-operations.html#sec-iteratorvalue)(`innerResult`).
		6.  If `generatorKind` is `async`, set `received` to [Completion](notational-conventions.html#sec-completion-ao)([AsyncGeneratorYield](control-abstraction-objects.html#sec-asyncgeneratoryield)(? [IteratorValue](abstract-operations.html#sec-iteratorvalue)(`innerResult`))).
		7.  Else, set `received` to [Completion](notational-conventions.html#sec-completion-ao)([GeneratorYield](control-abstraction-objects.html#sec-generatoryield)(`innerResult`)).
	2.  Else if `received` is a [throw completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type), then
		1.  Let `throw` be ?\u00a0[GetMethod](abstract-operations.html#sec-getmethod)(`iterator`, `"throw"`).
		2.  If `throw` is not `undefined`, then
			1.  Let `innerResult` be ?\u00a0[Call](abstract-operations.html#sec-call)(`throw`, `iterator`, \u00ab `received`.`\[\[Value\]\]`\u00a0\u00bb).
			2.  If `generatorKind` is `async`, set `innerResult` to ?\u00a0[Await](control-abstraction-objects.html#await)(`innerResult`).
			3.  NOTE: Exceptions from the inner [iterator](control-abstraction-objects.html#sec-iterator-interface) `throw` method are propagated. [Normal completions](ecmascript-data-types-and-values.html#sec-completion-record-specification-type) from an inner `throw` method are processed similarly to an inner `next`.
			4.  If `innerResult` [is not an Object](ecmascript-data-types-and-values.html#sec-object-type), throw a `TypeError` exception.
			5.  Let `done` be ?\u00a0[IteratorComplete](abstract-operations.html#sec-iteratorcomplete)(`innerResult`).
			6.  If `done` is `true`, then
				1.  Return ?\u00a0[IteratorValue](abstract-operations.html#sec-iteratorvalue)(`innerResult`).
			7.  If `generatorKind` is `async`, set `received` to [Completion](notational-conventions.html#sec-completion-ao)([AsyncGeneratorYield](control-abstraction-objects.html#sec-asyncgeneratoryield)(? [IteratorValue](abstract-operations.html#sec-iteratorvalue)(`innerResult`))).
			8.  Else, set `received` to [Completion](notational-conventions.html#sec-completion-ao)([GeneratorYield](control-abstraction-objects.html#sec-generatoryield)(`innerResult`)).
		3.  Else,
			1.  NOTE: If `iterator` does not have a `throw` method, this throw is going to terminate the `yield*` loop. But first we need to give `iterator` a chance to clean up.
			2.  Let `closeCompletion` be [NormalCompletion](ecmascript-data-types-and-values.html#sec-normalcompletion)(`empty`).
			3.  If `generatorKind` is `async`, perform ?\u00a0[AsyncIteratorClose](abstract-operations.html#sec-asynciteratorclose)(`iteratorRecord`, `closeCompletion`).
			4.  Else, perform ?\u00a0[IteratorClose](abstract-operations.html#sec-iteratorclose)(`iteratorRecord`, `closeCompletion`).
			5.  NOTE: The next step throws a `TypeError` to indicate that there was a `yield*` protocol violation: `iterator` does not have a `throw` method.
			6.  Throw a `TypeError` exception.
	3.  Else,
		1.  [Assert](notational-conventions.html#assert): `received` is a [return completion](ecmascript-data-types-and-values.html#sec-completion-record-specification-type).
		2.  Let `return` be ?\u00a0[GetMethod](abstract-operations.html#sec-getmethod)(`iterator`, `"return"`).
		3.  If `return` is `undefined`, then
			1.  Let `receivedValue` be `received`.`\[\[Value\]\]`.
			2.  If `generatorKind` is `async`, then
				1.  Set `receivedValue` to ?\u00a0[Await](control-abstraction-objects.html#await)(`receivedValue`).
			3.  Return [ReturnCompletion](ecmascript-data-types-and-values.html#sec-returncompletion)(`receivedValue`).
		4.  Let `innerReturnResult` be ?\u00a0[Call](abstract-operations.html#sec-call)(`return`, `iterator`, \u00ab `received`.`\[\[Value\]\]`\u00a0\u00bb).
		5.  If `generatorKind` is `async`, set `innerReturnResult` to ?\u00a0[Await](control-abstraction-objects.html#await)(`innerReturnResult`).
		6.  If `innerReturnResult` [is not an Object](ecmascript-data-types-and-values.html#sec-object-type), throw a `TypeError` exception.
		7.  Let `done` be ?\u00a0[IteratorComplete](abstract-operations.html#sec-iteratorcomplete)(`innerReturnResult`).
		8.  If `done` is `true`, then
			1.  Let `returnedValue` be ?\u00a0[IteratorValue](abstract-operations.html#sec-iteratorvalue)(`innerReturnResult`).
			2.  Return [ReturnCompletion](ecmascript-data-types-and-values.html#sec-returncompletion)(`returnedValue`).
		9.  If `generatorKind` is `async`, set `received` to [Completion](notational-conventions.html#sec-completion-ao)([AsyncGeneratorYield](control-abstraction-objects.html#sec-asyncgeneratoryield)(? [IteratorValue](abstract-operations.html#sec-iteratorvalue)(`innerReturnResult`))).
		10.  Else, set `received` to [Completion](notational-conventions.html#sec-completion-ao)([GeneratorYield](control-abstraction-objects.html#sec-generatoryield)(`innerReturnResult`)).
