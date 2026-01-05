## Summary

Add for loop support and compound operators to the HIRLIRIL pipeline.

## Changes

### New Features
- **For loop support**: Added HIRForStatement node and full parsing/lowering pipeline for or loops
- **Compound operators**: Added all missing compound assignment operators to TryLowerCompoundOperation:
  - /=, %=, **= (arithmetic)
  - &=, |=, ^= (bitwise)
  - <<=, >>=, >>>= (shift)

### Files Added
- Js2IL/IR/HIR/HIRForStatement.cs - HIR node for for loops

### Files Modified
- Js2IL/IR/HIR/HIRBuilder.cs - Added ForStatement parsing with scope handling
- Js2IL/IR/LIR/HIRToLIRLower.cs - Added for loop lowering and compound operators
- Test files with ssertOnIRPipelineFailure: true for applicable tests
- Updated 17 generator snapshots

## Testing
- All 828 tests pass (820 passed, 8 skipped)
- Enabled IR pipeline assertions for:
  - 11 compound assignment tests
  - 27 binary operator tests  
  - 1 for loop test