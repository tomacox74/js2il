# JS2IL Feature Implementation Roadmap

> **Last Updated**: 2026-02-12  
> **Based on**: ECMA-262 Documentation Analysis (docs/ECMA262)
> **Status**: Historical planning snapshot for sequencing context

> **Note (2026-02-17)**: Active execution sequencing now lives in `docs/tracking-issues/TriageScoreboard.md`. Use this roadmap for background rationale and implementation notes.

## Executive Summary

This document provides a prioritized roadmap for implementing ECMA-262 features in JS2IL, based on:
- Analysis of all ECMA-262 section documentation
- Modern JavaScript usage patterns
- Partner/developer demand
- Framework/library ecosystem requirements

**Key Finding**: Rest/spread parameters are the highest-priority blocking features that enable modern JavaScript patterns used across React, Vue, Angular, and most npm packages.

---

## Tier 1: CRITICAL (Foundational - Implement First)

### 1. Rest Parameters (`...args`)
**Status**: Not Yet Supported  
**ECMA Sections**: 15.1, 13.2  
**Priority**: 🔴 **CRITICAL**

**Why Critical**:
- Essential for modern JS/TS function signatures
- Used extensively in frameworks (React hooks, Vue composables)
- Enables variadic functions and flexible APIs
- Dependency for many modern npm packages

**Usage Examples**:
```javascript
function sum(...numbers) {
  return numbers.reduce((a, b) => a + b, 0);
}

const combine = (...arrays) => [].concat(...arrays);
```

**Implementation Notes**:
- Parse `...identifier` in function parameter lists
- Create array from remaining arguments
- Handle interaction with named parameters

---

### 2. Spread Operator in Function Calls
**Status**: Not Yet Supported  
**ECMA Section**: 13.2  
**Priority**: 🔴 **CRITICAL**

**Why Critical**:
- Complements rest parameters
- Used in nearly every modern JavaScript codebase
- Critical for functional programming patterns
- Enables dynamic argument passing

**Usage Examples**:
```javascript
const max = Math.max(...numbers);
array.push(...items);
func(...args);
```

**Implementation Notes**:
- Parse `...expression` in argument lists
- Expand iterable into individual arguments
- Handle multiple spreads in single call

---

### 3. Spread in Array Literals
**Status**: Partially Implemented  
**ECMA Section**: 13.2  
**Priority**: 🟡 **HIGH**

**Why Important**:
- Array concatenation and cloning
- Common pattern in state management (Redux, etc.)

**Usage Examples**:
```javascript
const combined = [...arr1, ...arr2];
const clone = [...original];
const extended = [1, 2, ...rest, 9, 10];
```

---

### 4. Spread in Object Literals
**Status**: Partially Implemented  
**ECMA Section**: 13.2  
**Priority**: 🟡 **HIGH**

**Why Important**:
- Object merging (common in React props, state updates)
- Shallow cloning pattern

**Usage Examples**:
```javascript
const merged = { ...obj1, ...obj2 };
const withDefaults = { ...defaults, ...userOptions };
const updated = { ...state, count: state.count + 1 };
```

---

## Tier 2: HIGH VALUE (Common Utilities)

### 5. Object.keys()
**Status**: Not Yet Supported  
**ECMA Section**: 20.1  
**Priority**: 🟡 **HIGH**

**Why Important**:
- Core utility used in nearly every application
- Enables object iteration patterns
- Foundation for object manipulation

**Usage Examples**:
```javascript
Object.keys(obj).forEach(key => {
  console.log(key, obj[key]);
});

const hasProperties = Object.keys(obj).length > 0;
```

---

### 6. Object.values()
**Status**: Not Yet Supported  
**ECMA Section**: 20.1  
**Priority**: 🟡 **HIGH**

**Usage Examples**:
```javascript
const sum = Object.values(scores).reduce((a, b) => a + b, 0);
```

---

### 7. Object.entries()
**Status**: Not Yet Supported  
**ECMA Section**: 20.1  
**Priority**: 🟡 **HIGH**

**Usage Examples**:
```javascript
for (const [key, value] of Object.entries(obj)) {
  console.log(`${key}: ${value}`);
}

const filtered = Object.fromEntries(
  Object.entries(obj).filter(([k, v]) => v > 0)
);
```

---

### 8. Object.assign()
**Status**: Not Yet Supported  
**ECMA Section**: 20.1  
**Priority**: 🟡 **HIGH**

**Why Important**:
- Essential for object merging
- Used in Redux, Angular, and many libraries
- Polyfill for object spread in older environments

**Usage Examples**:
```javascript
const merged = Object.assign({}, defaults, userOptions);
Object.assign(target, source1, source2);
```

---

### 9. Object.hasOwn()
**Status**: Not Yet Supported  
**ECMA Section**: 20.1  
**Priority**: 🟢 **MEDIUM**

**Why Important**:
- Modern replacement for `Object.prototype.hasOwnProperty.call()`
- Clean semantics without prototype issues

**Usage Examples**:
```javascript
if (Object.hasOwn(obj, 'property')) {
  // Safe check without prototype pollution
}
```

---

### 10. Object.is()
**Status**: Not Yet Supported  
**ECMA Section**: 20.1  
**Priority**: 🟢 **MEDIUM**

**Why Important**:
- Strict equality with correct NaN and -0 handling
- Used in React, testing frameworks

**Usage Examples**:
```javascript
Object.is(NaN, NaN);  // true (unlike ===)
Object.is(0, -0);     // false (unlike ===)
```

---

## Tier 3: IMPORTANT (Framework Enablers)

### 11. Array.from()
**Status**: Not Yet Supported  
**ECMA Section**: 23.1  
**Priority**: 🟡 **HIGH**

**Why Important**:
- Convert array-like objects and iterables to arrays
- Essential for DOM NodeLists, arguments object
- Enables functional programming patterns

**Usage Examples**:
```javascript
const arr = Array.from(arrayLike);
const chars = Array.from('hello');
const mapped = Array.from([1, 2, 3], x => x * 2);
```

---

### 12. Array.prototype.forEach()
**Status**: Not Yet Supported  
**ECMA Section**: 23.1  
**Priority**: 🟡 **HIGH**

**Usage Examples**:
```javascript
items.forEach((item, index) => {
  console.log(`${index}: ${item}`);
});
```

---

### 13. Array.prototype.every()
**Status**: Not Yet Supported  
**ECMA Section**: 23.1  
**Priority**: 🟢 **MEDIUM**

**Usage Examples**:
```javascript
const allPositive = numbers.every(n => n > 0);
```

---

### 14. Array.prototype.some()
**Status**: Not Yet Supported  
**ECMA Section**: 23.1  
**Priority**: 🟢 **MEDIUM**

**Usage Examples**:
```javascript
const hasNegative = numbers.some(n => n < 0);
```

---

### 15. Array.prototype.includes()
**Status**: Not Yet Supported  
**ECMA Section**: 23.1  
**Priority**: 🟡 **HIGH**

**Why Important**:
- Cleaner than `indexOf() !== -1`
- Handles NaN correctly

**Usage Examples**:
```javascript
if (array.includes(value)) {
  // Modern, readable check
}
```

---

### 16. Array.prototype.flat()
**Status**: Not Yet Supported  
**ECMA Section**: 23.1  
**Priority**: 🟢 **MEDIUM**

**Usage Examples**:
```javascript
const flattened = [[1, 2], [3, 4]].flat();
const deepFlat = [1, [2, [3, [4]]]].flat(Infinity);
```

---

### 17. Array.prototype.flatMap()
**Status**: Not Yet Supported  
**ECMA Section**: 23.1  
**Priority**: 🟢 **MEDIUM**

**Usage Examples**:
```javascript
const result = arr.flatMap(x => [x, x * 2]);
```

---

### 18. Array.prototype.at()
**Status**: Not Yet Supported  
**ECMA Section**: 23.1  
**Priority**: 🟢 **MEDIUM**

**Why Important**:
- Enables negative indices (like Python)
- Cleaner than `arr[arr.length - 1]`

**Usage Examples**:
```javascript
const last = arr.at(-1);
const secondLast = arr.at(-2);
```

---

## Tier 4: ADVANCED (Lower Immediate Demand)

### 19. async generators (`async function*`)
**Status**: Not Yet Supported  
**ECMA Section**: 15.6  
**Priority**: 🟢 **MEDIUM**

**Why Important**:
- Enables async iteration patterns
- Critical for streams/observables
- Modern data processing pipelines

**Usage Examples**:
```javascript
async function* asyncGenerator() {
  for (let i = 0; i < 3; i++) {
    await delay(100);
    yield i;
  }
}

for await (const value of asyncGenerator()) {
  console.log(value);
}
```

---

### 20. Getter/Setter Syntax
**Status**: Rejected by validator  
**ECMA Section**: 15.4  
**Priority**: 🟢 **MEDIUM**

**Why Important**:
- Computed properties
- Encapsulation patterns
- Framework internals (Vue reactivity, etc.)

**Usage Examples**:
```javascript
const obj = {
  get fullName() {
    return `${this.first} ${this.last}`;
  },
  set fullName(value) {
    [this.first, this.last] = value.split(' ');
  }
};
```

---

### 21. Symbol Support
**Status**: Incomplete  
**ECMA Section**: 6, 20.4  
**Priority**: 🟢 **MEDIUM**

**Why Important**:
- Enables well-known symbols (Symbol.iterator, Symbol.toStringTag)
- Foundation for iteration protocol
- Custom iterator implementations

**Usage Examples**:
```javascript
const obj = {
  [Symbol.iterator]() {
    // Custom iteration logic
  }
};
```

---

### 22. Temporal Dead Zone (TDZ)
**Status**: Partial (let/const block scoping exists)  
**ECMA Section**: 14.2-14.3  
**Priority**: 🔵 **LOW**

**Why Important**:
- Proper let/const semantics
- Error on access before initialization
- Spec compliance

**Usage Examples**:
```javascript
{
  // TDZ starts
  console.log(x); // Should throw ReferenceError
  let x = 5;      // TDZ ends
}
```

---

## Implementation Strategy

### Phase 1: Rest/Spread Foundation (4-6 weeks)
**Goal**: Enable modern function signatures and array/object manipulation

1. **Week 1-2**: Rest parameters in function declarations
   - Update parser to handle `...identifier`
   - Generate IL to create argument array
   - Handle interaction with named parameters

2. **Week 3-4**: Spread operator in calls and arrays
   - Parse spread in call expressions
   - Implement array expansion in IL
   - Handle multiple spreads

3. **Week 5-6**: Spread in object literals
   - Parse object spread syntax
   - Generate property copying IL
   - Handle nested spreads

**Success Metrics**:
- All rest/spread patterns work
- Existing tests continue to pass
- New execution tests for common patterns

---

### Phase 2: Object Utilities (2-3 weeks)
**Goal**: Implement core Object static methods

1. **Week 1**: Object.keys/values/entries
   - Implement in JavaScriptRuntime
   - Add property enumeration logic
   - Handle prototype chain correctly

2. **Week 2**: Object.assign/hasOwn/is
   - Implement property copying
   - Add type checking and edge cases

**Success Metrics**:
- All Object utilities work correctly
- Handle edge cases (null, undefined, symbols)
- Performance benchmarks

---

### Phase 3: Array Methods (3-4 weeks)
**Goal**: Complete array iteration and manipulation APIs

1. **Week 1-2**: forEach/every/some/includes
   - Implement in JavaScriptRuntime
   - Add callback handling
   - Proper this binding

2. **Week 3-4**: flat/flatMap/at/from
   - Implement recursive flattening
   - Handle negative indices
   - Array-like conversion

**Success Metrics**:
- All array methods functional
- Edge cases handled
- Performance comparable to native

---

### Phase 4: Advanced Features (4-6 weeks)
**Goal**: Enable advanced language features

1. **Async generators** (2 weeks)
2. **Getter/setter syntax** (2 weeks)
3. **Symbol basics** (1-2 weeks)
4. **TDZ enforcement** (1 week)

---

## Testing Strategy

For each feature:

1. **Unit Tests**: Test individual feature in isolation
2. **Integration Tests**: Test feature in combination with existing features
3. **Execution Tests**: Run compiled JavaScript and verify output
4. **Generator Tests**: Verify generated IL is correct (snapshot tests)
5. **Edge Cases**: Null, undefined, type errors, etc.

---

## Partner Impact Analysis

### High-Impact Features (Immediate Unlock)
- **Rest/Spread**: Enables React, Vue, modern npm packages
- **Object.assign**: Required by Redux, state management libraries
- **Array methods**: Common in all modern applications

### Medium-Impact Features (Quality of Life)
- **Object.keys/values/entries**: Widely used but often workaroundable
- **Array.includes**: Nice syntax improvement over indexOf

### Low-Impact Features (Spec Compliance)
- **TDZ**: Correctness but rare in practice
- **Symbols**: Advanced use cases

---

## Dependencies and Blockers

### Current State
✅ **Already Implemented**:
- Async/await
- Generators
- Classes
- Arrow functions
- Template literals
- Destructuring (partial)
- Map/Set

❌ **Blocking Features**:
- Rest parameters → blocks modern function APIs
- Spread operator → blocks functional patterns
- Object utilities → blocks common patterns

### Implementation Order Rationale

**Why Rest/Spread First**:
1. Most frequently requested by partners
2. Blocks adoption of modern npm packages
3. Foundation for other features
4. Relatively self-contained implementation

**Why Object/Array Methods Next**:
1. High usage frequency
2. Relatively easy to implement
3. Quick wins for developer productivity
4. Enables more real-world applications

---

## Success Criteria

### Phase 1 Complete When:
- [ ] Can compile modern function signatures with rest parameters
- [ ] Can use spread in function calls, arrays, and objects
- [ ] All existing tests pass
- [ ] New execution tests cover common patterns

### Phase 2 Complete When:
- [ ] All Object static methods (keys/values/entries/assign/hasOwn/is) work
- [ ] Edge cases handled correctly
- [ ] Performance is acceptable

### Phase 3 Complete When:
- [ ] All priority array methods implemented
- [ ] Can compile typical array manipulation code
- [ ] Method chaining works correctly

### Phase 4 Complete When:
- [ ] Async generators work
- [ ] Getter/setter syntax supported
- [ ] Basic Symbol support enables iteration protocol

---

## Risk Assessment

### Low Risk
- Object/Array methods: Well-defined, runtime implementations
- Rest parameters: Clear parsing and IL generation strategy

### Medium Risk
- Spread operator: Multiple contexts, interaction with other features
- Getter/setter: Parser changes, property descriptor handling

### High Risk
- Symbol support: Impacts type system, iteration protocol
- Async generators: Complex interaction with async/await and generators

---

## Recommendations

### Immediate Actions (This Sprint)
1. ✅ Complete ECMA-262 documentation review
2. **Start Phase 1**: Begin rest parameter implementation
3. Set up test infrastructure for new features
4. Create tracking issues for each phase

### Short-term Goals (Next Quarter)
1. Complete Phase 1 (Rest/Spread)
2. Complete Phase 2 (Object utilities)
3. Start Phase 3 (Array methods)

### Long-term Goals (6 months)
1. Complete all Tier 1-3 features
2. Evaluate Tier 4 based on partner feedback
3. Consider additional ECMA-262 features based on usage data

---

## Appendix A: Feature Status Reference

Quick reference for status terminology used in ECMA-262 docs:

- **Supported**: Fully implemented, safe for production use
- **Supported with Limitations**: Works for common cases, some edge cases missing
- **Incomplete**: Partial implementation, not safe to rely on
- **Not Yet Supported**: Not implemented
- **Untracked**: Status not yet evaluated
- **N/A (informational)**: Spec clause is documentation, not a feature

---

## Appendix B: Related Documentation

- [ECMA-262 Index](./ECMA262/Index.md)
- [Prototype Chain Support](./PrototypeChainSupport.md)
- [Async/Await Implementation](./AsyncAwait_ThreeWay_Comparison.md)
- [Type Mapping](./JavaScriptToDotNetTypeMapping.md)

---

*This roadmap is a living document. Update as priorities change based on partner feedback and usage data.*
