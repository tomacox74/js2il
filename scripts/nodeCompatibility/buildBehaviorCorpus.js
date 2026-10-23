'use strict';

const fs = require('node:fs');
const path = require('node:path');
const { hashJson, sha256, validateArtifact } = require('./artifacts');
const baseline = require('./baseline.json');
const { validateCorpus } = require('./behaviorCorpus');

const directory = path.resolve(__dirname, '../differential-test/corpus/node-public-api-v1');
const api = (module, member) => `node:api/${module}/${member}`;
const loader = name => `node:behavior/loader/${name}`;
const upstream = name => `test/${name.includes('/') ? name : `parallel/${name}`}`;
const conditional = [
  'packages/consumer/package.json',
  ...['package.json', 'require.cjs', 'import.mjs', 'fallback.cjs', 'feature.cjs', 'private.cjs']
    .map(file => `packages/consumer/node_modules/conditional/${file}`)
];
const definitions = [
  ['path/posix', 'path-posix.js', ['join', 'normalize', 'relative', 'isAbsolute'].map(name => api('path', `method/path.${name}`)),
    [], ['test-path-join.js', 'test-path-normalize.js', 'test-path-relative.js']],
  ['path/win32', 'path-win32.js', ['join', 'normalize', 'relative', 'isAbsolute'].map(name => api('path', `method/path.${name}`)),
    [], ['test-path-normalize.js', 'test-path-win32-exists.js']],
  ['path/shape-and-boundaries', 'path-shape.js', ['node:module/path', ...['parse', 'format', 'basename', 'extname'].map(name => api('path', `method/path.${name}`))],
    [], ['test-path-parse-format.js', 'test-path-basename.js', 'test-path-extname.js']],
  ['path/invalid-inputs', 'path-errors.js', ['join', 'basename', 'parse', 'relative'].map(name => api('path', `method/path.${name}`)),
    [], ['test-path-join.js', 'test-path-parse-format.js']],
  ['buffer/encodings', 'buffer-encoding.js', ['from', 'byteLength'].map(name => api('buffer', `class/Buffer/static-method/${name}`)),
    [], ['test-buffer-from.js', 'test-buffer-bytelength.js']],
  ['buffer/aliasing-and-boundaries', 'buffer-boundaries.js', [api('buffer', 'class/Buffer/static-method/alloc'),
    ...['subarray', 'writeUInt32LE', 'writeUInt16BE', 'readUInt32LE', 'readUInt16BE'].map(name => api('buffer', `class/Buffer/method/${name}`))],
    [], ['test-buffer-readuint.js', 'test-buffer-writeuint.js', 'test-buffer-alloc.js']],
  ['buffer/invalid-inputs', 'buffer-errors.js', ['from', 'alloc'].map(name => api('buffer', `class/Buffer/static-method/${name}`)),
    [], ['test-buffer-from.js', 'test-buffer-alloc.js', 'test-buffer-readuint.js']],
  ['buffer/export-shape', 'buffer-shape.js', ['node:module/buffer', api('buffer', 'class/Buffer/static-method/isBuffer'), api('buffer', 'class/Buffer/method/toJSON')],
    [], ['test-buffer-from.js']],
  ['events/synchronous-order', 'events-order.js', ['on', 'prependListener', 'emit'].map(name => api('events', `class/EventEmitter/method/${name}`)),
    [], ['test-event-emitter-once.js', 'test-event-emitter-emit-context.js']],
  ['events/once-and-removal', 'events-once-remove.js', ['once', 'on', 'emit', 'removeListener', 'listenerCount', 'eventNames'].map(name => api('events', `class/EventEmitter/method/${name}`)),
    [], ['test-event-emitter-once.js', 'test-event-emitter-remove-listeners.js']],
  ['events/error-semantics', 'events-errors.js', ['emit', 'on', 'once', 'removeListener'].map(name => api('events', `class/EventEmitter/method/${name}`)),
    [], ['test-event-emitter-errors.js', 'test-event-emitter-invalid-listener.js']],
  ['events/export-shape', 'events-shape.js', ['node:module/events', ...['on', 'listeners', 'listenerCount', 'once', 'off'].map(name => api('events', `class/EventEmitter/method/${name}`))],
    [], ['test-event-emitter-remove-listeners.js']],
  ['loader/commonjs-relative-and-cache', 'cjs-cache.js', [loader('commonjs-relative'), loader('commonjs-cache')],
    ['modules/cached.js'], ['test-require-cache.js']],
  ['loader/esm-relative-and-package-type', 'esm-relative.mjs', [loader('esm-relative'), loader('package-type')],
    ['modules/typed/package.json', 'modules/typed/value.js', 'modules/value.mjs'],
    ['es-module/test-esm-basic-imports.mjs', 'es-module/test-esm-type-field.mjs']],
  ['loader/exports-require', 'packages/consumer/require.cjs', [loader('package-exports-conditions')],
    conditional, ['es-module/test-esm-exports.mjs']],
  ['loader/exports-import', 'packages/consumer/import.mjs', [loader('package-exports-conditions')],
    conditional, ['es-module/test-esm-exports.mjs']],
  ['loader/package-imports', 'packages/consumer/imports.cjs', [loader('package-imports')],
    ['packages/consumer/package.json', 'packages/consumer/local.cjs'], ['es-module/test-esm-imports.mjs']],
  ['loader/transitive-dependency', 'packages/consumer/transitive.cjs', [loader('transitive-dependency')],
    ['packages/consumer/package.json', ...['outer/package.json', 'outer/index.js', 'outer/node_modules/nested/package.json',
      'outer/node_modules/nested/index.js', 'nested/package.json', 'nested/index.js']
      .map(file => `packages/consumer/node_modules/${file}`)], ['es-module/test-esm-package-map-basic.mjs']],
  ['loader/resolution-errors', 'packages/consumer/missing.cjs', [loader('unresolved-request'), loader('package-exports-conditions')],
    conditional, ['test-module-loading-error.js', 'es-module/test-esm-exports.mjs']]
];

function buildCorpus(root = directory) {
  const sourceInventory = JSON.parse(fs.readFileSync(path.join(root, 'upstream-inventory.json')));
  if (sourceInventory.reviewedForReferenceProfileSha256 !== hashJson(baseline.reference)) {
    throw new Error('Corpus applicability must be reviewed explicitly before changing the reference profile.');
  }
  const cases = definitions.map(([name, entry, targets, dependencies]) => ({
    id: `node:scenario/public-api/${name}`, targets,
    applicability: { state: 'applicable', reason: 'Reviewed for the pinned Linux x64 profile; deterministic local public-API behavior, no external services.' },
    entry, files: [...new Set([entry, 'harness.js', ...dependencies, 'corpus.json'])].sort(),
    expectedExitCode: 0, expectedStdout: '', expectedStderr: '',
    timeoutMs: 10000, compileTimeoutMs: 60000,
    timeoutRationale: 'Small deterministic local fixture with startup/JIT allowance; not a performance measurement.',
    normalization: 'exact',
    observables: [
      { path: 'result.json', format: 'json' },
      ...(name === 'buffer/encodings' ? [{ path: 'effect.bin', format: 'bytes' }] : []),
      ...(name === 'events/synchronous-order' ? [{
        path: 'order.json', format: 'event-order', order: [['before', 'prepended'], ['prepended', 'normal-9'], ['normal-9', 'after']]
      }] : [])
    ]
  }));
  const manifest = {
    schemaVersion: 1, suiteId: baseline.suiteId, artifactKind: 'scenario-manifest',
    scopeId: baseline.scopeId, corpusId: 'node-public-api-v1',
    referenceProfileSha256: hashJson(baseline.reference), corpusMetadata: 'corpus.json', cases
  };
  const fixtures = [...new Set(cases.flatMap(item => item.files))].filter(file => file !== 'corpus.json').sort().map(file => {
    const bytes = fs.readFileSync(path.join(root, file));
    return { path: file, bytes: bytes.length, sha256: sha256(bytes) };
  });
  const metadata = {
    schemaVersion: 1, suiteId: baseline.suiteId, artifactKind: 'behavior-corpus', scopeId: baseline.scopeId,
    corpusId: manifest.corpusId, referenceProfileSha256: manifest.referenceProfileSha256,
    selection: `Bounded first slice: ${cases.length} original scenarios plus ${sourceInventory.files.length} inspected upstream leads, not a full Node test inventory or upstream test-suite port.`,
    upstream: sourceInventory,
    fixtures,
    dependencies: { networkDuringExecution: false, npmInstallRequired: false, harness: ['harness.js'], packages: 'Original vendored resolution fixtures only; package.json files and dependency bytes are included in fixtures.' },
    scenarios: cases.map((scenario, index) => ({
      id: scenario.id, targets: scenario.targets, scenarioSha256: hashJson(scenario),
      origin: {
        kind: 'original',
        license: 'Apache-2.0',
        rationale: 'Independent assertions and inputs for documented public API behavior. Related upstream files are discovery leads, not copied/adapted tests; no claim that their full assertions or coverage were ported.',
        relatedUpstreamPaths: definitions[index][4].map(upstream)
      }
    }))
  };
  validateArtifact(manifest, 'scenario-manifest');
  validateArtifact(metadata, 'behavior-corpus');
  const metadataBytes = Buffer.from(JSON.stringify(metadata));
  const inventory = JSON.parse(fs.readFileSync(path.resolve(__dirname, '../../docs/nodejs/compatibility/inventory.v1.json')));
  validateCorpus(manifest, file => file === manifest.corpusMetadata ? metadataBytes : fs.readFileSync(path.join(root, file)), inventory);
  return { manifest, metadata };
}

function main(args) {
  if (args.some(arg => arg !== '--check')) throw new Error('Usage: node buildBehaviorCorpus.js [--check]');
  const { manifest, metadata } = buildCorpus();
  for (const [name, artifact] of [['manifest.json', manifest], ['corpus.json', metadata]]) {
    const file = path.join(directory, name);
    const bytes = `${JSON.stringify(artifact, null, 2)}\n`;
    if (args.includes('--check')) {
      if (fs.readFileSync(file, 'utf8') !== bytes) throw new Error(`Behavior corpus ${name} is stale; regenerate it.`);
    } else fs.writeFileSync(file, bytes);
  }
  console.log(`Behavior corpus: ${manifest.cases.length} original scenarios, ${metadata.upstream.files.length} inspected upstream leads.`);
}

if (require.main === module) {
  try { main(process.argv.slice(2)); } catch (failure) { console.error(failure.message); process.exitCode = 1; }
}

module.exports = { directory, buildCorpus };
