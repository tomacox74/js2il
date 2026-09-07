'use strict';

// Keep catalog discovery and execution on the MVP runner's metadata/source semantics.
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const { execFileSync } = require('node:child_process');
const bootstrap = require('./bootstrap');
const { parseTest262File } = require('./metadataParser');
const { determineVariants, evaluateCase, matchPathExclusion } = require('./runMvp');

function classify(root, relativePath, pin) {
  const absolutePath = path.join(root, relativePath);
  const sha256 = crypto.createHash('sha256').update(fs.readFileSync(absolutePath)).digest('hex');
  try {
    const metadata = parseTest262File(absolutePath);
    const exclusion = matchPathExclusion(relativePath, pin);
    const reasons = metadata.unsupported.concat(metadata.mvpBlockers);
    if (!metadata.hasFrontmatter) reasons.push({ code: 'missing-frontmatter' });
    if (exclusion) reasons.push({ code: 'skipped-by-policy', pattern: exclusion });
    const variants = metadata.execution.strictMode === 'module'
      ? ['module']
      : determineVariants(metadata.execution, null, relativePath);
    return { path: relativePath, sha256, variants,
      state: reasons.length ? 'blocked' : 'runnable', reasons };
  } catch (error) {
    return { path: relativePath, sha256, variants: [], state: 'metadata-error',
      reasons: [{ code: 'metadata-error', reason: error.message }] };
  }
}

function inventory(root, pin) {
  const revision = execFileSync('git', ['-C', root, 'rev-parse', 'HEAD'], { encoding: 'utf8' }).trim();
  if (revision !== pin.upstream.commit) throw new Error('Catalog requires the pinned upstream revision.');
  const paths = execFileSync('git', ['-C', root, 'ls-tree', '-r', '--name-only', 'HEAD', 'test'],
    { encoding: 'utf8', maxBuffer: 32 * 1024 * 1024 }).trim().split('\n')
    .filter(p => p.endsWith('.js')).sort();
  if (!paths.length) throw new Error('Empty upstream inventory.');
  // Missing sparse-checkout files are fatal: never silently report a partial inventory.
  return paths.map(p => classify(root, p, pin));
}

function run(request) {
  const { root, output, fixture, variant, jroc, timeout, compileTimeout } = request;
  const metadata = parseTest262File(path.join(root, fixture));
  try {
    return evaluateCase(root, output, {
      relativePath: fixture, absolutePath: path.join(root, fixture), variant, metadata,
    }, { type: 'dll', path: jroc }, {
      timeoutSeconds: timeout, compileTimeoutSeconds: compileTimeout,
    });
  } finally {
    // The caller allocates this directory exclusively for this one variant.
    fs.rmSync(output, { recursive: true, force: true });
  }
}

if (require.main === module) {
  const request = JSON.parse(fs.readFileSync(0, 'utf8'));
  const pin = bootstrap.loadPin(bootstrap.defaultPinPath());
  const result = request.command === 'inventory' ? inventory(request.root, pin) : run(request);
  process.stdout.write(JSON.stringify(result));
}

module.exports = { classify, inventory };
