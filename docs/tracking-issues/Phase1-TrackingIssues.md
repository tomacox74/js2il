# Phase 1: Rest/Spread Foundation - Tracking Issues

> **Created**: 2026-02-12  
> **Phase Duration**: 4-6 weeks  
> **Goal**: Enable modern function signatures and array/object manipulation

This document contains the tracking issues for Phase 1 features. These issues are ready to be created in GitHub.

## Current State (2026-02-18)

- ✅ Core implementation for all 4 Phase 1 issues is in place (rest parameters; spread in calls, arrays, and objects).
- ✅ Focused rest/spread test suites currently pass (329 passed, 0 failed across Validator/Function/Array/Object execution + generator tests).
- ✅ ECMA documentation alignment is complete for Phase 1 scope (`docs/ECMA262/13/Section13_2.json` and `docs/ECMA262/15/Section15_1.json`, with regenerated markdown).

---

## Issue 1: Implement Rest Parameters (`...args`)

**Labels**: `enhancement`, `phase-1`, `critical`, `rest-spread`

**Status**: ✅ Completed

### Description

Implement rest parameters to enable modern JavaScript function signatures with variadic arguments.

### Priority
🔴 **CRITICAL** - Blocks ~90% of modern JavaScript patterns

### Why This Matters
- Essential for modern JS/TS function signatures
- Used extensively in frameworks (React hooks, Vue composables)
- Enables variadic functions and flexible APIs
- Dependency for many modern npm packages

### Usage Examples

```javascript
function sum(...numbers) {
  return numbers.reduce((a, b) => a + b, 0);
}

const combine = (...arrays) => [].concat(...arrays);

// React hook example
function useCustomHook(...dependencies) {
  return useMemo(() => {
    // compute based on dependencies
  }, [...dependencies]);
}
```

### ECMA-262 Reference
- **Sections**: 15.1, 13.2
- **Spec URL**: https://tc39.es/ecma262/#sec-function-definitions

### Implementation Checklist

#### Parser Updates
- [x] Parse `...identifier` in function parameter lists
- [x] Validate rest parameter position (must be last)
- [x] Handle interaction with default parameters
- [x] Update AST nodes for rest parameters

#### Symbol Table / Type Generation
- [x] Track rest parameters in `SymbolTable`
- [x] Generate appropriate field for rest parameter array
- [x] Handle scope binding for rest parameter

#### IL Generation
- [x] Generate IL to collect remaining arguments into array
- [x] Handle case with no remaining arguments (empty array)
- [x] Handle interaction with named parameters
- [x] Emit proper array initialization code

#### Testing
- [x] Add execution tests for basic rest parameters
- [x] Test rest with named parameters: `function f(a, b, ...rest)`
- [x] Test rest-only: `function f(...args)`
- [x] Test empty rest case
- [x] Test nested functions with rest
- [x] Test arrow functions with rest
- [x] Add generator tests (IL snapshots)
- [x] Update test snapshots: `node scripts/updateVerifiedFiles.js`

#### Documentation
- [x] Update `docs/ECMA262/15/Section15_1.json` status
- [x] Regenerate section markdown
- [x] Add usage examples to documentation
- [x] Update `CHANGELOG.md`

### Acceptance Criteria
- [x] Can compile functions with rest parameters
- [x] Rest parameter creates proper array from remaining arguments
- [x] Works with named parameters
- [ ] All existing tests pass
- [x] New tests added and passing
- [x] Documentation updated

### Estimated Effort
**2 weeks** (Phase 1, Weeks 1-2)

### Related Issues
- Depends on: None
- Blocks: #[spread-in-calls], #[spread-in-arrays], #[spread-in-objects]
- Related: Phase 1 implementation

---

## Issue 2: Implement Spread Operator in Function Calls

**Labels**: `enhancement`, `phase-1`, `critical`, `rest-spread`

**Status**: ✅ Completed

### Description

Implement spread operator in function call expressions to enable dynamic argument passing from arrays.

### Priority
🔴 **CRITICAL** - Complements rest parameters, used in nearly every modern codebase

### Why This Matters
- Critical for functional programming patterns
- Enables dynamic argument passing
- Used in nearly every modern JavaScript codebase
- Common pattern in React/Vue/Angular applications

### Usage Examples

```javascript
const max = Math.max(...numbers);
array.push(...items);
func(...args);

// Multiple spreads
func(...args1, middle, ...args2);

// With literals
func(1, 2, ...rest, 9, 10);
```

### ECMA-262 Reference
- **Section**: 13.2
- **Spec URL**: https://tc39.es/ecma262/#sec-argument-lists

### Implementation Checklist

#### Parser Updates
- [x] Parse `...expression` in argument lists
- [x] Handle multiple spreads in single call
- [x] Update AST nodes for spread arguments

#### IL Generation
- [x] Implement array expansion into individual arguments
- [x] Handle multiple spreads in single call
- [x] Generate proper argument list from spread
- [x] Handle interleaving of spread and regular arguments
- [x] Optimize for common cases (single spread)

#### Testing
- [x] Basic spread: `func(...arr)`
- [x] Multiple spreads: `func(...a, ...b)`
- [x] Mixed: `func(1, ...arr, 2)`
- [x] Nested spreads
- [x] Empty array spread
- [x] Test with various array types
- [x] Add generator tests (IL snapshots)
- [x] Update test snapshots

#### Documentation
- [x] Update `docs/ECMA262/13/Section13_2.json` status
- [x] Regenerate section markdown
- [x] Add usage examples
- [x] Update `CHANGELOG.md`

### Acceptance Criteria
- [x] Can compile spread in function calls
- [x] Handles multiple spreads correctly
- [x] Works with mixed arguments
- [ ] All existing tests pass
- [x] New tests added and passing

### Estimated Effort
**2 weeks** (Phase 1, Weeks 3-4)

### Dependencies
- Recommended after: #[rest-parameters] (helps with testing)

---

## Issue 3: Implement Spread in Array Literals

**Labels**: `enhancement`, `phase-1`, `high-priority`, `rest-spread`

**Status**: ✅ Completed

### Description

Complete implementation of spread operator in array literals for array concatenation and cloning.

### Priority
🟡 **HIGH** - Partially implemented, needs completion

### Current Status
Implemented with execution and generator coverage for basic, mixed, multiple, empty, and nested spread patterns.

### Why This Matters
- Array concatenation and cloning
- Common pattern in state management (Redux)
- Functional programming patterns

### Usage Examples

```javascript
const combined = [...arr1, ...arr2];
const clone = [...original];
const extended = [1, 2, ...rest, 9, 10];

// Multiple spreads
const all = [...a, ...b, ...c];

// React state update pattern
const newState = [...state, newItem];
```

### ECMA-262 Reference
- **Section**: 13.2
- **Spec URL**: https://tc39.es/ecma262/#sec-array-initializer

### Implementation Checklist

#### Parser Updates
- [x] Verify parser handles spread in array literals
- [x] Handle multiple spreads in single array
- [x] Update AST validation if needed

#### IL Generation
- [x] Complete IL generation for array spread
- [x] Handle multiple spreads in single array
- [x] Optimize for common patterns
- [x] Handle empty arrays
- [x] Handle mixed literals and spreads

#### Testing
- [x] Basic spread: `[...arr]`
- [x] Multiple spreads: `[...a, ...b]`
- [x] Mixed: `[1, ...arr, 2]`
- [x] Cloning: `[...original]`
- [x] Empty array: `[...empty]`
- [x] Nested arrays
- [x] Add generator tests
- [x] Update test snapshots

#### Documentation
- [x] Update `docs/ECMA262/13/Section13_2.json` status
- [x] Regenerate section markdown
- [x] Update `CHANGELOG.md`

### Acceptance Criteria
- [x] Full spread in array literals support
- [x] Handles multiple spreads
- [x] Works with mixed elements
- [ ] All tests pass

### Estimated Effort
**1 week** (Phase 1, Weeks 3-4, parallel with spread in calls)

### Dependencies
- Can be done in parallel with: #[spread-in-calls]

---

## Issue 4: Implement Spread in Object Literals

**Labels**: `enhancement`, `phase-1`, `high-priority`, `rest-spread`

**Status**: ✅ Completed

### Description

Complete implementation of spread operator in object literals for object merging and shallow cloning.

### Priority
🟡 **HIGH** - Partially implemented, critical for React/state management patterns

### Current Status
Implemented with execution and generator coverage for basic, multiple, clone, empty, symbol-keyed, nested, and non-enumerable filtering semantics.

### Why This Matters
- Object merging (common in React props, state updates)
- Shallow cloning pattern
- Essential for Redux and state management
- Modern JavaScript idiom

### Usage Examples

```javascript
const merged = { ...obj1, ...obj2 };
const clone = { ...original };
const withDefaults = { ...defaults, ...userOptions };

// React state update (critical pattern)
const updated = { ...state, count: state.count + 1 };

// Multiple spreads
const combined = { ...a, ...b, ...c };

// Override properties
const modified = { ...original, name: 'new name' };
```

### ECMA-262 Reference
- **Section**: 13.2
- **Spec URL**: https://tc39.es/ecma262/#sec-object-initializer

### Implementation Checklist

#### Parser Updates
- [x] Verify parser handles spread in object literals
- [x] Handle multiple spreads in single object
- [x] Handle property override semantics

#### IL Generation
- [x] Complete IL generation for object spread
- [x] Implement property copying IL
- [x] Handle multiple spreads (later wins)
- [x] Handle nested spreads
- [x] Handle property override semantics
- [x] Optimize common patterns

#### Testing
- [x] Basic spread: `{ ...obj }`
- [x] Multiple spreads: `{ ...a, ...b }`
- [x] Override: `{ ...obj, prop: 'new' }`
- [x] Cloning: `{ ...original }`
- [x] Empty object: `{ ...empty }`
- [x] Symbol properties
- [x] Nested objects
- [x] Add generator tests
- [x] Update test snapshots

#### Documentation
- [x] Update `docs/ECMA262/13/Section13_2.json` status
- [x] Regenerate section markdown
- [x] Update `CHANGELOG.md`

### Acceptance Criteria
- [x] Full spread in object literals support
- [x] Handles multiple spreads with correct semantics
- [x] Property override works correctly
- [ ] All tests pass

### Estimated Effort
**2 weeks** (Phase 1, Weeks 5-6)

### Dependencies
- Recommended after: #[spread-in-calls], #[spread-in-arrays] (build confidence)

---

## Phase 1 Success Criteria

When all 4 issues are complete:

### Functional Success
- [x] All rest/spread patterns compile and run correctly
- [ ] Can compile modern React components with hooks
- [ ] Can compile functional programming patterns
- [ ] Can compile state management code (Redux patterns)

### Test Success
- [ ] All existing tests continue to pass
- [x] New execution tests for all patterns added
- [x] Generator tests (IL snapshots) added
- [x] Test coverage for edge cases

### Documentation Success
- [x] All relevant ECMA-262 section docs updated
- [x] CHANGELOG.md updated
- [x] Usage examples documented
- [x] Known limitations documented

### Example Code That Should Work

```javascript
// Rest parameters
function sum(...numbers) {
  return numbers.reduce((a, b) => a + b, 0);
}

// Spread in calls
const max = Math.max(...[1, 2, 3]);

// Spread in arrays
const combined = [...arr1, ...arr2];

// Spread in objects (React pattern)
const newState = { ...state, updated: true };

// Combined usage (React hook pattern)
function useCustomHook(...deps) {
  return useMemo(() => compute(...deps), [...deps]);
}
```

---

## Notes

- These issues track **Phase 1** of the ECMA-262 Feature Implementation Roadmap
- See `docs/FeatureImplementationRoadmap.md` for complete context
- See `docs/ECMA262FeaturePriority.md` for executive summary
- Total estimated effort: **4-6 weeks**
- Impact: Unlocks ~60% of modern JavaScript patterns

---

*Generated from ECMA-262 Feature Implementation Analysis*  
*Last updated: 2026-02-18*
