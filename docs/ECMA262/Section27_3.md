<!-- AUTO-GENERATED: splitEcma262SectionsIntoSubsections.ps1 -->

# Section 27.3: GeneratorFunction Objects

[Back to Section27](Section27.md) | [Back to Index](Index.md)

_Lists clause numbers/titles/links only (no spec text) in the index above. See appendix for extracted spec text._ 

| Clause | Title | Status | Link |
|---:|---|---|---|
| 27.3 | GeneratorFunction Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 27.3.1 | The GeneratorFunction Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-constructor) |
| 27.3.1.1 | GeneratorFunction ( ...parameterArgs, bodyArg ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction) |
| 27.3.2 | Properties of the GeneratorFunction Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-generatorfunction-constructor) |
| 27.3.2.1 | GeneratorFunction.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction.prototype) |
| 27.3.3 | Properties of the GeneratorFunction Prototype Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-generatorfunction-prototype-object) |
| 27.3.3.1 | GeneratorFunction.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction.prototype.constructor) |
| 27.3.3.2 | GeneratorFunction.prototype.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction.prototype.prototype) |
| 27.3.3.3 | GeneratorFunction.prototype [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction.prototype-%symbol.tostringtag%) |
| 27.3.4 | GeneratorFunction Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-instances) |
| 27.3.4.1 | length | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-instances-length) |
| 27.3.4.2 | name | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-instances-name) |
| 27.3.4.3 | prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-instances-prototype) |

## Appendix: Extracted Spec Text (Converted)

This appendix is generated from a locally extracted tc39.es HTML fragment. It may be overwritten if this file is regenerated.

### 27.3 GeneratorFunction Objects

GeneratorFunctions are functions that are usually created by evaluating [GeneratorDeclaration](ecmascript-language-functions-and-classes.html#prod-GeneratorDeclaration)s, [GeneratorExpression](ecmascript-language-functions-and-classes.html#prod-GeneratorExpression)s, and [GeneratorMethod](ecmascript-language-functions-and-classes.html#prod-GeneratorMethod)s. They may also be created by calling the [%GeneratorFunction%](control-abstraction-objects.html#sec-generatorfunction-constructor) intrinsic.

    Figure 6 (Informative): Generator Objects Relationships

### 27.3.1 The GeneratorFunction Constructor

The GeneratorFunction [constructor](ecmascript-data-types-and-values.html#constructor):

- is %GeneratorFunction%.

- is a subclass of `Function`.

- creates and initializes a new GeneratorFunction when called as a function rather than as a [constructor](ecmascript-data-types-and-values.html#constructor). Thus the function call `GeneratorFunction (…)` is equivalent to the object creation expression `new GeneratorFunction (…)` with the same arguments.

- may be used as the value of an `extends` clause of a class definition. Subclass [constructors](ecmascript-data-types-and-values.html#constructor) that intend to inherit the specified GeneratorFunction behaviour must include a `super` call to the GeneratorFunction [constructor](ecmascript-data-types-and-values.html#constructor) to create and initialize subclass instances with the internal slots necessary for built-in GeneratorFunction behaviour. All ECMAScript syntactic forms for defining generator [function objects](ecmascript-data-types-and-values.html#function-object) create direct instances of GeneratorFunction. There is no syntactic means to create instances of GeneratorFunction subclasses.

### 27.3.1.1 GeneratorFunction ( ...`parameterArgs`, `bodyArg` )

The last argument (if any) specifies the body (executable code) of a generator function; any preceding arguments specify formal parameters.

This function performs the following steps when called:

- Let `C` be the [active function object](executable-code-and-execution-contexts.html#active-function-object).
- If `bodyArg` is not present, set `bodyArg` to the empty String.
- Return ? [CreateDynamicFunction](fundamental-objects.html#sec-createdynamicfunction)(`C`, NewTarget, `generator`, `parameterArgs`, `bodyArg`).

	Note

See NOTE for [20.2.1.1](fundamental-objects.html#sec-function-p1-p2-pn-body).

### 27.3.2 Properties of the GeneratorFunction Constructor

The GeneratorFunction [constructor](ecmascript-data-types-and-values.html#constructor):

- is a standard built-in [function object](ecmascript-data-types-and-values.html#function-object) that inherits from the Function [constructor](ecmascript-data-types-and-values.html#constructor).

- has a `[[Prototype]]` internal slot whose value is [%Function%](fundamental-objects.html#sec-function-constructor).

- has a "length" property whose value is `1`𝔽.

- has a "name" property whose value is "GeneratorFunction".

- has the following properties:

### 27.3.2.1 GeneratorFunction.prototype

The initial value of `GeneratorFunction.prototype` is the [GeneratorFunction prototype object](control-abstraction-objects.html#sec-properties-of-the-generatorfunction-prototype-object).

This property has the attributes { `[[Writable]]`: `false`, `[[Enumerable]]`: `false`, `[[Configurable]]`: `false` }.

### 27.3.3 Properties of the GeneratorFunction Prototype Object

The GeneratorFunction prototype object:

- is %GeneratorFunction.prototype% (see [Figure 6](control-abstraction-objects.html#figure-2)).

- is an [ordinary object](ecmascript-data-types-and-values.html#ordinary-object).

- is not a [function object](ecmascript-data-types-and-values.html#function-object) and does not have an `[[ECMAScriptCode]]` internal slot or any other of the internal slots listed in [Table 28](ordinary-and-exotic-objects-behaviours.html#table-internal-slots-of-ecmascript-function-objects) or [Table 91](control-abstraction-objects.html#table-internal-slots-of-generator-instances).

- has a `[[Prototype]]` internal slot whose value is [%Function.prototype%](fundamental-objects.html#sec-properties-of-the-function-prototype-object).

### 27.3.3.1 GeneratorFunction.prototype.constructor

The initial value of `GeneratorFunction.prototype.constructor` is [%GeneratorFunction%](control-abstraction-objects.html#sec-generatorfunction-constructor).

This property has the attributes { `[[Writable]]`: `false`, `[[Enumerable]]`: `false`, `[[Configurable]]`: `true` }.

### 27.3.3.2 GeneratorFunction.prototype.prototype

The initial value of `GeneratorFunction.prototype.prototype` is [%GeneratorPrototype%](control-abstraction-objects.html#sec-properties-of-generator-prototype).

This property has the attributes { `[[Writable]]`: `false`, `[[Enumerable]]`: `false`, `[[Configurable]]`: `true` }.

### 27.3.3.3 GeneratorFunction.prototype [ %Symbol.toStringTag% ]

The initial value of the [%Symbol.toStringTag%](ecmascript-data-types-and-values.html#sec-well-known-symbols) property is the String value "GeneratorFunction".

This property has the attributes { `[[Writable]]`: `false`, `[[Enumerable]]`: `false`, `[[Configurable]]`: `true` }.

### 27.3.4 GeneratorFunction Instances

Every GeneratorFunction instance is an ECMAScript [function object](ecmascript-data-types-and-values.html#function-object) and has the internal slots listed in [Table 28](ordinary-and-exotic-objects-behaviours.html#table-internal-slots-of-ecmascript-function-objects). The value of the `[[IsClassConstructor]]` internal slot for all such instances is `false`.

Each GeneratorFunction instance has the following own properties:

### 27.3.4.1 length

The specification for the "length" property of Function instances given in [20.2.4.1](fundamental-objects.html#sec-function-instances-length) also applies to GeneratorFunction instances.

### 27.3.4.2 name

The specification for the "name" property of Function instances given in [20.2.4.2](fundamental-objects.html#sec-function-instances-name) also applies to GeneratorFunction instances.

### 27.3.4.3 prototype

Whenever a GeneratorFunction instance is created another [ordinary object](ecmascript-data-types-and-values.html#ordinary-object) is also created and is the initial value of the generator function's "prototype" property. The value of the prototype property is used to initialize the `[[Prototype]]` internal slot of a newly created Generator when the generator [function object](ecmascript-data-types-and-values.html#function-object) is invoked using `[[Call]]`.

This property has the attributes { `[[Writable]]`: `true`, `[[Enumerable]]`: `false`, `[[Configurable]]`: `false` }.

	Note

Unlike Function instances, the object that is the value of a GeneratorFunction's "prototype" property does not have a "constructor" property whose value is the GeneratorFunction instance.
