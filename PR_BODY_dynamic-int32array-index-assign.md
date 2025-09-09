### Summary
Adds dynamic Int32Array indexed element assignment emission using JavaScriptRuntime.Object.AssignItem runtime fallback (inside class methods and general expressions) and fixes prior stack imbalance (AssignItem result + ldnull) by discarding the helper return only in statement contexts.

### Changes
- Emitter: dynamic Int32Array target[index] = value path; evaluates receiver once, calls AssignItem(receiver,index,value), leaves value for expression contexts.
- Stack handling: clarifies statement vs expression behavior; previous code left value + pushed ldnull.
- Tests: Adds execution snapshot BeanCounter_Class_Index_Assign (42 output) plus generator inspection (internal review).
- Changelog: Updated Unreleased section with feature + IL fix note.

### Motivation
Needed for upcoming compound operations (e.g. |=) and for parity with simple identifier assignments on typed arrays.

### Follow-ups (not in this PR)
- Compound element assignments (|=, +=, etc.) for Int32Array.
- Remove internal Pop once generic statement Pop path uniformly applies (if refactor planned).
- Broader typed array family support (Uint8Array, Float64Array, etc.).

### Risk / Impact
Low. Affects only new dynamic indexed assignment fallback; existing fast path for known Int32Array receivers unchanged. Tests cover new behavior.

### Checklist
- [x] Added execution test & snapshot
- [x] Updated CHANGELOG
- [x] Branch created (feature/dynamic-int32array-index-assign)

---
