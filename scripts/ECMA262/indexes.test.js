'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const { test } = require('node:test');

function fixture(t) {
  const root = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-ecma-index-'));
  t.after(() => fs.rmSync(root, { recursive: true, force: true }));
  function write(relative, text) {
    const file = path.join(root, relative);
    fs.mkdirSync(path.dirname(file), { recursive: true });
    fs.writeFileSync(file, text);
  }
  write('Index.md', [
    '# Coverage', '', '## Summary', '', 'Existing summary', '',
    '## Test262 Conformance Status', '', 'Retain this independently maintained evidence.', '',
    '## Sections', '', '| Section | Title | Status | Spec | Document |',
    '|---:|---|---|---|---|',
    '| 1 | Scope | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-scope) | [Index](1/Index.md) |',
    '| 23 | Collections | Supported | [tc39.es](https://tc39.es/ecma262/#sec-indexed-collections) | [Index](23/Index.md) |',
    '',
  ].join('\n'));
  for (const [number, title, status] of [[1, 'Scope', 'N/A (informational)'], [23, 'Collections', 'Supported']]) {
    write(`${number}/Section${number}.md`, [
      `# Section ${number}: ${title}`, '', '[Back to Index](../Index.md)', '',
      '| Clause | Title | Status | Link |', '|---:|---|---|---|',
      `| ${number} | ${title} | ${status} | [tc39.es](https://tc39.es/ecma262/#sec-${number}) |`, '',
    ].join('\n'));
    write(`${number}/Index.md`, `# Section ${number}\n\n- [Section document](Section${number}.md)\n`);
  }
  for (const [sub, title, status] of [
    [1, 'Array Objects', 'Supported'],
    [2, 'Typed | Array Objects', 'Not Yet Supported'],
    [10, 'Other Collections', 'N/A (informational)'],
  ]) {
    write(`23/Section23_${sub}.json`, JSON.stringify({
      clause: `23.${sub}`, title, status, specUrl: `https://tc39.es/ecma262/#sec-23-${sub}`,
      parent: { clause: '23' },
    }));
    write(`23/Section23_${sub}.md`, `# Section 23.${sub}: ${title}\n\nPreserve this JSON-backed subsection.\n`);
  }
  return { root, write, read: relative => fs.readFileSync(path.join(root, relative), 'utf8') };
}

function run(script, root, ...args) {
  const child = spawnSync(process.execPath, [path.join(__dirname, script), '--root', root, ...args], { encoding: 'utf8' });
  assert.equal(child.status, 0, child.stderr || child.stdout);
}

function assertIndexes(docs, expectedStatus) {
  const nested = docs.read('23/Index.md');
  assert.match(nested, /\| Section \| Title \| Status \| Spec \| Document \|/);
  assert(nested.includes(`| 23 | Collections | ${expectedStatus} |`));
  assert.match(nested, /\| Subsection \| Title \| Status \| Spec \| Document \|/);
  assert(nested.includes('[Section23.md](Section23.md)'));
  assert(nested.includes('[Section23_2.md](Section23_2.md)'));
  assert(nested.includes('[Back to ECMA-262 index](../Index.md)'));
  const rows = nested.split('\n').filter(line => /^\| 23\.\d+ \|/.test(line));
  assert.deepEqual(rows.map(line => line.split('|')[1].trim()), ['23.1', '23.2', '23.10']);
  assert(rows[1].includes('Typed \\| Array Objects'));
  assert(docs.read('Index.md').includes(`| 23 | Collections | ${expectedStatus} |`));
  assert(docs.read('Index.md').includes('Retain this independently maintained evidence.'));
  assert(docs.read('1/Index.md').includes('| 1 | Scope | N/A (informational) |'));
}

test('JSON rollup restores all nested index tables and their section status', t => {
  const docs = fixture(t);
  run('rollupEcma262Statuses.js', docs.root);
  assertIndexes(docs, 'Incomplete');
  assert(docs.read('23/Index.md').includes('| 23.2 | Typed \\| Array Objects | Not Yet Supported |'));
});

test('subsection markdown generation refreshes nested and parent status tables', t => {
  const docs = fixture(t);
  const json = JSON.parse(docs.read('23/Section23_2.json'));
  json.status = 'Partially Supported';
  docs.write('23/Section23_2.json', JSON.stringify(json));
  const untouchedSubsection = docs.read('23/Section23_1.md');
  run('generateEcma262SectionMarkdown.js', docs.root, '--section', '23.2');
  assertIndexes(docs, 'Supported with Limitations');
  assert.equal(docs.read('23/Section23_1.md'), untouchedSubsection);
});

test('legacy splitting preserves JSON-backed pages and uses the same status policy', t => {
  const docs = fixture(t);
  docs.write('23/Section23_1.md', [
    '# Section 23.1: Array Objects', '', '[Back to Index](../Index.md)', '',
    '| Clause | Title | Status | Spec |', '|---|---|---|---|',
    '| 23.1 | Array Objects | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array-objects) |',
    '', '## Detailed support notes', '', 'Do not replace this JSON-backed content.', '',
  ].join('\n'));
  docs.write('23/Section23.md', docs.read('23/Section23.md') +
    '| 23.1 | Array Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-array-objects) |\n');
  const untouchedSubsection = docs.read('23/Section23_1.md');
  run('splitEcma262SectionsIntoSubsections.js', docs.root);
  assertIndexes(docs, 'Incomplete');
  assert.equal(docs.read('23/Section23_1.md'), untouchedSubsection);
});

test('legacy markdown-only sections also get nested tables and mixed-status rollups', t => {
  const docs = fixture(t);
  for (const [sub, status] of [[1, 'Supported'], [2, 'Not Yet Supported'], [10, 'N/A (informational)']]) {
    fs.unlinkSync(path.join(docs.root, `23/Section23_${sub}.json`));
    docs.write(`23/Section23_${sub}.md`, [
      `# Section 23.${sub}`, '', '| Clause | Title | Status | Link |', '|---|---|---|---|',
      `| 23.${sub} | Legacy ${sub} | ${status} | [tc39.es](https://tc39.es/ecma262/#sec-23-${sub}) |`, '',
    ].join('\n'));
  }
  run('splitEcma262SectionsIntoSubsections.js', docs.root);
  assert(docs.read('23/Index.md').includes('| 23 | Collections | Incomplete |'));
  assert(docs.read('23/Index.md').includes('| 23.2 | Legacy 2 | Not Yet Supported |'));
  assert(docs.read('Index.md').includes('| 23 | Collections | Incomplete |'));
});

test('rollups are repeatable without timestamp-only changes', t => {
  const docs = fixture(t);
  const { rollupStatuses } = require('./rollupEcma262Statuses');
  rollupStatuses(docs.root, new Date('2025-01-01T00:00:00Z'));
  const paths = ['Index.md', '1/Index.md', '23/Index.md', '23/Section23.md'];
  const before = paths.map(file => docs.read(file));
  rollupStatuses(docs.root, new Date('2026-01-01T00:00:00Z'));
  assert.deepEqual(paths.map(file => docs.read(file)), before);
});

test('shared status policy handles informational, untracked and legacy statuses', () => {
  const { getRollupStatus } = require('./rollupEcma262Statuses');
  for (const [statuses, expected] of [
    [['Supported', 'Not Yet Supported'], 'Incomplete'],
    [['Supported with Limitations', 'Not Yet Supported'], 'Incomplete'],
    [['Supported', 'Incomplete'], 'Incomplete'],
    [['Supported', 'Not Supported'], 'Incomplete'],
    [['Partially Supported'], 'Supported with Limitations'],
    [['Untracked', 'Untracked'], 'Untracked'],
    [['N/A (informational)', 'N/A (informational)'], 'N/A (informational)'],
    [['Supported', 'N/A (informational)'], 'Supported'],
    [['Not Yet Supported', 'Untracked'], 'Not Yet Supported'],
  ]) {
    assert.equal(getRollupStatus(statuses), expected, statuses.join(', '));
  }
});

test('invalid JSON status fails explicitly instead of publishing a misleading index', t => {
  const docs = fixture(t);
  const json = JSON.parse(docs.read('23/Section23_2.json'));
  json.status = 'Typo';
  docs.write('23/Section23_2.json', JSON.stringify(json));
  const child = spawnSync(process.execPath, [path.join(__dirname, 'rollupEcma262Statuses.js'), '--root', docs.root], { encoding: 'utf8' });
  assert.notEqual(child.status, 0);
  assert.match(child.stderr, /Invalid status 'Typo'/);
});

test('every checked-in numeric index matches JSON statuses, root rollups and real document links', () => {
  const { getRollupStatus } = require('./rollupEcma262Statuses');
  const root = path.resolve(__dirname, '../../docs/ECMA262');
  const parent = fs.readFileSync(path.join(root, 'Index.md'), 'utf8');
  const sections = fs.readdirSync(root, { withFileTypes: true }).filter(entry => entry.isDirectory() && /^\d+$/.test(entry.name));
  assert(sections.length > 0);
  for (const section of sections) {
    const dir = path.join(root, section.name);
    const index = fs.readFileSync(path.join(dir, 'Index.md'), 'utf8');
    const rootRow = parent.split(/\r?\n/).find(line => line.startsWith(`| ${section.name} |`));
    assert(rootRow, `Missing root row for ${section.name}`);
    const rootStatus = rootRow.split('|')[3].trim();
    assert(index.split(/\r?\n/).some(line => line.startsWith(`| ${section.name} |`) && line.includes(`| ${rootStatus} |`)));
    const jsonFiles = fs.readdirSync(dir).filter(name => new RegExp(`^Section${section.name}_\\d+\\.json$`).test(name));
    const statuses = [];
    for (const jsonFile of jsonFiles) {
      const doc = JSON.parse(fs.readFileSync(path.join(dir, jsonFile), 'utf8'));
      const status = getRollupStatus([doc.status]);
      statuses.push(status);
      const title = doc.title.replace(/\|/g, '\\|');
      assert(index.includes(`| ${doc.clause} | ${title} | ${status} |`), `Stale subsection ${doc.clause}`);
    }
    if (statuses.length > 0) assert.equal(rootStatus, getRollupStatus(statuses), `Stale section ${section.name}`);
    for (const match of index.matchAll(/\]\(([^)]+\.md)\)/g)) {
      assert(fs.existsSync(path.resolve(dir, match[1])), `Broken link in ${section.name}/Index.md: ${match[1]}`);
    }
    const subsectionRows = index.split(/\r?\n/).filter(line => new RegExp(`^\\| ${section.name}\\.\\d+ \\|`).test(line));
    assert.equal(subsectionRows.length, jsonFiles.length, `Missing or duplicate subsection rows in ${section.name}`);
  }
});

test('splitting the complete documentation tree preserves all JSON-backed subsection pages', t => {
  const source = path.resolve(__dirname, '../../docs/ECMA262');
  const root = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-ecma-split-'));
  t.after(() => fs.rmSync(root, { recursive: true, force: true }));
  fs.cpSync(source, root, { recursive: true });
  const preserved = new Map();
  for (const entry of fs.readdirSync(source, { withFileTypes: true })) {
    if (!entry.isDirectory() || !/^\d+$/.test(entry.name)) continue;
    const dir = path.join(source, entry.name);
    for (const name of fs.readdirSync(dir)) {
      if (!new RegExp(`^Section${entry.name}_\\d+\\.json$`).test(name)) continue;
      const relative = path.join(entry.name, name.replace(/\.json$/, '.md'));
      preserved.set(relative, fs.readFileSync(path.join(source, relative)));
    }
  }
  run('splitEcma262SectionsIntoSubsections.js', root);
  for (const [relative, bytes] of preserved) {
    assert.deepEqual(fs.readFileSync(path.join(root, relative)), bytes, relative);
  }
  const withoutTimestamp = text => text.split(/\r?\n/).filter(line => !line.startsWith('> Last generated (UTC):')).join('\n');
  for (const relative of ['Index.md', ...fs.readdirSync(source).filter(name => /^\d+$/.test(name)).map(name => `${name}/Index.md`)]) {
    assert.equal(
      withoutTimestamp(fs.readFileSync(path.join(root, relative), 'utf8')),
      withoutTimestamp(fs.readFileSync(path.join(source, relative), 'utf8')),
      relative
    );
  }
});
