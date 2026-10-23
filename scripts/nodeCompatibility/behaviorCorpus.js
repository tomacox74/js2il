'use strict';

const { hashJson, sha256, validateArtifact } = require('./artifacts');

function validateCorpus(manifest, readInput, inventory) {
  if (!manifest.corpusMetadata) return null;
  const metadata = JSON.parse(new TextDecoder('utf-8', { fatal: true }).decode(readInput(manifest.corpusMetadata)));
  validateArtifact(metadata, 'behavior-corpus');
  if (metadata.suiteId !== manifest.suiteId || metadata.scopeId !== manifest.scopeId
    || metadata.corpusId !== manifest.corpusId || metadata.referenceProfileSha256 !== manifest.referenceProfileSha256
    || metadata.upstream.sourceRevision !== inventory.reference.sourceRevision
    || metadata.upstream.reviewedForReferenceProfileSha256 !== manifest.referenceProfileSha256) {
    throw new Error('Behavior corpus identities/profile/source pin do not reconcile.');
  }
  const targets = new Set([...inventory.entries, ...inventory.behaviors].map(entry => entry.id));
  if (metadata.scenarios.length !== manifest.cases.length) throw new Error('Corpus scenario inventory does not reconcile.');
  for (const scenario of manifest.cases) {
    const origin = metadata.scenarios.find(item => item.id === scenario.id);
    if (!origin || origin.scenarioSha256 !== hashJson(scenario) || hashJson(origin.targets) !== hashJson(scenario.targets)
      || scenario.targets.some(target => !targets.has(target))) {
      throw new Error('Corpus scenario/target/policy mapping does not reconcile.');
    }
  }
  const expectedFiles = [...new Set(manifest.cases.flatMap(scenario => scenario.files))]
    .filter(file => file !== manifest.corpusMetadata).sort();
  if (hashJson(expectedFiles) !== hashJson(metadata.fixtures.map(file => file.path).sort())) {
    throw new Error('Corpus fixture/dependency inventory does not reconcile.');
  }
  for (const fixture of metadata.fixtures) {
    const bytes = readInput(fixture.path);
    if (bytes.length !== fixture.bytes || sha256(bytes) !== fixture.sha256) throw new Error(`Corpus fixture hash/size mismatch: ${fixture.path}.`);
  }
  return metadata;
}

async function verifyUpstream(metadata, readSource = async url => {
  const response = await fetch(url, { signal: AbortSignal.timeout(30000) });
  if (!response.ok) throw new Error(`Upstream HTTP ${response.status}: ${url}`);
  const chunks = [];
  let length = 0;
  for await (const chunk of response.body) {
    length += chunk.length;
    if (length > 1024 * 1024) throw new Error('Upstream source exceeds the 1 MiB inspection budget.');
    chunks.push(chunk);
  }
  return Buffer.concat(chunks);
}) {
  validateArtifact(metadata, 'behavior-corpus');
  for (const file of [metadata.upstream.license, ...metadata.upstream.files]) {
    const bytes = await readSource(file.url);
    if (bytes.length !== file.bytes || sha256(bytes) !== file.sha256) throw new Error(`Upstream source hash/size mismatch: ${file.path}.`);
  }
}

module.exports = { validateCorpus, verifyUpstream };

if (require.main === module) {
  (async () => {
    if (process.argv.slice(2).join(' ') !== '--verify-upstream') throw new Error('Usage: node behaviorCorpus.js --verify-upstream');
    const { metadata } = require('./buildBehaviorCorpus').buildCorpus();
    await verifyUpstream(metadata);
    console.log(`Verified ${metadata.upstream.files.length} pinned source leads and upstream license.`);
  })().catch(failure => { console.error(failure.message); process.exitCode = 1; });
}
