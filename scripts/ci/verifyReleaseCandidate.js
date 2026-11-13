'use strict';

const fs = require('node:fs');
const path = require('node:path');
const { createHash } = require('node:crypto');

function verifyReleaseCandidate(root, expectedSource, expectedVersion) {
  if (!root || !/^[a-f0-9]{40}$/.test(expectedSource || '') ||
      !/^[0-9]+\.[0-9]+\.[0-9]+([-+][0-9A-Za-z.-]+)?$/.test(expectedVersion || '')) {
    throw new Error('Candidate verification requires an artifact directory, exact source SHA, and release package version.');
  }
  const read = file => fs.readFileSync(path.join(root, file), 'utf8').trim();
  const source = read('source-sha.txt');
  const version = read('package-version.txt');
  const sdk = read('build-sdk.txt');
  if (source !== expectedSource) {
    throw new Error(`Candidate source ${source} does not match release source ${expectedSource}.`);
  }
  if (version !== expectedVersion) {
    throw new Error(`Candidate version ${version} does not match release version ${expectedVersion}.`);
  }
  if (!/^11\.0\.[0-9]+$/.test(sdk)) {
    throw new Error(`Candidate must be built with stable SDK 11, found ${sdk}.`);
  }
  const hashes = new Map();
  for (const line of read('packages.sha256').split(/\r?\n/)) {
    const match = /^([a-f0-9]{64})  (.+)$/.exec(line);
    if (!match || hashes.has(match[2])) {
      throw new Error('Candidate hash manifest contains an invalid or duplicate entry.');
    }
    hashes.set(match[2], match[1]);
  }
  const packages = ['jroc', 'Jroc.Core', 'Jroc.Runtime', 'Jroc.SDK'].map(id => `${id}.${version}.nupkg`);
  for (const name of packages) {
    const hash = createHash('sha256').update(fs.readFileSync(path.join(root, 'feed', name))).digest('hex');
    if (hashes.get(name) !== hash) {
      throw new Error(`Candidate package ${name} does not match its validated SHA-256 manifest.`);
    }
  }
  return { source, version, sdk, packages };
}

if (require.main === module) {
  const candidate = verifyReleaseCandidate(process.argv[2], process.env.RELEASE_SOURCE_SHA, process.env.VERSION);
  console.log(`Verified SDK ${candidate.sdk} candidate ${candidate.version} from ${candidate.source}.`);
}

module.exports = { verifyReleaseCandidate };
