'use strict';

// Keep catalog discovery and execution on the MVP runner's metadata/source semantics.
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const { execFileSync } = require('node:child_process');
const bootstrap = require('./bootstrap');
const { parseTest262File, parseTest262Metadata } = require('./metadataParser');
const { determineVariants, evaluateCase, matchPathExclusion } = require('./runMvp');

const SPECIFICATION_KEYS = ['esid', 'es5id', 'es6id'];

function specificationIdentifiers(frontmatter) {
  // Only the supervisor reads references; staged fixture runtimes need no YAML package.
  const { isMap, isScalar, parseDocument } = require('yaml');
  const document = parseDocument(frontmatter, {
    version: '1.2', schema: 'core', uniqueKeys: false, intAsBigInt: true,
  });
  const diagnostics = document.errors.concat(document.warnings);
  if (diagnostics.length) throw new Error(diagnostics.map(d => d.message).join('\n'));
  if (document.contents === null) return { identifiers: {}, occurrences: 0 };
  if (!isMap(document.contents)) throw new Error('Specification frontmatter must be a YAML mapping.');
  const parsed = document.toJS({ maxAliasCount: 0 });
  const identifiers = Object.fromEntries(SPECIFICATION_KEYS
    .filter(key => Object.hasOwn(parsed, key)).map(key => [key, parsed[key]]));
  const json = JSON.stringify(identifiers, (key, value) => {
    if (typeof value === 'bigint') {
      const number = Number(value);
      if (!Number.isSafeInteger(number)) {
        throw new Error('YAML specification integers cannot be represented losslessly in JSON.');
      }
      return number;
    }
    if (typeof value === 'number' && !Number.isFinite(value)) {
      throw new Error('Non-finite YAML specification identifiers cannot be represented in JSON.');
    }
    return value;
  });
  const occurrences = document.contents.items.filter(pair =>
    isScalar(pair.key) && SPECIFICATION_KEYS.includes(pair.key.value)).length;
  return { identifiers: JSON.parse(json), occurrences };
}

function specificationMetadata(source, metadata, diagnostic) {
  const start = source.indexOf('/*---');
  const end = start < 0 ? -1 : source.indexOf('---*/', start + 5);
  const rawFrontmatter = end < 0 ? null : source.substring(start + 5, end);
  let rawIdentifiers = {};
  let identifierOccurrences = 0;
  if (metadata && metadata.hasFrontmatter) {
    try {
      const parsed = specificationIdentifiers(rawFrontmatter);
      rawIdentifiers = parsed.identifiers;
      identifierOccurrences = parsed.occurrences;
    } catch (error) {
      if (error.code === 'MODULE_NOT_FOUND') throw error;
      diagnostic = 'Specification YAML metadata: ' + error.message;
    }
  }
  const keys = Object.keys(rawIdentifiers);
  let state = 'unresolved';
  if (metadata && metadata.hasFrontmatter && !diagnostic) {
    if (keys.length === 0) state = 'absent';
    else if (keys.length > 1 || identifierOccurrences > keys.length || keys.some(key =>
      typeof rawIdentifiers[key] !== 'string' || rawIdentifiers[key].trim() === '')) state = 'ambiguous';
    else if (keys[0] !== 'esid') state = 'legacy';
  }
  return {
    description: metadata ? metadata.description : null,
    info: metadata ? metadata.info : null,
    author: metadata ? metadata.author : null,
    esid: rawIdentifiers.esid ?? null,
    es5id: rawIdentifiers.es5id ?? null,
    es6id: rawIdentifiers.es6id ?? null,
    specification: {
      schema_version: 1,
      raw_identifiers: rawIdentifiers,
      raw_frontmatter: rawFrontmatter,
      reference_state: state,
      ...(diagnostic ? { diagnostic } : {}),
    },
  };
}

function classify(root, relativePath, pin) {
  const absolutePath = path.join(root, relativePath);
  const bytes = fs.readFileSync(absolutePath);
  const source = bytes.toString('utf8');
  const sha256 = crypto.createHash('sha256').update(bytes).digest('hex');
  try {
    const metadata = parseTest262Metadata(source, { filePath: absolutePath });
    const exclusion = matchPathExclusion(relativePath, pin);
    const reasons = metadata.unsupported.concat(metadata.mvpBlockers);
    if (!metadata.hasFrontmatter) reasons.push({ code: 'missing-frontmatter' });
    if (exclusion) reasons.push({ code: 'skipped-by-policy', pattern: exclusion });
    const variants = metadata.execution.strictMode === 'module'
      ? ['module']
      : determineVariants(metadata.execution, null, relativePath);
    return { path: relativePath, sha256, variants,
      state: reasons.length ? 'blocked' : 'runnable', reasons,
      metadata: { flags: metadata.flags, includes: metadata.includes, features: metadata.features,
        negative: metadata.negative, execution: metadata.execution,
        ...specificationMetadata(source, metadata) },
      isSupportFile: /(?:^|\/)(?:_[^/]*|[^/]*_FIXTURE)\.js$/i.test(relativePath) };
  } catch (error) {
    if (error.code === 'MODULE_NOT_FOUND') throw error;
    return { path: relativePath, sha256, variants: [], state: 'metadata-error',
      metadata: specificationMetadata(source, null, error.message),
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
      compilationCoverage: request.compilationCoverage === true,
    });
  } finally {
    // The caller allocates this directory exclusively for this one variant.
    fs.rmSync(output, { recursive: true, force: true });
  }
}

if (require.main === module) {
  const request = JSON.parse(fs.readFileSync(0, 'utf8'));
  const pin = bootstrap.loadPin(bootstrap.defaultPinPath());
  let result;
  if (request.command === 'inventory') result = inventory(request.root, pin);
  else if (request.command === 'metadata') {
    if (!Array.isArray(request.paths) || request.paths.length > 250 ||
        request.paths.some(p => typeof p !== 'string' || p === '' ||
          path.isAbsolute(p) || p.includes('\\') || p.split('/').some(part => part === '..' || part === ''))) {
      throw new Error('Metadata requests require at most 250 relative fixture paths.');
    }
    result = request.paths.map(p => classify(request.root, p, pin));
  } else result = run(request);
  process.stdout.write(JSON.stringify(result));
}

module.exports = { classify, inventory };
