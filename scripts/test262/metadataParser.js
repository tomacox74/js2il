#!/usr/bin/env node
'use strict';

const fs = require('node:fs');

const DEFAULT_HARNESS_INCLUDES = ['assert.js', 'sta.js'];
const ASYNC_HARNESS_INCLUDE = 'doneprintHandle.js';
const KNOWN_FRONTMATTER_KEYS = [
  'description',
  'info',
  'esid',
  'es5id',
  'es6id',
  'author',
  'includes',
  'flags',
  'features',
  'locale',
  'defines',
  'negative',
];
const KNOWN_FLAG_VALUES = [
  'onlyStrict',
  'noStrict',
  'module',
  'raw',
  'async',
  'generated',
  'CanBlockIsFalse',
  'CanBlockIsTrue',
  'non-deterministic',
];

function normalizeLineEndings(text) {
  return String(text).replace(/\r\n/g, '\n').replace(/\r/g, '\n');
}

function countIndent(rawLine) {
  let count = 0;
  while (count < rawLine.length) {
    const ch = rawLine[count];
    if (ch !== ' ' && ch !== '\t') {
      break;
    }
    count++;
  }
  return count;
}

function unquote(value) {
  if (value.length >= 2) {
    const first = value[0];
    const last = value[value.length - 1];
    if ((first === '"' && last === '"') || (first === '\'' && last === '\'')) {
      return value.substring(1, value.length - 1);
    }
  }
  return value;
}

function parseInlineList(text) {
  const inner = text.substring(1, text.length - 1).trim();
  if (inner === '') {
    return [];
  }

  const parts = inner.split(',');
  const values = [];
  for (let i = 0; i < parts.length; i++) {
    const part = parts[i].trim();
    if (part !== '') {
      values.push(unquote(part));
    }
  }

  return values;
}

function parseInlineValue(text) {
  const trimmed = text.trim();
  if (trimmed.startsWith('[') && trimmed.endsWith(']')) {
    return parseInlineList(trimmed);
  }

  return unquote(trimmed);
}

function foldBlockLines(lines) {
  const parts = [];
  let pendingBreaks = 0;

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    if (line === '') {
      pendingBreaks++;
      continue;
    }

    if (parts.length === 0) {
      parts.push(line);
    } else if (pendingBreaks > 0) {
      parts.push('\n'.repeat(pendingBreaks + 1) + line);
    } else {
      parts.push(' ' + line);
    }

    pendingBreaks = 0;
  }

  return parts.join('');
}

function parseBlockScalar(lines, state, parentIndent, style) {
  const collected = [];
  let blockIndent = -1;

  while (state.index < lines.length) {
    const rawLine = lines[state.index];
    if (rawLine.trim() === '') {
      collected.push('');
      state.index++;
      continue;
    }

    const indent = countIndent(rawLine);
    if (indent <= parentIndent) {
      break;
    }

    if (blockIndent < 0) {
      blockIndent = indent;
    }

    collected.push(rawLine.substring(blockIndent));
    state.index++;
  }

  return style === '>' ? foldBlockLines(collected) : collected.join('\n');
}

function parseIndentedValue(lines, state, parentIndent) {
  let probe = state.index;
  while (probe < lines.length && lines[probe].trim() === '') {
    probe++;
  }

  if (probe >= lines.length) {
    state.index = probe;
    return '';
  }

  const nextIndent = countIndent(lines[probe]);
  if (nextIndent <= parentIndent) {
    state.index = probe;
    return '';
  }

  state.index = probe;
  return parseYamlSubsetLines(lines, state, nextIndent);
}

function parseYamlSubsetLines(lines, state, baseIndent) {
  const result = {};

  while (state.index < lines.length) {
    const rawLine = lines[state.index];
    if (rawLine.trim() === '') {
      state.index++;
      continue;
    }

    const indent = countIndent(rawLine);
    if (indent < baseIndent) {
      break;
    }

    if (indent > baseIndent) {
      throw new Error(`Unexpected indentation at frontmatter line ${state.index + 1}.`);
    }

    const line = rawLine.substring(indent);
    const match = /^([A-Za-z0-9_-]+):(.*)$/.exec(line);
    if (!match) {
      throw new Error(`Invalid frontmatter line ${state.index + 1}: ${line}`);
    }

    const key = match[1];
    const remainder = match[2].trim();
    state.index++;

    let value;
    if (remainder === '|' || remainder === '>') {
      value = parseBlockScalar(lines, state, baseIndent, remainder);
    } else if (remainder === '') {
      value = parseIndentedValue(lines, state, baseIndent);
    } else {
      value = parseInlineValue(remainder);
    }

    result[key] = value;
  }

  return result;
}

function parseTest262Frontmatter(frontmatterText) {
  const lines = normalizeLineEndings(frontmatterText).split('\n');
  return parseYamlSubsetLines(lines, { index: 0 }, 0);
}

function extractTest262Frontmatter(sourceText) {
  const normalized = normalizeLineEndings(sourceText);
  const start = normalized.indexOf('/*---');
  if (start < 0) {
    return null;
  }

  const end = normalized.indexOf('---*/', start + 5);
  if (end < 0) {
    throw new Error('Unterminated test262 frontmatter block.');
  }

  let frontmatter = normalized.substring(start + 5, end);
  if (frontmatter.startsWith('\n')) {
    frontmatter = frontmatter.substring(1);
  }

  return frontmatter;
}

function normalizePath(filePath) {
  return (filePath || '').replace(/\\/g, '/');
}

function getFileName(filePath) {
  if (!filePath) {
    return '';
  }

  const slashIndex = filePath.lastIndexOf('/');
  return slashIndex >= 0 ? filePath.substring(slashIndex + 1) : filePath;
}

function contains(values, candidate) {
  return values.indexOf(candidate) >= 0;
}

function pushIssue(target, code, source, reason) {
  target.push({
    code,
    source,
    reason,
  });
}

function toStringArray(value, source, issues) {
  if (value === undefined || value === null || value === '') {
    return [];
  }

  if (!Array.isArray(value)) {
    pushIssue(issues, 'invalid-array', source, `Expected an array for '${source}'.`);
    return [];
  }

  const result = [];
  for (let i = 0; i < value.length; i++) {
    const entry = value[i];
    if (typeof entry !== 'string') {
      pushIssue(issues, 'invalid-array-entry', source, `Expected string entries for '${source}'.`);
      continue;
    }

    result.push(entry);
  }

  return result;
}

function toNullableString(value, source, issues) {
  if (value === undefined || value === null || value === '') {
    return null;
  }

  if (typeof value !== 'string') {
    pushIssue(issues, 'invalid-string', source, `Expected a string for '${source}'.`);
    return null;
  }

  return value;
}

function normalizeNegative(rawNegative, issues) {
  if (rawNegative === undefined || rawNegative === null || rawNegative === '') {
    return null;
  }

  if (!rawNegative || typeof rawNegative !== 'object' || Array.isArray(rawNegative)) {
    pushIssue(issues, 'invalid-negative', 'negative', 'Expected an object for the negative frontmatter value.');
    return null;
  }

  const phase = toNullableString(rawNegative.phase, 'negative.phase', issues);
  const type = toNullableString(rawNegative.type, 'negative.type', issues);
  const allowedPhases = ['parse', 'resolution', 'runtime'];

  if (phase && allowedPhases.indexOf(phase) < 0) {
    pushIssue(issues, 'unknown-negative-phase', 'negative.phase', `Unsupported negative phase '${phase}'.`);
  }

  const keys = Object.keys(rawNegative);
  for (let i = 0; i < keys.length; i++) {
    const key = keys[i];
    if (key !== 'phase' && key !== 'type') {
      pushIssue(issues, 'unknown-negative-key', `negative.${key}`, `Unsupported negative metadata key '${key}'.`);
    }
  }

  if (!phase || !type) {
    return null;
  }

  return { phase, type };
}

function buildExecutionMetadata(flags, includes, fileName) {
  const isModule = contains(flags, 'module');
  const isRaw = contains(flags, 'raw');
  const isAsync = contains(flags, 'async');
  const isGenerated = contains(flags, 'generated');
  const isNonDeterministic = contains(flags, 'non-deterministic');
  const isFixture = fileName.indexOf('_FIXTURE') >= 0;

  const strictnessFlags = [];
  const strictnessOrder = ['onlyStrict', 'noStrict', 'module', 'raw'];
  for (let i = 0; i < strictnessOrder.length; i++) {
    const flag = strictnessOrder[i];
    if (contains(flags, flag)) {
      strictnessFlags.push(flag);
    }
  }

  let strictMode = 'strict-and-non-strict';
  if (strictnessFlags.length === 1) {
    const flag = strictnessFlags[0];
    if (flag === 'onlyStrict') {
      strictMode = 'strict-only';
    } else if (flag === 'noStrict' || flag === 'raw') {
      strictMode = 'non-strict-only';
    } else if (flag === 'module') {
      strictMode = 'module';
    }
  } else if (strictnessFlags.length > 1) {
    strictMode = 'conflicting-flags';
  }

  const defaultHarnessIncludes = isRaw ? [] : DEFAULT_HARNESS_INCLUDES.slice();
  const harnessIncludes = defaultHarnessIncludes.slice();

  if (isAsync && !isRaw && !contains(harnessIncludes, ASYNC_HARNESS_INCLUDE)) {
    harnessIncludes.push(ASYNC_HARNESS_INCLUDE);
  }

  for (let i = 0; i < includes.length; i++) {
    if (!contains(harnessIncludes, includes[i])) {
      harnessIncludes.push(includes[i]);
    }
  }

  const hasAgentInclude = contains(includes, 'agent.js');
  const canBlock = contains(flags, 'CanBlockIsFalse')
    ? 'false'
    : contains(flags, 'CanBlockIsTrue')
      ? 'true'
      : 'unspecified';

  return {
    sourceType: isModule ? 'module' : 'script',
    strictMode,
    defaultHarnessIncludes,
    harnessIncludes,
    requiresDefaultHarness: defaultHarnessIncludes.length > 0,
    requiresAsyncHarness: isAsync || contains(includes, ASYNC_HARNESS_INCLUDE),
    requiresAgent: hasAgentInclude || canBlock !== 'unspecified',
    canBlock,
    isModule,
    isRaw,
    isAsync,
    isGenerated,
    isNonDeterministic,
    isFixture,
  };
}

function buildMvpBlockers(filePath, negative, execution) {
  const blockers = [];

  if (execution.isFixture) {
    pushIssue(blockers, 'fixture-file', filePath || '<anonymous>', 'Files whose names include `_FIXTURE` are support files, not standalone MVP tests.');
  }

  if (execution.isModule) {
    pushIssue(blockers, 'module-flag', 'flags:module', 'Module tests are outside the plain synchronous script MVP.');
  }

  if (execution.isRaw) {
    pushIssue(blockers, 'raw-flag', 'flags:raw', 'Raw tests bypass default harness injection and are outside the initial MVP.');
  }

  if (execution.requiresAsyncHarness) {
    pushIssue(blockers, 'async-requirement', 'flags/includes', 'Async tests and async harness requirements are outside the initial MVP.');
  }

  if (execution.requiresAgent) {
    pushIssue(blockers, 'agent-requirement', 'flags/includes', 'Agent-sensitive tests are outside the initial MVP.');
  }

  if (execution.canBlock !== 'unspecified') {
    pushIssue(blockers, 'can-block-requirement', `flags:CanBlockIs${execution.canBlock === 'true' ? 'True' : 'False'}`, 'Tests requiring a specific [[CanBlock]] mode are outside the initial MVP.');
  }

  if (negative && negative.phase === 'resolution') {
    pushIssue(blockers, 'resolution-negative', 'negative.phase', 'Module resolution failures are outside the plain script MVP.');
  }

  return blockers;
}

function parseTest262Metadata(sourceText, options) {
  const normalizedOptions = options || {};
  const normalizedPath = normalizePath(normalizedOptions.filePath || '');
  const fileName = getFileName(normalizedPath);
  const frontmatterText = extractTest262Frontmatter(sourceText);
  const unsupported = [];
  const hasFrontmatter = frontmatterText !== null;
  const frontmatter = hasFrontmatter ? parseTest262Frontmatter(frontmatterText) : {};

  const topLevelKeys = Object.keys(frontmatter);
  const unknownKeys = [];
  for (let i = 0; i < topLevelKeys.length; i++) {
    const key = topLevelKeys[i];
    if (KNOWN_FRONTMATTER_KEYS.indexOf(key) < 0) {
      unknownKeys.push(key);
      pushIssue(unsupported, 'unknown-frontmatter-key', key, `Unsupported frontmatter key '${key}'.`);
    }
  }

  const includes = toStringArray(frontmatter.includes, 'includes', unsupported);
  const flags = toStringArray(frontmatter.flags, 'flags', unsupported);
  const features = toStringArray(frontmatter.features, 'features', unsupported);
  const locale = toStringArray(frontmatter.locale, 'locale', unsupported);
  const defines = toStringArray(frontmatter.defines, 'defines', unsupported);
  const negative = normalizeNegative(frontmatter.negative, unsupported);

  for (let i = 0; i < flags.length; i++) {
    if (KNOWN_FLAG_VALUES.indexOf(flags[i]) < 0) {
      pushIssue(unsupported, 'unknown-flag', `flags:${flags[i]}`, `Unsupported flag '${flags[i]}'.`);
    }
  }

  const execution = buildExecutionMetadata(flags, includes, fileName);
  if (execution.strictMode === 'conflicting-flags') {
    pushIssue(unsupported, 'conflicting-strictness-flags', 'flags', 'Multiple strictness/source-type flags were declared together.');
  }

  return {
    filePath: normalizedPath || null,
    fileName: fileName || null,
    hasFrontmatter,
    frontmatter,
    unknownKeys,
    description: toNullableString(frontmatter.description, 'description', unsupported),
    info: toNullableString(frontmatter.info, 'info', unsupported),
    esid: toNullableString(frontmatter.esid, 'esid', unsupported),
    es5id: toNullableString(frontmatter.es5id, 'es5id', unsupported),
    es6id: toNullableString(frontmatter.es6id, 'es6id', unsupported),
    author: toNullableString(frontmatter.author, 'author', unsupported),
    includes,
    flags,
    features,
    locale,
    defines,
    negative,
    execution,
    mvpBlockers: buildMvpBlockers(normalizedPath, negative, execution),
    unsupported,
  };
}

function parseTest262File(filePath) {
  const sourceText = fs.readFileSync(filePath, 'utf8');
  return parseTest262Metadata(sourceText, { filePath });
}

module.exports = {
  extractTest262Frontmatter,
  parseTest262File,
  parseTest262Frontmatter,
  parseTest262Metadata,
};
