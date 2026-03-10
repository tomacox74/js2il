# ECMA-262 Feature Status - Quick Reference

> **Last Updated**: 2026-02-12  
> **Quick lookup for feature support status and implementation plans**
> **Status**: Historical snapshot; may not reflect latest releases

> **Note (2026-02-17)**: This file predates recent runtime/spec work. For active prioritization use `docs/tracking-issues/TriageScoreboard.md`, and for shipped behavior cross-check `CHANGELOG.md` plus section/module JSON docs.

## How to Use This Guide

- ✅ **Supported** - Feature is implemented and works
- ⚠️ **Supported with Limitations** - Works but has known edge cases
- 🔶 **Incomplete** - Partial implementation, don't rely on it
- ❌ **Not Yet Supported** - Not implemented
- 📅 **Planned** - In the implementation roadmap

---

## REST & SPREAD

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| Rest parameters (`...args`) | ❌ Not Yet Supported | 📅 Phase 1 (Week 1-2) | CRITICAL - Top priority |
| Spread in function calls | ❌ Not Yet Supported | 📅 Phase 1 (Week 3-4) | CRITICAL - Enables modern patterns |
| Spread in arrays `[...arr]` | 🔶 Incomplete | 📅 Phase 1 (Week 3-4) | Partial support exists |
| Spread in objects `{...obj}` | 🔶 Incomplete | 📅 Phase 1 (Week 5-6) | Partial support exists |

**Why Critical**: Used in ~90% of modern JavaScript code, blocks React/Vue/Angular adoption.

---

## OBJECT METHODS

| Method | Status | Planned | Notes |
|--------|--------|---------|-------|
| `Object.keys()` | ❌ Not Yet Supported | 📅 Phase 2 (Week 1) | HIGH - Very common |
| `Object.values()` | ❌ Not Yet Supported | 📅 Phase 2 (Week 1) | HIGH - Very common |
| `Object.entries()` | ❌ Not Yet Supported | 📅 Phase 2 (Week 1) | HIGH - Very common |
| `Object.assign()` | ❌ Not Yet Supported | 📅 Phase 2 (Week 2) | HIGH - Redux/state management |
| `Object.hasOwn()` | ❌ Not Yet Supported | 📅 Phase 2 (Week 2) | Modern replacement for hasOwnProperty |
| `Object.is()` | ❌ Not Yet Supported | 📅 Phase 2 (Week 2) | Strict equality with NaN handling |
| `Object.create()` | ⚠️ Supported with Limitations | - | Works but has edge cases |
| `Object.defineProperty()` | 🔶 Incomplete | - | Partial support |

---

## ARRAY METHODS

| Method | Status | Planned | Notes |
|--------|--------|---------|-------|
| `Array.isArray()` | ✅ Supported | - | Works! |
| `Array.from()` | ❌ Not Yet Supported | 📅 Phase 3 (Week 1-2) | HIGH - Converts iterables |
| `Array.of()` | ❌ Not Yet Supported | - | Lower priority |
| `Array.prototype.map()` | ✅ Supported | - | Works! |
| `Array.prototype.filter()` | ✅ Supported | - | Works! |
| `Array.prototype.reduce()` | ✅ Supported | - | Works! |
| `Array.prototype.forEach()` | ❌ Not Yet Supported | 📅 Phase 3 (Week 1-2) | HIGH - Most common iteration |
| `Array.prototype.every()` | ❌ Not Yet Supported | 📅 Phase 3 (Week 1-2) | Validation patterns |
| `Array.prototype.some()` | ❌ Not Yet Supported | 📅 Phase 3 (Week 1-2) | Validation patterns |
| `Array.prototype.includes()` | ❌ Not Yet Supported | 📅 Phase 3 (Week 1-2) | HIGH - Better than indexOf |
| `Array.prototype.find()` | ✅ Supported | - | Works! |
| `Array.prototype.findIndex()` | ✅ Supported | - | Works! |
| `Array.prototype.flat()` | ❌ Not Yet Supported | 📅 Phase 3 (Week 3-4) | Flatten nested arrays |
| `Array.prototype.flatMap()` | ❌ Not Yet Supported | 📅 Phase 3 (Week 3-4) | Map + flatten |
| `Array.prototype.at()` | ❌ Not Yet Supported | 📅 Phase 3 (Week 3-4) | Negative indices |
| `Array.prototype.concat()` | ✅ Supported | - | Works! |
| `Array.prototype.slice()` | ✅ Supported | - | Works! |
| `Array.prototype.splice()` | ✅ Supported | - | Works! |
| `Array.prototype.join()` | ✅ Supported | - | Works! |
| `Array.prototype.reverse()` | ✅ Supported | - | Works! |
| `Array.prototype.sort()` | ✅ Supported | - | Works! |

---

## STRING METHODS

| Method | Status | Planned | Notes |
|--------|--------|---------|-------|
| `String.prototype.charAt()` | ✅ Supported | - | Works! |
| `String.prototype.charCodeAt()` | ✅ Supported | - | Works! |
| `String.prototype.concat()` | ✅ Supported | - | Works! |
| `String.prototype.includes()` | ✅ Supported | - | Works! |
| `String.prototype.indexOf()` | ✅ Supported | - | Works! |
| `String.prototype.lastIndexOf()` | ✅ Supported | - | Works! |
| `String.prototype.slice()` | ✅ Supported | - | Works! |
| `String.prototype.substring()` | ✅ Supported | - | Works! |
| `String.prototype.toLowerCase()` | ✅ Supported | - | Works! |
| `String.prototype.toUpperCase()` | ✅ Supported | - | Works! |
| `String.prototype.trim()` | ✅ Supported | - | Works! |
| `String.prototype.split()` | ✅ Supported | - | Works! |
| `String.prototype.replace()` | ✅ Supported | - | Works! |
| `String.prototype.match()` | ✅ Supported | - | Works! |
| `String.prototype.search()` | ✅ Supported | - | Works! |
| `String.prototype.startsWith()` | ✅ Supported | - | Works! |
| `String.prototype.endsWith()` | ✅ Supported | - | Works! |
| `String.prototype.repeat()` | ✅ Supported | - | Works! |
| `String.prototype.padStart()` | ✅ Supported | - | Works! |
| `String.prototype.padEnd()` | ✅ Supported | - | Works! |

---

## ASYNC & PROMISES

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| `Promise` | ✅ Supported | - | Works! |
| `Promise.all()` | ✅ Supported | - | Works! |
| `Promise.race()` | ✅ Supported | - | Works! |
| `Promise.resolve()` | ✅ Supported | - | Works! |
| `Promise.reject()` | ✅ Supported | - | Works! |
| `async/await` | ✅ Supported | - | Works! |
| Async generators (`async function*`) | ❌ Not Yet Supported | 📅 Phase 4 (Week 1-2) | Advanced feature |
| `for await...of` | ❌ Not Yet Supported | 📅 Phase 4 (Week 1-2) | Requires async generators |

---

## COLLECTIONS

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| `Map` | ✅ Supported | - | Works! |
| `Set` | ✅ Supported | - | Works! |
| `WeakMap` | ✅ Supported | - | Works! |
| `WeakSet` | ✅ Supported | - | Works! |

---

## FUNCTIONS & CLASSES

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| Arrow functions | ✅ Supported | - | Works! |
| Default parameters | ✅ Supported | - | Works! |
| Rest parameters | ❌ Not Yet Supported | 📅 Phase 1 (Week 1-2) | CRITICAL - See above |
| Generators (`function*`) | ✅ Supported | - | Works! |
| Async generators | ❌ Not Yet Supported | 📅 Phase 4 (Week 1-2) | See above |
| Classes | ✅ Supported | - | Works! |
| Class inheritance | ✅ Supported | - | Works! |
| Static methods | ✅ Supported | - | Works! |
| Getters/Setters | ❌ Not Yet Supported | 📅 Phase 4 (Week 3-4) | Rejected by validator |
| Private fields | ✅ Supported | - | Works! (with name mangling) |

---

## CONTROL FLOW

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| `if/else` | ✅ Supported | - | Works! |
| `switch/case` | ✅ Supported | - | Works! |
| `for` loop | ✅ Supported | - | Works! |
| `for...in` | ✅ Supported | - | Works! |
| `for...of` | ✅ Supported | - | Works! |
| `while` | ✅ Supported | - | Works! |
| `do...while` | ✅ Supported | - | Works! |
| `break/continue` | ✅ Supported | - | Works! |
| `try/catch/finally` | ✅ Supported | - | Works! |
| `throw` | ✅ Supported | - | Works! |

---

## OPERATORS

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| Arithmetic (`+`, `-`, `*`, `/`, `%`) | ✅ Supported | - | Works! |
| Comparison (`==`, `===`, `!=`, `!==`, `<`, `>`, etc.) | ✅ Supported | - | Works! |
| Logical (`&&`, `\|\|`, `!`) | ✅ Supported | - | Works! |
| Bitwise (`&`, `\|`, `^`, `~`, `<<`, `>>`, `>>>`) | ✅ Supported | - | Works! |
| Assignment (`=`, `+=`, `-=`, etc.) | ✅ Supported | - | Works! |
| Ternary (`? :`) | ✅ Supported | - | Works! |
| Spread (`...`) | 🔶 Incomplete | 📅 Phase 1 (Week 3-6) | See above |
| Destructuring | ⚠️ Supported with Limitations | - | Arrays work, objects partial |
| Optional chaining (`?.`) | ❌ Not Yet Supported | - | Not yet planned |
| Nullish coalescing (`??`) | ❌ Not Yet Supported | - | Not yet planned |

---

## LITERALS & TEMPLATES

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| Template literals | ✅ Supported | - | Works! |
| Template expressions `${expr}` | ✅ Supported | - | Works! |
| Tagged templates | ❌ Not Yet Supported | - | Lower priority |
| Object literals | ✅ Supported | - | Works! |
| Array literals | ✅ Supported | - | Works! |
| Computed property names | ✅ Supported | - | Works! |

---

## VARIABLES

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| `var` | ✅ Supported | - | Works! |
| `let` | ✅ Supported | - | Block scoping works |
| `const` | ✅ Supported | - | Block scoping works |
| Temporal Dead Zone (TDZ) | 🔶 Incomplete | 📅 Phase 4 (Week 5-6) | Partial enforcement |

---

## SYMBOLS

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| `Symbol()` | 🔶 Incomplete | 📅 Phase 4 (Week 3-4) | Partial support |
| `Symbol.iterator` | ❌ Not Yet Supported | 📅 Phase 4 (Week 3-4) | Well-known symbols |
| `Symbol.toStringTag` | ❌ Not Yet Supported | 📅 Phase 4 (Week 3-4) | Well-known symbols |

---

## REGULAR EXPRESSIONS

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| RegExp literals | ✅ Supported | - | Works! |
| RegExp methods | ✅ Supported | - | `test()`, `exec()` work |
| String regex methods | ✅ Supported | - | `match()`, `replace()`, etc. work |

---

## JSON

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| `JSON.parse()` | ✅ Supported | - | Works! |
| `JSON.stringify()` | ✅ Supported | - | Works! |

---

## MATH

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| `Math.abs()` | ✅ Supported | - | Works! |
| `Math.ceil()` | ✅ Supported | - | Works! |
| `Math.floor()` | ✅ Supported | - | Works! |
| `Math.round()` | ✅ Supported | - | Works! |
| `Math.max()` | ✅ Supported | - | Works! |
| `Math.min()` | ✅ Supported | - | Works! |
| `Math.pow()` | ✅ Supported | - | Works! |
| `Math.sqrt()` | ✅ Supported | - | Works! |
| `Math.random()` | ✅ Supported | - | Works! |
| Other Math methods | ✅ Supported | - | Most work! |

---

## NUMBER & DATE

| Feature | Status | Planned | Notes |
|---------|--------|---------|-------|
| `Number.isNaN()` | ✅ Supported | - | Works! |
| `Number.isFinite()` | ✅ Supported | - | Works! |
| `Number.parseInt()` | ✅ Supported | - | Works! |
| `Number.parseFloat()` | ✅ Supported | - | Works! |
| `Date` | ✅ Supported | - | Works! |
| Date methods | ✅ Supported | - | Most work! |

---

## WORKAROUNDS FOR UNSUPPORTED FEATURES

### Rest Parameters
**Instead of**:
```javascript
function sum(...numbers) {
  return numbers.reduce((a, b) => a + b, 0);
}
```

**Use**:
```javascript
function sum() {
  const numbers = Array.prototype.slice.call(arguments);
  return numbers.reduce((a, b) => a + b, 0);
}
```

### Spread in Function Calls
**Instead of**:
```javascript
const max = Math.max(...numbers);
```

**Use**:
```javascript
const max = Math.max.apply(null, numbers);
```

### Object.keys/values/entries
**Instead of**:
```javascript
Object.keys(obj).forEach(key => { ... });
```

**Use**:
```javascript
for (const key in obj) {
  if (obj.hasOwnProperty(key)) {
    // process key
  }
}
```

### Object.assign
**Instead of**:
```javascript
const merged = Object.assign({}, obj1, obj2);
```

**Use**:
```javascript
const merged = {};
for (const key in obj1) {
  if (obj1.hasOwnProperty(key)) merged[key] = obj1[key];
}
for (const key in obj2) {
  if (obj2.hasOwnProperty(key)) merged[key] = obj2[key];
}
```

### Array.includes
**Instead of**:
```javascript
if (array.includes(value)) { ... }
```

**Use**:
```javascript
if (array.indexOf(value) !== -1) { ... }
```

---

## IMPLEMENTATION TIMELINE

```
Phase 1 (4-6 weeks):  Rest/Spread          🔴 CRITICAL
Phase 2 (2-3 weeks):  Object utilities     🟡 HIGH
Phase 3 (3-4 weeks):  Array methods        🟡 HIGH
Phase 4 (4-6 weeks):  Advanced features    🟢 MEDIUM
```

**Total**: ~4-6 months for complete modern JavaScript support

---

## NEED A FEATURE NOT LISTED?

1. Check the [full roadmap](./FeatureImplementationRoadmap.md) for detailed analysis
2. Review the [ECMA-262 Index](../ECMA262/Index.md) for complete coverage status
3. File an issue if you need a specific feature prioritized

---

*Last updated: February 12, 2026*  
*Based on ECMA-262 specification analysis*
