<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 20.4: Symbol Objects

[Back to Section20](Section20.md) | [Back to Index](../Index.md)

JS2IL supports Symbol callable creation, global registry APIs (Symbol.for/keyFor), well-known symbols, and core Symbol.prototype behaviors (description/toString/valueOf). Some advanced descriptor-level semantics remain tracked as limitations.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 20.4 | Symbol Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 20.4.1 | The Symbol Constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-symbol-constructor) |
| 20.4.1.1 | Symbol ( [ description ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-symbol-description) |
| 20.4.2 | Properties of the Symbol Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-symbol-constructor) |
| 20.4.2.1 | Symbol.asyncIterator | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.asynciterator) |
| 20.4.2.2 | Symbol.for ( key ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.for) |
| 20.4.2.3 | Symbol.hasInstance | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.hasinstance) |
| 20.4.2.4 | Symbol.isConcatSpreadable | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.isconcatspreadable) |
| 20.4.2.5 | Symbol.iterator | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.iterator) |
| 20.4.2.6 | Symbol.keyFor ( sym ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.keyfor) |
| 20.4.2.7 | Symbol.match | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.match) |
| 20.4.2.8 | Symbol.matchAll | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.matchall) |
| 20.4.2.9 | Symbol.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype) |
| 20.4.2.10 | Symbol.replace | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.replace) |
| 20.4.2.11 | Symbol.search | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.search) |
| 20.4.2.12 | Symbol.species | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.species) |
| 20.4.2.13 | Symbol.split | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.split) |
| 20.4.2.14 | Symbol.toPrimitive | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.toprimitive) |
| 20.4.2.15 | Symbol.toStringTag | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.tostringtag) |
| 20.4.2.16 | Symbol.unscopables | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.unscopables) |
| 20.4.3 | Properties of the Symbol Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-symbol-prototype-object) |
| 20.4.3.1 | Symbol.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype.constructor) |
| 20.4.3.2 | get Symbol.prototype.description | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype.description) |
| 20.4.3.3 | Symbol.prototype.toString ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype.tostring) |
| 20.4.3.3.1 | SymbolDescriptiveString ( sym ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symboldescriptivestring) |
| 20.4.3.4 | Symbol.prototype.valueOf ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype.valueof) |
| 20.4.3.4.1 | ThisSymbolValue ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-thissymbolvalue) |
| 20.4.3.5 | Symbol.prototype [ %Symbol.toPrimitive% ] ( hint ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype-%symbol.toprimitive%) |
| 20.4.3.6 | Symbol.prototype [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-symbol.prototype-%symbol.tostringtag%) |
| 20.4.4 | Properties of Symbol Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-symbol-instances) |
| 20.4.5 | Abstract Operations for Symbols | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-symbols) |
| 20.4.5.1 | KeyForSymbol ( sym ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-keyforsymbol) |

## Support

Feature-level support tracking with test script references.

### 20.4.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-symbol-description))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Symbol([description]) callable (basic) | Supported with Limitations | [`IntrinsicCallables_Symbol_Callable_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_Symbol_Callable_Basic.js) | Supports callable invocation with 0/1 arguments, unique symbol instances, and typeof === 'symbol'. Does not implement the full Symbol registry or well-known symbols. |

### 20.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-symbol-constructor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Symbol constructor properties (well-known symbols) | Supported with Limitations | [`IntrinsicCallables_Symbol_Registry_WellKnown.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_Symbol_Registry_WellKnown.js) | Supports Symbol.iterator, Symbol.asyncIterator, Symbol.hasInstance, Symbol.isConcatSpreadable, Symbol.match, Symbol.matchAll, Symbol.replace, Symbol.search, Symbol.species, Symbol.split, Symbol.toPrimitive, Symbol.toStringTag, and Symbol.unscopables as stable singletons. |

### 20.4.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-symbol.for))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Symbol.for(key) and Symbol.keyFor(sym) registry APIs | Supported | [`IntrinsicCallables_Symbol_Registry_WellKnown.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_Symbol_Registry_WellKnown.js) | Supports global symbol registry round-tripping and TypeError on non-symbol Symbol.keyFor input. |

### 20.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-symbol-prototype-object))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Symbol.prototype core surface | Supported | [`IntrinsicCallables_Symbol_Prototype_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_Symbol_Prototype_Basic.js) | Supports description, toString(), and valueOf() behavior for symbol instances. |

