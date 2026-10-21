'use strict';

const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { execFileSync } = require('node:child_process');
const { test } = require('node:test');
const { classify } = require('./catalogBridge');

function fixture(frontmatter, check) {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'test262-specification-'));
  const source = `/*---\r\n${frontmatter}\r\n---*/\r\nvoid 0;\r\n`;
  try {
    fs.writeFileSync(path.join(directory, 'example.js'), source);
    const row = classify(directory, 'example.js', {});
    assert.equal(row.sha256, crypto.createHash('sha256').update(source).digest('hex'));
    check(row, directory);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
}

test('ingestion preserves modern reference, descriptions and exact source frontmatter', () => {
  fixture('esid: sec-example\r\ndescription: A useful description\r\ninfo: |\r\n  Algorithm details.\r\nauthor: Example', row => {
    assert.equal(row.metadata.esid, 'sec-example');
    assert.equal(row.metadata.description, 'A useful description');
    assert.equal(row.metadata.info, 'Algorithm details.');
    assert.equal(row.metadata.author, 'Example');
    assert.deepEqual(row.metadata.specification.raw_identifiers, { esid: 'sec-example' });
    assert.equal(row.metadata.specification.reference_state, 'unresolved');
    assert.equal(row.metadata.specification.schema_version, 1);
    assert.equal(row.metadata.specification.raw_frontmatter,
      '\r\nesid: sec-example\r\ndescription: A useful description\r\ninfo: |\r\n  Algorithm details.\r\nauthor: Example\r\n');
  });
});

for (const key of ['es5id', 'es6id']) {
  test(`ingestion preserves ${key} as legacy without converting its clause identifier`, () => {
    fixture(`${key}: " 10.5-1-s "\r\ndescription: Legacy reference`, row => {
      assert.equal(row.metadata[key], ' 10.5-1-s ');
      assert.deepEqual(row.metadata.specification.raw_identifiers, { [key]: ' 10.5-1-s ' });
      assert.equal(row.metadata.specification.reference_state, 'legacy');
    });
  });
}

test('reference-free fixture is explicitly absent, never inferred from the filename', () => {
  fixture('description: No specification identifier', row => {
    assert.equal(row.metadata.esid, null);
    assert.equal(row.metadata.es5id, null);
    assert.equal(row.metadata.es6id, null);
    assert.deepEqual(row.metadata.specification.raw_identifiers, {});
    assert.equal(row.metadata.specification.reference_state, 'absent');
  });
});

test('multiple or malformed identifiers remain raw and explicitly ambiguous', () => {
  for (const frontmatter of ['esid: sec-example\r\nes5id: 10.5', 'esid: [sec-one, sec-two]',
    'esid:', 'esid: sec-first\r\nesid: sec-second']) {
    fixture(frontmatter, row => {
      assert.equal(row.metadata.specification.reference_state, 'ambiguous');
      assert.ok(Object.hasOwn(row.metadata.specification.raw_identifiers, 'esid'));
    });
  }
});

test('metadata errors and missing frontmatter are unresolved, not absent', () => {
  fixture('invalid yaml', row => {
    assert.equal(row.state, 'metadata-error');
    assert.equal(row.metadata.specification.reference_state, 'unresolved');
    assert.match(row.metadata.specification.diagnostic, /Invalid frontmatter/);
    assert.equal(row.metadata.specification.raw_frontmatter, '\r\ninvalid yaml\r\n');
  });
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'test262-specification-'));
  try {
    fs.writeFileSync(path.join(directory, 'example.js'), 'void 0;');
    const row = classify(directory, 'example.js', {});
    assert.equal(row.metadata.specification.reference_state, 'unresolved');
    assert.equal(row.metadata.specification.raw_frontmatter, null);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test('bounded metadata bridge request reads source without executing it', () => {
  fixture('esid: sec-example', (row, directory) => {
    fs.appendFileSync(path.join(directory, 'example.js'), 'throw new Error("must not execute");');
    const rows = JSON.parse(execFileSync(process.execPath, [path.join(__dirname, 'catalogBridge.js')], {
      input: JSON.stringify({ command: 'metadata', root: directory, paths: ['example.js'] }),
      encoding: 'utf8',
    }));
    assert.equal(rows.length, 1);
    assert.deepEqual(rows[0].metadata, row.metadata);
  });
});

test('metadata bridge rejects unbounded requests and paths outside the checkout', () => {
  for (const paths of [Array(251).fill('example.js'), ['../example.js'], ['/example.js'], ['test\\example.js']]) {
    assert.throws(() => execFileSync(process.execPath, [path.join(__dirname, 'catalogBridge.js')], {
      input: JSON.stringify({ command: 'metadata', root: __dirname, paths }),
      encoding: 'utf8', stdio: 'pipe',
    }), /Metadata requests require/);
  }
});
