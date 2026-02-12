# Phase 1 Implementation Guide

> **Status**: Ready for Implementation  
> **Created**: 2026-02-12  
> **Phase Duration**: 4-6 weeks  
> **Goal**: Enable modern function signatures and array/object manipulation

## What is Phase 1?

Phase 1 is the first phase of the ECMA-262 Feature Implementation Roadmap for JS2IL. It focuses on implementing **Rest/Spread** functionality, which is foundational for modern JavaScript.

### Why Phase 1 Matters

Rest and spread operators are:
- 🔴 **CRITICAL** - Used in ~90% of modern JavaScript codebases
- 🚀 **Game Changing** - Enables compilation of React, Vue, Angular, and modern npm packages
- 📈 **High Impact** - Unlocks ~60% of modern JavaScript patterns

### Phase 1 Features

1. **Rest Parameters** (`...args`) - CRITICAL
2. **Spread in Function Calls** (`func(...arr)`) - CRITICAL
3. **Spread in Array Literals** (`[...arr1, ...arr2]`) - HIGH PRIORITY
4. **Spread in Object Literals** (`{...obj1, ...obj2}`) - HIGH PRIORITY

---

## Creating the GitHub Issues

Four GitHub issues need to be created to track Phase 1 implementation. The issue bodies have been extracted and are ready in `/tmp/issue-*-body.md`.

### Issue 1: Implement Rest Parameters (`...args`)

**Title**: `Implement Rest Parameters (\`...\`args)`  
**Labels**: `enhancement`, `phase-1`, `critical`, `rest-spread`  
**Body**: See `/tmp/issue-1-body.md`

**Create manually**:
```bash
gh issue create \
  --repo tomacox74/js2il \
  --title "Implement Rest Parameters (\`...\`args)" \
  --label "enhancement,phase-1,critical,rest-spread" \
  --body-file /tmp/issue-1-body.md
```

---

### Issue 2: Implement Spread Operator in Function Calls

**Title**: `Implement Spread Operator in Function Calls`  
**Labels**: `enhancement`, `phase-1`, `critical`, `rest-spread`  
**Body**: See `/tmp/issue-2-body.md`

**Create manually**:
```bash
gh issue create \
  --repo tomacox74/js2il \
  --title "Implement Spread Operator in Function Calls" \
  --label "enhancement,phase-1,critical,rest-spread" \
  --body-file /tmp/issue-2-body.md
```

---

### Issue 3: Implement Spread in Array Literals

**Title**: `Implement Spread in Array Literals`  
**Labels**: `enhancement`, `phase-1`, `high-priority`, `rest-spread`  
**Body**: See `/tmp/issue-3-body.md`

**Create manually**:
```bash
gh issue create \
  --repo tomacox74/js2il \
  --title "Implement Spread in Array Literals" \
  --label "enhancement,phase-1,high-priority,rest-spread" \
  --body-file /tmp/issue-3-body.md
```

---

### Issue 4: Implement Spread in Object Literals

**Title**: `Implement Spread in Object Literals`  
**Labels**: `enhancement`, `phase-1`, `high-priority`, `rest-spread`  
**Body**: See `/tmp/issue-4-body.md`

**Create manually**:
```bash
gh issue create \
  --repo tomacox74/js2il \
  --title "Implement Spread in Object Literals" \
  --label "enhancement,phase-1,high-priority,rest-spread" \
  --body-file /tmp/issue-4-body.md
```

---

## Bulk Issue Creation Script

A convenience script is available at `/tmp/create-phase1-issues.sh`:

```bash
# Run from the repository root after authenticating with gh
bash /tmp/create-phase1-issues.sh
```

---

## Implementation Order

1. **Start with Issue 1** (Rest Parameters) - It's foundational
2. **Then Issue 2** (Spread in Calls) - Complements rest parameters
3. **Issues 3 & 4** can be done in parallel or sequentially

---

## Success Criteria

Phase 1 is complete when:

### Functional Success
- ✅ All rest/spread patterns compile and run correctly
- ✅ Can compile modern React components with hooks
- ✅ Can compile functional programming patterns
- ✅ Can compile state management code (Redux patterns)

### Test Success
- ✅ All existing tests continue to pass
- ✅ New execution tests for all patterns added
- ✅ Generator tests (IL snapshots) added
- ✅ Test coverage for edge cases

### Documentation Success
- ✅ All relevant ECMA-262 section docs updated
- ✅ CHANGELOG.md updated
- ✅ Usage examples documented
- ✅ Known limitations documented

---

## Example Code That Should Work After Phase 1

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

## Next Steps

1. **Create the GitHub issues** (see commands above)
2. **Review the tracking issues** in `docs/tracking-issues/Phase1-TrackingIssues.md`
3. **Review the implementation roadmap** in `docs/FeatureImplementationRoadmap.md`
4. **Begin implementation** starting with Issue 1 (Rest Parameters)
5. **Follow the workflow** in `docs/tracking-issues/README.md`

---

## Related Documentation

- **Detailed Issue Specs**: `docs/tracking-issues/Phase1-TrackingIssues.md`
- **Feature Roadmap**: `docs/FeatureImplementationRoadmap.md`
- **Feature Priority**: `docs/ECMA262FeaturePriority.md`
- **Feature Status**: `docs/ECMA262FeatureStatus.md`
- **Implementation Workflow**: `docs/tracking-issues/README.md`

---

## Notes

- These tracking issues are part of the ECMA-262 Feature Implementation Roadmap
- Phase 1 focuses on Rest/Spread because it's foundational for modern JavaScript
- Total estimated effort: **4-6 weeks**
- Impact: Unlocks **~60%** of modern JavaScript patterns
- After Phase 1, we can compile most React, Vue, and Angular applications

---

*Generated: 2026-02-12*  
*For: JS2IL Phase 1 Implementation*
