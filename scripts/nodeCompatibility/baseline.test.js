'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const os = require('node:os');
const { spawnSync } = require('node:child_process');
const { test } = require('node:test');
const { buildInventory, verifyDocumentation, readContractEvidence, sha256 } = require('./buildInventory');
const { validateEnvironment } = require('./checkReference');
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
    nodeVersion: '24.22.1', platform: 'linux', architecture: 'x64',
    nodeArguments: [], runtimeOverridesPresent: [],
    dotnetSdk: '10.0.113', dotnetRuntimes: ['10.0.13'],
    jrocRevision: 'a'.repeat(40), worktreeDirty: false,
    inventorySha256: 'b'.repeat(64), corpusRevision: 'c'.repeat(64),
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
    { inventorySha256: null }, { corpusRevision: null }, { lockfiles: [] },
    { builtinModules: ['http'] }, { nodeArguments: ['--jitless'] },
    { runtimeOverridesPresent: ['NODE_OPTIONS'] }
  ]) {
    assert.throws(() => validateEnvironment({ ...environment(), ...changed }, config));
  }
  assert.doesNotThrow(() => validateEnvironment({ ...environment(), worktreeDirty: true }, config, { allowDirty: true }));
});

test('checked-in inventory has unique IDs and only explicitly untested evidence', () => {
  const inventory = JSON.parse(fs.readFileSync(path.join(repoRoot, 'docs/nodejs/compatibility/inventory.v1.json'), 'utf8'));
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
