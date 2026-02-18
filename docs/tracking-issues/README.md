# Tracking Issues and Triage

This directory contains planning/tracking artifacts used to prioritize ECMA-262, Node.js compatibility, and issue-driven reliability work.

## Current Source of Truth (Active)

**Primary planning doc**: [TriageScoreboard.md](./TriageScoreboard.md)

- Uses the current implementation split: **70% features / 30% docs+issues**
- Prioritizes **Node compatibility** as primary implementation lane
- Includes mandatory feature PR docs-sync gate
- Defines weekly triage cadence and ranking criteria

---

## Historical Phase Plans (Context)

The phase-based ECMA planning docs remain useful context but are **not** the active source of truth for current sequencing:

- [Phase1-TrackingIssues.md](./Phase1-TrackingIssues.md)

Use them as background/reference only when drafting or updating issue scopes.

## Quick Start

### Creating or Updating GitHub Issues

The tracking issue documents are formatted to be directly copied into GitHub issues. Each issue includes:
- Detailed description and context
- Implementation checklist
- Acceptance criteria
- Estimated effort
- Dependencies

Before creating new issues, rank work in [TriageScoreboard.md](./TriageScoreboard.md) and verify no duplicate issue exists.

---

## How to Create Issues

> Follow `.github/copilot-instructions.md` guidance: use `issue-body.md` for complex issue bodies and the dedupe flow when possible.

### Option 1: Manual Creation (Recommended if no GitHub CLI)

1. Open [Phase1-TrackingIssues.md](./Phase1-TrackingIssues.md)
2. Copy the content for each issue (from "## Issue N" to the next issue or section)
3. Create a new GitHub issue
4. Paste the content as the issue body
5. Add labels as specified
6. Set the title as specified

### Option 2: Bulk Creation Script

Create all Phase 1 issues at once:

```bash
cd /home/runner/work/js2il/js2il

# Create Issue 1: Rest Parameters
gh issue create \
  --repo tomacox74/js2il \
  --title "Implement Rest Parameters (...args)" \
  --label "enhancement,phase-1,critical,rest-spread" \
  --body "$(awk '/^## Issue 1:/,/^## Issue 2:/' docs/tracking-issues/Phase1-TrackingIssues.md | head -n -2)"

# Create Issue 2: Spread in Function Calls
gh issue create \
  --repo tomacox74/js2il \
  --title "Implement Spread Operator in Function Calls" \
  --label "enhancement,phase-1,critical,rest-spread" \
  --body "$(awk '/^## Issue 2:/,/^## Issue 3:/' docs/tracking-issues/Phase1-TrackingIssues.md | head -n -2)"

# Create Issue 3: Spread in Array Literals
gh issue create \
  --repo tomacox74/js2il \
  --title "Implement Spread in Array Literals" \
  --label "enhancement,phase-1,high-priority,rest-spread" \
  --body "$(awk '/^## Issue 3:/,/^## Issue 4:/' docs/tracking-issues/Phase1-TrackingIssues.md | head -n -2)"

# Create Issue 4: Spread in Object Literals
gh issue create \
  --repo tomacox74/js2il \
  --title "Implement Spread in Object Literals" \
  --label "enhancement,phase-1,high-priority,rest-spread" \
  --body "$(awk '/^## Issue 4:/,/^## Phase 1 Success/' docs/tracking-issues/Phase1-TrackingIssues.md | head -n -2)"
```

---

## Issue Structure

Each tracking issue includes:

### Header
- **Title**: Clear, concise feature description
- **Labels**: Priority, phase, and feature category
- **Priority**: Visual indicator (🔴 CRITICAL, 🟡 HIGH, 🟢 MEDIUM, 🔵 LOW)

### Content Sections
1. **Description**: What the feature does
2. **Priority & Why This Matters**: Business justification
3. **Usage Examples**: Code samples showing the feature in action
4. **ECMA-262 Reference**: Spec sections and links
5. **Implementation Checklist**: Broken down by area (Parser, IL Gen, Testing, Docs)
6. **Acceptance Criteria**: Definition of done
7. **Estimated Effort**: Time estimate
8. **Dependencies**: Related issues

---

## Implementation Workflow

For each issue:

1. **Pre-Implementation**
   - Review the tracking issue
   - Read related ECMA-262 spec sections
   - Understand dependencies
   - Set up test infrastructure

2. **Implementation**
   - Follow the implementation checklist
   - Make small, incremental changes
   - Test frequently
   - Update documentation as you go

3. **Testing**
   - Add execution tests (compile + run + verify output)
   - Add generator tests (IL snapshot tests)
   - Update snapshots: `node scripts/updateVerifiedFiles.js`
   - Ensure all existing tests pass

4. **Documentation**
   - Update relevant `docs/ECMA262/*/Section*.json` files
   - Regenerate markdown: `node scripts/ECMA262/generateEcma262SectionMarkdown.js --section X.Y`
   - Update `CHANGELOG.md`
   - Add usage examples

5. **Completion**
   - All checklist items complete
   - All acceptance criteria met
   - Code review passed
   - Tests passing
   - Documentation updated

### Mandatory Docs-Sync Gate for Feature PRs

Feature PRs are not ready to merge until docs are synchronized:

1. Update relevant source JSON docs (`docs/ECMA262/**/Section*.json` and/or `docs/nodejs/*.json`)
2. Regenerate markdown/index artifacts
3. Ensure status summaries reflect shipped behavior
4. Include changelog entry when behavior is user-visible

---

## Phase Tracking (Historical Snapshot)

### Phase 1: Rest/Spread Foundation
- **Status**: Historical planning context
- **Duration**: 4-6 weeks
- **Issues**: 4 tracking issues created
- **Impact**: 60% modern JavaScript coverage

### Phase 2: Object Utilities
- **Status**: Historical planning context
- **Duration**: 2-3 weeks
- **Impact**: 85% coverage

### Phase 3: Array Methods
- **Status**: Historical planning context
- **Duration**: 3-4 weeks
- **Impact**: 95% coverage

### Phase 4: Advanced Features
- **Status**: Historical planning context
- **Duration**: 4-6 weeks
- **Impact**: ~100% coverage

---

## Related Documentation

- **[TriageScoreboard.md](./TriageScoreboard.md)**: Active prioritization and weekly execution board
- **[NodeGapPopularityBacklog_2026-02-17.md](./NodeGapPopularityBacklog_2026-02-17.md)**: Persisted holistic gap analysis + popularity-weighted Node backlog snapshot
- **[FeatureImplementationRoadmap.md](../FeatureImplementationRoadmap.md)**: Complete technical roadmap
- **[ECMA262FeaturePriority.md](../ECMA262FeaturePriority.md)**: Executive summary
- **[ECMA262FeatureStatus.md](../ECMA262FeatureStatus.md)**: Quick reference
- **[ECMA262_README.md](../ECMA262_README.md)**: Documentation index

---

## Tips for Implementers

### Getting Started
1. Start with Issue 1 (Rest Parameters) - it's foundational
2. Set up your development environment
3. Run existing tests to establish baseline
4. Review the copilot instructions: `.github/copilot-instructions.md`

### During Implementation
- Make small, focused commits
- Test early and often
- Use the existing code patterns in the codebase
- Don't hesitate to ask questions

### Testing Best Practices
- Write tests before implementation (TDD)
- Test both positive and negative cases
- Test edge cases (empty arrays, null, undefined, etc.)
- Ensure existing tests still pass

### Common Pitfalls to Avoid
- Don't skip the symbol table updates
- Don't forget to handle edge cases
- Don't skip documentation updates
- Don't leave TODOs in production code

---

## Questions?

If you have questions about:
- **What to implement now**: See [TriageScoreboard.md](./TriageScoreboard.md)
- **What was originally planned**: See the phase tracking docs
- **How to implement**: See `docs/FeatureImplementationRoadmap.md`
- **Why we're implementing**: See `docs/ECMA262FeaturePriority.md`
- **Current status**: See `docs/ECMA262FeatureStatus.md`

---

*This directory supports ongoing ECMA-262 + Node.js compatibility triage*  
*Last updated: 2026-02-17*
