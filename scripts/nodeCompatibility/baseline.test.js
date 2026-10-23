'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const os = require('node:os');
const { spawnSync } = require('node:child_process');
const { test } = require('node:test');
const { buildInventory, verifyDocumentation, readContractEvidence, sha256 } = require('./buildInventory');
const { validateEnvironment, collectEnvironment } = require('./checkReference');
const { hashJson, inventoryDigests, validateArtifact } = require('./artifacts');
const config = require('./baseline.json');
const repoRoot = path.resolve(__dirname, '../..');

function sourceFixture() {
  return {
    modules: [{
      source: 'doc/api/http.md', name: 'http', methods: [
        { name: 'request', textRaw: '`http.request(options[, callback])`', signatures: [{ params: [{ name: 'options', type: 'Object' }] }] },
        { name: 'request', textRaw: '`http.request(url[, options][, callback])`', signatures: [{ params: [{ name: 'url', type: 'string' }] }] }
      ], classes: [{
        name: 'http.Agent', textRaw: 'Class: `http.Agent`',
        signatures: [{ params: [{ name: 'options', type: 'Object' }] }],
        methods: [{ name: 'destroy', textRaw: '`agent.destroy()`' }],
        properties: [{ name: 'Type', textRaw: '`freeSockets` Type: {Object}' }]
      }]
    }, {
      source: 'doc/api/http2.md', name: 'http/2',
      methods: [{ name: 'connect', textRaw: '`http2.connect(authority[, options])`' }]
    }],
    miscs: [{
      source: 'doc/api/globals.md', name: 'Global objects',
      miscs: [{ name: '`fetch`', textRaw: '`fetch`' }]
    }, {
      source: 'doc/api/n-api.md', name: 'Node-API',
      methods: [{ name: 'native', textRaw: '`native()`' }]
    }]
  };
}

test('inventory retains missing modules, globals, classes, members and overload families', () => {
  const inventory = buildInventory(sourceFixture(), config, []);
  const entries = inventory.entries;
  assert(entries.some(x => x.id === 'node:module/http2'));
  assert(entries.some(x => x.id === 'node:api/http/method/http.request' && x.headings.length === 2));
  assert.equal(entries.find(x => x.id === 'node:api/http/method/http.request').signatureShapes.length, 2);
  assert(entries.some(x => x.id === 'node:api/http/class/http.Agent/constructor/new'));
  assert(entries.some(x => x.id === 'node:api/http/class/http.Agent/method/destroy'));
  assert(entries.some(x => x.id === 'node:api/http/class/http.Agent/property/freeSockets'));
  assert(entries.some(x => x.id === 'node:api/globals/global/fetch'));
  assert(!entries.some(x => x.id.includes('/native')));
  assert(inventory.documents.some(x => x.id === 'n-api' && x.scope === 'excluded' && x.reason));
  assert.equal(inventory.behaviors.length, 8);
  assert(inventory.behaviors.every(x => x.status === 'not-run'));
});

test('IDs and serialization do not depend on traversal or overload order', () => {
  const first = sourceFixture();
  const second = sourceFixture();
  second.modules.reverse();
  second.modules[1].methods.reverse();
  assert.deepEqual(buildInventory(first, config, []), buildInventory(second, config, []));
});

test('duplicate global class records merge without duplicating their members', () => {
  const data = sourceFixture();
  const item = { name: 'URL', textRaw: 'Class: `URL`', methods: [{ name: 'toString', textRaw: '`url.toString()`' }] };
  data.miscs[0].classes = [item];
  data.classes = [{ ...item, source: 'doc/api/globals.md' }];
  const inventory = buildInventory(data, config, []);
  assert.equal(inventory.entries.filter(x => x.id === 'node:api/globals/class/URL').length, 1);
  assert.equal(inventory.entries.filter(x => x.id === 'node:api/globals/class/URL/method/toString').length, 1);
});

test('nested object members stay distinct from class methods with the same name', () => {
  const data = sourceFixture();
  data.modules[0].classes[0].properties[0].methods = [
    { name: 'destroy', textRaw: '`agent.freeSockets.destroy()`' }
  ];
  const entries = buildInventory(data, config, []).entries;
  assert(entries.some(x => x.id === 'node:api/http/class/http.Agent/method/destroy'));
  assert(entries.some(x => x.id === 'node:api/http/class/http.Agent/property/freeSockets/method/destroy'));
});

test('function-object properties do not collide and explicit subpath namespaces remain distinct', () => {
  const data = sourceFixture();
  data.modules[0].methods.forEach(method => {
    method.properties = [{ name: 'Type', textRaw: '`custom` Type: {symbol}' }];
  });
  data.modules[0].methods[1].name = 'get';
  data.modules[0].methods[1].textRaw = '`http.get(url)`';
  data.modules.push({
    source: 'doc/api/inspector.md', name: 'inspector', modules: [
      { name: 'promises_api', classes: [{ name: 'inspector.Session', textRaw: 'Class: `inspector.Session`' }] },
      { name: 'callback_api', classes: [{ name: 'inspector.Session', textRaw: 'Class: `inspector.Session`' }] }
    ]
  });
  const entries = buildInventory(data, config, []).entries;
  assert(entries.some(x => x.id === 'node:api/http/method/http.request/property/custom'));
  assert(entries.some(x => x.id === 'node:api/http/method/http.get/property/custom'));
  assert(entries.some(x => x.id === 'node:api/inspector/class/inspector.Session'));
  assert(entries.some(x => x.id === 'node:api/inspector/promises/class/inspector.Session'));
});

test('contract configuration is evidence, never a behavioral pass', () => {
  const evidence = [{
    module: 'http', source: 'contract.cs', nodeVersion: '24.18.1',
    members: ['request'], implementations: [], overrides: [], adapterSource: 'adapter.cs'
  }];
  const inventory = buildInventory(sourceFixture(), config, evidence);
  const request = inventory.entries.find(x => x.id === 'node:api/http/method/http.request');
  assert.equal(request.contracts[0].adapter, 'generated-stub');
  assert.equal(inventory.defaults.availability, 'unverified');
  assert.equal(inventory.defaults.behavior, 'not-run');
  assert.equal(inventory.defaults.applicability.platform, 'review-required');
});

test('documentation hash mismatch fails closed', () => {
  assert.throws(() => verifyDocumentation(Buffer.from('{}'), config), /SHA-256/);
  const bytes = Buffer.from(JSON.stringify(sourceFixture()));
  assert.deepEqual(verifyDocumentation(bytes, {
    reference: { documentationSha256: sha256(bytes) }
  }), sourceFixture());
  assert.throws(() => buildInventory({ modules: [{ name: 'missing-source' }] }, config, []), /source path/);
});

test('CLI rejects unknown flags and wrong documentation before changing the inventory', () => {
  for (const script of ['buildInventory.js', 'checkReference.js']) {
    const result = spawnSync(process.execPath, [path.join(__dirname, script), '--invalid'], { encoding: 'utf8' });
    assert.equal(result.status, 1);
    assert.match(result.stderr, /Unknown argument|Usage:/);
  }
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-node-inventory-'));
  try {
    const input = path.join(directory, 'invalid.json');
    fs.writeFileSync(input, '{}');
    const inventory = fs.readFileSync(path.join(repoRoot, 'docs/nodejs/compatibility/inventory.v1.json'));
    const result = spawnSync(process.execPath, [path.join(__dirname, 'buildInventory.js'), '--input', input], { encoding: 'utf8' });
    assert.equal(result.status, 1);
    assert.match(result.stderr, /SHA-256 mismatch/);
    assert.deepEqual(fs.readFileSync(path.join(repoRoot, 'docs/nodejs/compatibility/inventory.v1.json')), inventory);
  } finally {
    fs.unlinkSync(path.join(directory, 'invalid.json'));
    fs.rmdirSync(directory);
  }
});

function environment() {
  return {
    schemaVersion: 1, suiteId: config.suiteId, artifactKind: 'reference-environment',
    scopeId: config.scopeId, recordedAt: '2026-10-23T19:00:00.000Z',
    nodeExecutable: '/usr/bin/node', nodeVersions: { node: '24.22.1' },
    kernelVersion: 'test-kernel',
    nodeVersion: '24.22.1', platform: 'linux', architecture: 'x64',
    nodeArguments: [], runtimeOverridesPresent: [],
    dotnetSdk: '10.0.113', dotnetRuntimes: ['10.0.13'],
    jrocRevision: 'a'.repeat(40), worktreeDirty: false,
    inventorySha256: 'b'.repeat(64), behaviorRegistrationSha256: 'c'.repeat(64),
    targetInventorySha256: 'e'.repeat(64), implementationEvidenceSha256: 'f'.repeat(64),
    referenceProfileSha256: hashJson(config.reference),
    baselineConfigSha256: 'b'.repeat(64), executableBehaviorCases: 0,
    referenceEnvironmentEligible: true, referenceSourceRevision: config.reference.sourceRevision,
    lockfiles: [{ path: 'package-lock.json', sha256: 'd'.repeat(64) }],
    builtinModules: config.builtinModules
  };
}

test('reference environment rejects patch, platform, SDK, runtime and dirty drift', () => {
  assert.doesNotThrow(() => validateEnvironment(environment(), config));
  for (const changed of [
    { nodeVersion: '24.18.0' }, { nodeVersion: '24.22.2' },
    { platform: 'win32' }, { architecture: 'arm64' },
    { dotnetSdk: '10.0.114' }, { dotnetRuntimes: ['10.0.12'] },
    { dotnetRuntimes: ['10.0.13', '10.0.14'] },
    { jrocRevision: 'unknown' }, { worktreeDirty: true },
    { inventorySha256: null }, { behaviorRegistrationSha256: null }, { lockfiles: [] },
    { builtinModules: ['http'] }, { nodeArguments: ['--jitless'] },
    { runtimeOverridesPresent: ['NODE_OPTIONS'] }
  ]) {
    assert.throws(() => validateEnvironment({ ...environment(), ...changed }, config));
  }
  assert.doesNotThrow(() => validateEnvironment({
    ...environment(), worktreeDirty: true, referenceEnvironmentEligible: false
  }, config, { allowDirty: true }));
});

test('checked-in inventory has unique IDs and only explicitly untested evidence', () => {
  const inventory = JSON.parse(fs.readFileSync(path.join(repoRoot, 'docs/nodejs/compatibility/inventory.v1.json'), 'utf8'));
  assert.doesNotThrow(() => validateArtifact(inventory, 'api-inventory'));
  assert.equal(new Set(inventory.entries.map(x => x.id)).size, inventory.entries.length);
  assert.equal(inventory.reference.nodeVersion, config.reference.nodeVersion);
  assert.equal(inventory.reference.documentationSha256, config.reference.documentationSha256);
  assert(inventory.entries.length > 1500);
  assert(inventory.entries.some(x => x.id === 'node:module/worker_threads'));
  assert(inventory.entries.some(x => x.id === 'node:api/globals/global/fetch'));
  assert.equal(inventory.defaults.behavior, 'not-run');
  assert.equal(inventory.behaviors.length, config.loaderBehaviors.length);
  assert(readContractEvidence(repoRoot).some(x => x.module === 'http' && x.members.includes('METHODS')));
  const processContract = inventory.contracts.find(x => x.module === 'process' && !x.type);
  assert(processContract.implementations.includes('exit'));
  const stub = inventory.entries.find(x => x.id === 'node:api/http/property/METHODS');
  assert.equal(stub.contracts[0].adapter, 'generated-stub');
});

test('artifacts have explicit suite and artifact identities', () => {
  const inventory = buildInventory(sourceFixture(), config, []);
  assert.equal(inventory.suiteId, 'nodejs-compatibility');
  assert.equal(inventory.artifactKind, 'api-inventory');
});

test('registration-only environments do not require an executable corpus identity', () => {
  const sample = environment();
  assert(!Object.hasOwn(sample, 'corpusRevision'));
  assert.doesNotThrow(() => validateEnvironment(sample, config));
});

test('target and implementation identities change independently', () => {
  const first = buildInventory(sourceFixture(), config, []);
  const contracts = readContractEvidence(repoRoot);
  const second = buildInventory(sourceFixture(), config, contracts);
  assert.match(first.targetInventorySha256, /^[a-f0-9]{64}$/);
  assert.equal(first.targetInventorySha256, second.targetInventorySha256);
  assert.notEqual(first.implementationEvidenceSha256, second.implementationEvidenceSha256);
  const changed = sourceFixture();
  changed.modules[0].methods.push({ name: 'get', textRaw: '`http.get(options)`' });
  const third = buildInventory(changed, config, []);
  assert.notEqual(first.targetInventorySha256, third.targetInventorySha256);
  assert.equal(first.implementationEvidenceSha256, third.implementationEvidenceSha256);
});

test('contract-only changes do not reidentify target scope', () => {
  const contracts = readContractEvidence(repoRoot);
  const first = buildInventory(sourceFixture(), config, contracts);
  const changed = structuredClone(contracts);
  changed[0].sha256 = '0'.repeat(64);
  const second = buildInventory(sourceFixture(), config, changed);
  assert.equal(first.targetInventorySha256, second.targetInventorySha256);
  assert.notEqual(first.implementationEvidenceSha256, second.implementationEvidenceSha256);
  assert.notEqual(sha256(JSON.stringify(first)), sha256(JSON.stringify(second)));
});

test('reference upgrades do not reidentify unchanged targets or implementation evidence', () => {
  const first = buildInventory(sourceFixture(), config, []);
  const updated = structuredClone(config);
  updated.reference.nodeVersion = '24.22.2';
  updated.reference.sourceRevision = 'a'.repeat(40);
  const second = buildInventory(sourceFixture(), updated, []);
  assert.equal(first.targetInventorySha256, second.targetInventorySha256);
  assert.equal(first.implementationEvidenceSha256, second.implementationEvidenceSha256);
  assert.notEqual(first.referenceProfileSha256, second.referenceProfileSha256);
  assert.deepEqual(first.entries.map(x => x.id), second.entries.map(x => x.id));
});

test('semantic hashing is independent of object-key order but preserves array order', () => {
  assert.equal(hashJson({ z: 1, a: { y: 2, b: 3 } }), hashJson({ a: { b: 3, y: 2 }, z: 1 }));
  assert.notEqual(hashJson([1, 2]), hashJson([2, 1]));
  assert.throws(() => hashJson({ missing: undefined }), /undefined/);
  assert.throws(() => hashJson({ number: Infinity }), /non-finite/);
});

test('typed inventory artifacts round-trip without losing evidence or identities', () => {
  const original = JSON.parse(fs.readFileSync(path.join(repoRoot, 'docs/nodejs/compatibility/inventory.v1.json'), 'utf8'));
  const restored = JSON.parse(JSON.stringify(original));
  validateArtifact(restored, 'api-inventory');
  assert.deepEqual(restored, original);
  assert.deepEqual(inventoryDigests(restored), inventoryDigests(original));
});

test('inventory validation rejects malformed, ambiguous and forged artifacts', () => {
  const original = buildInventory(sourceFixture(), config, []);
  validateArtifact(original, 'api-inventory');
  const mutations = [
    value => { delete value.suiteId; },
    value => { value.suiteId = 'test262'; },
    value => { value.artifactKind = 'reference-environment'; },
    value => { value.schemaVersion = 2; },
    value => { value.unrecognized = true; },
    value => { value.defaults.behavior = 'pass'; },
    value => { value.entries[0].stability = ['stable']; },
    value => { value.targetInventorySha256 = '0'.repeat(64); },
    value => { value.implementationEvidenceSha256 = '0'.repeat(64); },
    value => { value.referenceProfileSha256 = '0'.repeat(64); },
    value => { value.behaviors[0].status = 'pass'; },
    value => { value.behaviors[0].expectation += ' changed'; },
    value => { value.entries.push(structuredClone(value.entries[0])); },
    value => { value.behaviors.push(structuredClone(value.behaviors[0])); },
    value => { value.entries[0].owner = 'unknown'; },
    value => { value.entries[0].contracts = [{ source: 'unknown.cs' }]; },
    value => { delete value.documents.find(x => x.scope === 'excluded').reason; }
  ];
  for (const mutate of mutations) {
    const changed = structuredClone(original);
    mutate(changed);
    assert.throws(() => validateArtifact(changed, 'api-inventory'));
  }
  assert.throws(() => validateArtifact(original, 'unknown'), /Unknown artifact/);
});

test('environment validation rejects mislabeled corpus identity and eligibility claims', () => {
  const original = environment();
  validateArtifact(JSON.parse(JSON.stringify(original)), 'reference-environment');
  assert.doesNotThrow(() => validateEnvironment({
    ...original, nodeVersions: { ...original.nodeVersions, nghttp3: '', ngtcp2: '' }
  }, config));
  for (const changed of [
    { corpusRevision: 'a'.repeat(64) },
    { suiteId: 'test262' }, { artifactKind: 'api-inventory' }, { schemaVersion: 2 },
    { executableBehaviorCases: 8 }, { behaviorRegistrationSha256: 'unknown' },
    { recordedAt: '2026-02-30T00:00:00.000Z' }, { referenceEnvironmentEligible: false },
    { nodeVersions: { node: '24.18.0' } },
    { lockfiles: [...original.lockfiles, ...original.lockfiles] },
    { referenceProfileSha256: '0'.repeat(64) },
    { referenceSourceRevision: '0'.repeat(40) }
  ]) {
    assert.throws(() => validateEnvironment({ ...original, ...changed }, config));
  }
});

test('reference collection rejects malformed, forged and stale inventory evidence', t => {
  const inventoryPath = path.join(repoRoot, 'docs/nodejs/compatibility/inventory.v1.json');
  const readFile = fs.readFileSync.bind(fs);
  const original = JSON.parse(readFile(inventoryPath, 'utf8'));
  let changed;
  t.mock.method(fs, 'readFileSync', (file, ...args) => file === inventoryPath
    ? Buffer.from(JSON.stringify(changed)) : readFile(file, ...args));
  changed = structuredClone(original);
  changed.defaults.behavior = 'pass';
  assert.throws(() => collectEnvironment(), /Invalid api-inventory artifact/);
  changed = structuredClone(original);
  changed.targetInventorySha256 = '0'.repeat(64);
  assert.throws(() => collectEnvironment(), /targetInventorySha256 mismatch/);
  changed = structuredClone(original);
  changed.contracts[0].sha256 = '0'.repeat(64);
  Object.assign(changed, inventoryDigests(changed));
  assert.throws(() => collectEnvironment(), /contract evidence is stale/);
});
