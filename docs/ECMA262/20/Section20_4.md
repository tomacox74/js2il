<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 20.4: Symbol Objects

[Back to Section20](Section20.md) | [Back to Index](../Index.md)

JS2IL provides a minimal Symbol implementation sufficient for basic callable usage and typeof/equality semantics. Well-known symbols, registry APIs (Symbol.for/keyFor), and full Symbol.prototype surface are not yet implemented.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 20.4 | Symbol Objects | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-symbol-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 20.4.1 | The Symbol Constructor | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-symbol-constructor) |
| 20.4.1.1 | Symbol ( [ description ] ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-symbol-description) |
| 20.4.2 | Properties of the Symbol Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-symbol-constructor) |
| 20.4.2.1 | Symbol.asyncIterator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.asynciterator) |
| 20.4.2.2 | Symbol.for ( key ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.for) |
| 20.4.2.3 | Symbol.hasInstance | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.hasinstance) |
| 20.4.2.4 | Symbol.isConcatSpreadable | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.isconcatspreadable) |
| 20.4.2.5 | Symbol.iterator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.iterator) |
| 20.4.2.6 | Symbol.keyFor ( sym ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.keyfor) |
| 20.4.2.7 | Symbol.match | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.match) |
| 20.4.2.8 | Symbol.matchAll | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.matchall) |
| 20.4.2.9 | Symbol.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype) |
| 20.4.2.10 | Symbol.replace | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.replace) |
| 20.4.2.11 | Symbol.search | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.search) |
| 20.4.2.12 | Symbol.species | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.species) |
| 20.4.2.13 | Symbol.split | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.split) |
| 20.4.2.14 | Symbol.toPrimitive | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.toprimitive) |
| 20.4.2.15 | Symbol.toStringTag | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.tostringtag) |
| 20.4.2.16 | Symbol.unscopables | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.unscopables) |
| 20.4.3 | Properties of the Symbol Prototype Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-symbol-prototype-object) |
| 20.4.3.1 | Symbol.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype.constructor) |
| 20.4.3.2 | get Symbol.prototype.description | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype.description) |
| 20.4.3.3 | Symbol.prototype.toString ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype.tostring) |
| 20.4.3.3.1 | SymbolDescriptiveString ( sym ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symboldescriptivestring) |
| 20.4.3.4 | Symbol.prototype.valueOf ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype.valueof) |
| 20.4.3.4.1 | ThisSymbolValue ( value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-thissymbolvalue) |
| 20.4.3.5 | Symbol.prototype [ %Symbol.toPrimitive% ] ( hint ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype-%symbol.toprimitive%) |
| 20.4.3.6 | Symbol.prototype [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype-%symbol.tostringtag%) |
| 20.4.4 | Properties of Symbol Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-symbol-instances) |
| 20.4.5 | Abstract Operations for Symbols | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-symbols) |
| 20.4.5.1 | KeyForSymbol ( sym ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-keyforsymbol) |

## Support

Feature-level support tracking with test script references.

### 20.4.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-symbol-description))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Symbol([description]) callable (basic) | Partially Supported | [`IntrinsicCallables_Symbol_Callable_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_Symbol_Callable_Basic.js) | Supports callable invocation with 0/1 arguments, unique symbol instances, and typeof === 'symbol'. Does not implement the full Symbol registry or well-known symbols. |

