'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { test } = require('node:test');
const { buildCorpus, directory } = require('./buildBehaviorCorpus');
const { validateCorpus, verifyUpstream } = require('./behaviorCorpus');
const { hashJson, sha256, validateArtifact } = require('./artifacts');
const inventory = require('../../docs/nodejs/compatibility/inventory.v1.json');

function validate(manifest, metadata, changedFiles = new Map()) {
  return validateCorpus(manifest, file => file === manifest.corpusMetadata
    ? Buffer.from(JSON.stringify(metadata))
    : changedFiles.get(file) ?? fs.readFileSync(path.join(directory, file)), inventory);
}

test('checked-in behavior corpus is deterministic, content-pinned and losslessly mapped', () => {
  const { manifest, metadata } = buildCorpus();
  assert.deepEqual(manifest, JSON.parse(fs.readFileSync(path.join(directory, 'manifest.json'))));
  assert.deepEqual(metadata, JSON.parse(fs.readFileSync(path.join(directory, 'corpus.json'))));
  assert.deepEqual(buildCorpus(), { manifest, metadata });
  assert.deepEqual(validate(manifest, metadata), JSON.parse(JSON.stringify(metadata)));
  assert.equal(manifest.cases.length, 19);
  assert.equal(metadata.upstream.files.length, 32);
  assert.equal(new Set(manifest.cases.map(item => item.id)).size, 19);
  const mapped = new Set(manifest.cases.flatMap(item => item.targets));
  assert(inventory.behaviors.every(item => mapped.has(item.id)));
  assert.equal(metadata.scenarios.filter(item => item.origin.kind === 'original').length, 19);
  assert.equal(metadata.upstream.files.filter(item => item.classification === 'runnable').length, 0);
  assert(metadata.upstream.files.some(item => item.applicability.state === 'unknown'));
});

test('corpus rejects modified bytes, missing dependencies and forged target/policy mappings', () => {
  const { manifest, metadata } = buildCorpus();
  assert.throws(() => validate(manifest, metadata, new Map([['path-posix.js', Buffer.from('changed')]])), /hash\/size/);
  const missing = structuredClone(metadata);
  missing.fixtures.pop();
  assert.throws(() => validate(manifest, missing), /inventory/);
  const forged = structuredClone(metadata);
  forged.scenarios[0].targets = ['node:module/buffer'];
  assert.throws(() => validate(manifest, forged), /mapping/);
  const policy = structuredClone(manifest);
  policy.cases[0].normalization = 'crlf-to-lf';
  assert.throws(() => validate(policy, metadata), /mapping/);
  const unknown = structuredClone(manifest);
  const matching = structuredClone(metadata);
  unknown.cases[0].targets = ['node:module/not-an-inventoried-api'];
  matching.scenarios[0].targets = unknown.cases[0].targets;
  matching.scenarios[0].scenarioSha256 = hashJson(unknown.cases[0]);
  assert.throws(() => validate(unknown, matching), /mapping/);
});

test('profile/source review and original-vs-upstream provenance fail closed', () => {
  const { manifest, metadata } = buildCorpus();
  for (const mutate of [
    value => { value.upstream.reviewedForReferenceProfileSha256 = '0'.repeat(64); },
    value => { value.referenceProfileSha256 = '0'.repeat(64); },
    value => { value.upstream.files[0].url = 'https://example.invalid/mutable-main'; },
    value => { value.upstream.files[0].classification = 'runnable'; },
    value => { value.upstream.files.push(value.upstream.files[0]); },
    value => { value.scenarios[0].origin.relatedUpstreamPaths = ['test/not-inspected.js']; },
    value => { value.scenarios[0].origin.kind = 'upstream'; }
  ]) {
    const changed = structuredClone(metadata);
    mutate(changed);
    assert.throws(() => validate(manifest, changed));
  }
  const changed = structuredClone(manifest);
  changed.cases[0].files = changed.cases[0].files.filter(file => file !== changed.corpusMetadata);
  assert.throws(() => validateArtifact(changed, 'scenario-manifest'), /retain corpus metadata/);
});

test('fixture revisions change content identity without changing stable scenario/target identity', t => {
  const root = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-corpus-generation-'));
  t.after(() => fs.rmSync(root, { recursive: true, force: true }));
  fs.cpSync(directory, root, { recursive: true });
  const before = buildCorpus(root);
  fs.appendFileSync(path.join(root, 'path-posix.js'), '\n');
  const after = buildCorpus(root);
  assert.deepEqual(before.manifest, after.manifest);
  assert.notEqual(hashJson(before.metadata.fixtures), hashJson(after.metadata.fixtures));
  assert.deepEqual(before.metadata.scenarios, after.metadata.scenarios);
  const review = JSON.parse(fs.readFileSync(path.join(root, 'upstream-inventory.json')));
  review.reviewedForReferenceProfileSha256 = '0'.repeat(64);
  fs.writeFileSync(path.join(root, 'upstream-inventory.json'), JSON.stringify(review));
  assert.throws(() => buildCorpus(root), /reviewed explicitly/);
});

test('upstream verification checks exact source and license bytes, never silently repins', async () => {
  const { metadata } = buildCorpus();
  const synthetic = structuredClone(metadata);
  const contents = new Map();
  for (const file of [synthetic.upstream.license, ...synthetic.upstream.files]) {
    const bytes = Buffer.from(file.path);
    file.sha256 = sha256(bytes);
    file.bytes = bytes.length;
    contents.set(file.url, bytes);
  }
  await verifyUpstream(synthetic, async url => contents.get(url));
  await assert.rejects(verifyUpstream(synthetic, async () => Buffer.from('wrong source')), /hash\/size/);
  await assert.rejects(verifyUpstream(synthetic, async () => { throw new Error('Offline'); }), /Offline/);
});
