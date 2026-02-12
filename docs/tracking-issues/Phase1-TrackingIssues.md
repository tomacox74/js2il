# Phase 1: Rest/Spread Foundation - Tracking Issues

> **Created**: 2026-02-12  
> **Phase Duration**: 4-6 weeks  
> **Goal**: Enable modern function signatures and array/object manipulation

This document contains the tracking issues for Phase 1 features. These issues are ready to be created in GitHub.

---

## Issue 1: Implement Rest Parameters (`...args`)

**Labels**: `enhancement`, `phase-1`, `critical`, `rest-spread`

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
- [ ] Parse `...identifier` in function parameter lists
- [ ] Validate rest parameter position (must be last)
- [ ] Handle interaction with default parameters
- [ ] Update AST nodes for rest parameters

#### Symbol Table / Type Generation
- [ ] Track rest parameters in `SymbolTable`
- [ ] Generate appropriate field for rest parameter array
- [ ] Handle scope binding for rest parameter

#### IL Generation
- [ ] Generate IL to collect remaining arguments into array
- [ ] Handle case with no remaining arguments (empty array)
- [ ] Handle interaction with named parameters
- [ ] Emit proper array initialization code

#### Testing
- [ ] Add execution tests for basic rest parameters
- [ ] Test rest with named parameters: `function f(a, b, ...rest)`
- [ ] Test rest-only: `function f(...args)`
- [ ] Test empty rest case
- [ ] Test nested functions with rest
- [ ] Test arrow functions with rest
- [ ] Add generator tests (IL snapshots)
- [ ] Update test snapshots: `node scripts/updateVerifiedFiles.js`

#### Documentation
- [ ] Update `docs/ECMA262/15/Section15_1.json` status
- [ ] Regenerate section markdown
- [ ] Add usage examples to documentation
- [ ] Update `CHANGELOG.md`

### Acceptance Criteria
- [ ] Can compile functions with rest parameters
- [ ] Rest parameter creates proper array from remaining arguments
- [ ] Works with named parameters
- [ ] All existing tests pass
- [ ] New tests added and passing
- [ ] Documentation updated

### Estimated Effort
**2 weeks** (Phase 1, Weeks 1-2)

### Related Issues
- Depends on: None
- Blocks: #[spread-in-calls], #[spread-in-arrays], #[spread-in-objects]
- Related: Phase 1 implementation

---

## Issue 2: Implement Spread Operator in Function Calls

**Labels**: `enhancement`, `phase-1`, `critical`, `rest-spread`

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
- [ ] Parse `...expression` in argument lists
- [ ] Handle multiple spreads in single call
- [ ] Update AST nodes for spread arguments

#### IL Generation
- [ ] Implement array expansion into individual arguments
- [ ] Handle multiple spreads in single call
- [ ] Generate proper argument list from spread
- [ ] Handle interleaving of spread and regular arguments
- [ ] Optimize for common cases (single spread)

#### Testing
- [ ] Basic spread: `func(...arr)`
- [ ] Multiple spreads: `func(...a, ...b)`
- [ ] Mixed: `func(1, ...arr, 2)`
- [ ] Nested spreads
- [ ] Empty array spread
- [ ] Test with various array types
- [ ] Add generator tests (IL snapshots)
- [ ] Update test snapshots

#### Documentation
- [ ] Update `docs/ECMA262/13/Section13_2.json` status
- [ ] Regenerate section markdown
- [ ] Add usage examples
- [ ] Update `CHANGELOG.md`

### Acceptance Criteria
- [ ] Can compile spread in function calls
- [ ] Handles multiple spreads correctly
- [ ] Works with mixed arguments
- [ ] All existing tests pass
- [ ] New tests added and passing

### Estimated Effort
**2 weeks** (Phase 1, Weeks 3-4)

### Dependencies
- Recommended after: #[rest-parameters] (helps with testing)

---

## Issue 3: Implement Spread in Array Literals

**Labels**: `enhancement`, `phase-1`, `high-priority`, `rest-spread`

### Description

Complete implementation of spread operator in array literals for array concatenation and cloning.

### Priority
🟡 **HIGH** - Partially implemented, needs completion

### Current Status
Partially implemented - some spread in arrays may work, but needs full support and testing.

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
- [ ] Verify parser handles spread in array literals
- [ ] Handle multiple spreads in single array
- [ ] Update AST validation if needed

#### IL Generation
- [ ] Complete IL generation for array spread
- [ ] Handle multiple spreads in single array
- [ ] Optimize for common patterns
- [ ] Handle empty arrays
- [ ] Handle mixed literals and spreads

#### Testing
- [ ] Basic spread: `[...arr]`
- [ ] Multiple spreads: `[...a, ...b]`
- [ ] Mixed: `[1, ...arr, 2]`
- [ ] Cloning: `[...original]`
- [ ] Empty array: `[...empty]`
- [ ] Nested arrays
- [ ] Add generator tests
- [ ] Update test snapshots

#### Documentation
- [ ] Update `docs/ECMA262/13/Section13_2.json` status
- [ ] Regenerate section markdown
- [ ] Update `CHANGELOG.md`

### Acceptance Criteria
- [ ] Full spread in array literals support
- [ ] Handles multiple spreads
- [ ] Works with mixed elements
- [ ] All tests pass

### Estimated Effort
**1 week** (Phase 1, Weeks 3-4, parallel with spread in calls)

### Dependencies
- Can be done in parallel with: #[spread-in-calls]

---

## Issue 4: Implement Spread in Object Literals

**Labels**: `enhancement`, `phase-1`, `high-priority`, `rest-spread`

### Description

Complete implementation of spread operator in object literals for object merging and shallow cloning.

### Priority
🟡 **HIGH** - Partially implemented, critical for React/state management patterns

### Current Status
Partially implemented - some spread in objects may work, but needs full support and testing.

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
- [ ] Verify parser handles spread in object literals
- [ ] Handle multiple spreads in single object
- [ ] Handle property override semantics

#### IL Generation
- [ ] Complete IL generation for object spread
- [ ] Implement property copying IL
- [ ] Handle multiple spreads (later wins)
- [ ] Handle nested spreads
- [ ] Handle property override semantics
- [ ] Optimize common patterns

#### Testing
- [ ] Basic spread: `{ ...obj }`
- [ ] Multiple spreads: `{ ...a, ...b }`
- [ ] Override: `{ ...obj, prop: 'new' }`
- [ ] Cloning: `{ ...original }`
- [ ] Empty object: `{ ...empty }`
- [ ] Symbol properties
- [ ] Nested objects
- [ ] Add generator tests
- [ ] Update test snapshots

#### Documentation
- [ ] Update `docs/ECMA262/13/Section13_2.json` status
- [ ] Regenerate section markdown
- [ ] Update `CHANGELOG.md`

### Acceptance Criteria
- [ ] Full spread in object literals support
- [ ] Handles multiple spreads with correct semantics
- [ ] Property override works correctly
- [ ] All tests pass

### Estimated Effort
**2 weeks** (Phase 1, Weeks 5-6)

### Dependencies
- Recommended after: #[spread-in-calls], #[spread-in-arrays] (build confidence)

---

## Phase 1 Success Criteria

When all 4 issues are complete:

### Functional Success
- [ ] All rest/spread patterns compile and run correctly
- [ ] Can compile modern React components with hooks
- [ ] Can compile functional programming patterns
- [ ] Can compile state management code (Redux patterns)

### Test Success
- [ ] All existing tests continue to pass
- [ ] New execution tests for all patterns added
- [ ] Generator tests (IL snapshots) added
- [ ] Test coverage for edge cases

### Documentation Success
- [ ] All relevant ECMA-262 section docs updated
- [ ] CHANGELOG.md updated
- [ ] Usage examples documented
- [ ] Known limitations documented

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
*Last updated: 2026-02-12*
