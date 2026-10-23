#!/usr/bin/env node
'use strict';

const fs = require('node:fs');
const path = require('node:path');
const config = require('./baseline.json');
const { sha256, inventoryDigests, validateArtifact } = require('./artifacts');

const repoRoot = path.resolve(__dirname, '../..');
const outputPath = path.join(repoRoot, 'docs/nodejs/compatibility/inventory.v1.json');
const defaultInput = path.join(repoRoot, 'artifacts/nodeCompatibility/node-v24.22.1-all.json');
const memberKinds = {
  methods: 'method', classMethods: 'static-method', properties: 'property',
  classes: 'class', events: 'event', ctors: 'constructor', globals: 'global'
};

function verifyDocumentation(bytes, baseline) {
  const actual = sha256(bytes);
  if (actual !== baseline.reference.documentationSha256) {
    throw new Error(`Official documentation SHA-256 mismatch: expected ${baseline.reference.documentationSha256}, received ${actual}.`);
  }
  return JSON.parse(bytes.toString('utf8'));
}

function readContractEvidence(root) {
  const directory = path.join(root, 'src/JavaScriptRuntime/Node/Contracts');
  const adapterFiles = fs.readdirSync(path.join(root, 'src/JavaScriptRuntime/Node'));
  const overrideDirectory = path.join(root, 'scripts/nodeContracts');
  const overrides = fs.readdirSync(overrideDirectory).filter(x => x.endsWith('.overrides.json')).sort()
    .map(file => ({
      path: `scripts/nodeContracts/${file}`,
      value: JSON.parse(fs.readFileSync(path.join(overrideDirectory, file), 'utf8'))
    }));
  return fs.readdirSync(directory).filter(x => x.endsWith('.Generated.cs')).sort().flatMap(file => {
    const source = `src/JavaScriptRuntime/Node/Contracts/${file}`;
    const bytes = fs.readFileSync(path.join(root, source));
    const text = bytes.toString('utf8');
    const identity = text.match(/\[NodeModule(Interface|Type)\("([^"]+)"(?:,\s*"([^"]+)")?\)\]/);
    if (!identity) return [];
    const members = [...new Set([...text.matchAll(/\[NodeModuleMember\("([^"]+)"\)\]/g)].map(x => x[1]))].sort();
    const module = identity[2];
    const version = text.match(/official Node\.js ([\d.]+)/);
    if (!version) throw new Error(`Missing Node documentation version in ${source}.`);
    const interfaceName = file.replace('.Generated.cs', '');
    const stem = interfaceName.replace(/^I/, '').replace(/Module$/, '');
    const overrideStem = stem[0].toLowerCase() + stem.slice(1);
    const related = overrides.flatMap(item => {
      if (identity[1] === 'Interface') {
        return item.path.endsWith(`/${overrideStem}.node24.overrides.json`)
          ? [{ ...item, implementation: item.value }] : [];
      }
      return (item.value.nestedContracts ?? []).filter(x => x.interfaceName === interfaceName)
        .map(value => ({ ...item, implementation: value }));
    });
    const adapter = adapterFiles.find(x => x.endsWith(`.${interfaceName}.Generated.cs`));
    return [{
      module, ...(identity[3] ? { type: identity[3] } : {}),
      source, sha256: sha256(bytes), nodeVersion: version[1], members,
      documentationUrl: text.match(/Source: (https:\/\/[^\s]+)/)?.[1],
      ...(adapter ? {
        adapterSource: `src/JavaScriptRuntime/Node/${adapter}`,
        adapterSha256: sha256(fs.readFileSync(path.join(root, 'src/JavaScriptRuntime/Node', adapter)))
      } : {}),
      implementations: [...new Set(related.flatMap(x => Object.keys(x.implementation.intrinsicImplementations ?? {})))].sort(),
      overrides: related.map(x => ({ path: x.path, sha256: sha256(fs.readFileSync(path.join(root, x.path))) }))
    }];
  });
}

function memberName(node, kind, hasOwner) {
  if (kind === 'property' || kind === 'global') {
    const code = node.textRaw?.match(/`([^`]+)`/);
    if (code) return code[1];
    if (node.name === 'Type') throw new Error(`Cannot recover property identity: ${node.textRaw}`);
    if (kind === 'global' && node.name === 'Process') return 'process';
  }
  if (hasOwner || kind === 'class' || kind === 'event') return node.name;
  const code = node.textRaw?.match(/`([^`]+)`/);
  return code ? code[1].replace(/^new\s+/, '').split('(')[0] : node.name;
}

function buildInventory(data, baseline, contracts) {
  const entries = new Map();
  const documents = new Map();
  const encode = value => encodeURIComponent(value).replace(/%2E/g, '.');
  const provenance = document => ({
    source: `doc/api/${document}.md`,
    documentationUrl: `https://nodejs.org/docs/v${baseline.reference.nodeVersion}/api/${document}.html`,
    sourceUrl: `https://github.com/nodejs/node/blob/${baseline.reference.sourceRevision}/doc/api/${document}.md`
  });

  function add(id, document, kind, name, heading, owner = null, stability = null, signatures = []) {
    if (!name) throw new Error(`Missing identity for ${id}.`);
    let entry = entries.get(id);
    if (!entry) {
      entry = { id, document, kind, name, ...(owner ? { owner } : {}), headings: [], stability: [] };
      entries.set(id, entry);
    }
    if (heading && !entry.headings.includes(heading)) entry.headings.push(heading);
    if (stability !== null && !entry.stability.includes(stability)) entry.stability.push(stability);
    for (const signature of signatures) {
      const shape = JSON.stringify({
        parameters: (signature.params ?? []).map(x => ({
          name: x.name, ...(x.type ? { type: x.type } : {}),
          ...(x.optional !== undefined ? { optional: x.optional } : {})
        })),
        ...(signature.return?.type ? { returns: signature.return.type } : {})
      });
      entry.signatureShapes ??= [];
      if (!entry.signatureShapes.includes(shape)) entry.signatureShapes.push(shape);
    }
    return entry;
  }

  function visit(node, document, owner = null, namespace = document) {
    for (const [collection, kind] of Object.entries(memberKinds)) {
      for (const child of node[collection] ?? []) {
        const name = memberName(child, kind, owner !== null);
        const id = `${owner ?? `node:api/${namespace}`}/${kind}/${encode(name)}`;
        const entry = add(id, document, kind, name, child.textRaw, owner,
          child.stability ?? node.stability ?? null, child.signatures);
        if (namespace !== document) entry.namespace = namespace;
        if (kind === 'class' && child.signatures?.length) {
          add(`${id}/constructor/new`, document, 'constructor', 'new', child.textRaw, id,
            child.stability ?? node.stability ?? null, child.signatures);
        }
        visit(child, document, id, namespace);
      }
    }
    for (const child of [...(node.modules ?? []), ...(node.miscs ?? [])]) {
      const global = document === 'globals' && /^`[^`]+`$/.test(child.textRaw ?? '');
      if (global) {
        const name = memberName(child, 'global', false);
        const id = `node:api/globals/global/${encode(name)}`;
        add(id, document, 'global', name, child.textRaw, null, child.stability ?? null);
        visit(child, document, id, namespace);
      } else {
        documents.get(document).sections.add(child.textRaw ?? child.name);
        visit(child, document, owner, baseline.sectionNamespaces?.[document]?.[child.name] ?? namespace);
      }
    }
  }

  for (const collection of ['modules', 'miscs', 'classes', 'globals', 'methods']) {
    for (const root of data[collection] ?? []) {
      if (!/^doc\/api\/[^/]+\.md$/.test(root.source ?? '')) {
        throw new Error(`Missing official source path for root ${root.name}.`);
      }
      const document = path.basename(root.source, '.md');
      if (!documents.has(document)) {
        documents.set(document, {
          id: document, ...provenance(document),
          scope: baseline.excludedDocuments[document] ? 'excluded' : 'included',
          ...(baseline.excludedDocuments[document] ? { reason: baseline.excludedDocuments[document] } : {}),
          sections: new Set()
        });
      }
      if (baseline.excludedDocuments[document]) continue;
      if (memberKinds[collection]) visit({ [collection]: [root] }, document);
      else visit(root, document);
    }
  }

  for (const specifier of baseline.builtinModules) {
    const name = specifier.replace(/^node:/, '');
    const document = baseline.builtinDocumentation[name] ?? name.split('/')[0];
    add(`node:module/${name}`, document, 'module', `node:${name}`, `node:${name}`);
  }

  for (const entry of entries.values()) {
    entry.headings.sort();
    entry.stability.sort();
    entry.signatureShapes?.sort();
    const matching = contracts.filter(contract => {
      if (entry.kind === 'module') return `node:${contract.module}` === entry.name && !contract.type;
      const owner = entries.get(entry.owner);
      if (owner?.kind === 'class') {
        return !!contract.type && contract.documentationUrl?.endsWith(`/${entry.document}.json`)
          && owner.name.split('.').at(-1) === contract.type
          && contract.members.includes(entry.name);
      }
      if (contract.type || owner && owner.kind !== 'global') return false;
      const document = baseline.builtinDocumentation[contract.module] ?? contract.module.split('/')[0];
      const prefix = owner?.name ?? (entry.name.includes('.') ? entry.name.slice(0, entry.name.lastIndexOf('.')) : document);
      return document === entry.document && prefix === contract.module
        && contract.members.includes(entry.name.split('.').at(-1));
    });
    if (matching.length) {
      entry.contracts = matching.map(contract => ({
        source: contract.source,
        ...(entry.kind !== 'module' ? {
          adapter: contract.implementations.includes(entry.name.split('.').at(-1))
            ? 'configured-implementation' : contract.adapterSource ? 'generated-stub' : 'unreviewed'
        } : {})
      }));
    }
  }

  const behaviors = baseline.loaderBehaviors.map(item => ({
    id: `node:behavior/loader/${item.key}`, document: item.document,
    expectation: item.expectation, status: 'not-run',
    applicability: { platform: 'all-reference-platforms', feature: 'public-module-loading' }
  })).sort((a, b) => a.id.localeCompare(b.id, 'en'));
  const inventory = {
    schemaVersion: baseline.schemaVersion, suiteId: baseline.suiteId,
    artifactKind: 'api-inventory', scopeId: baseline.scopeId,
    reference: baseline.reference, seedJrocRevision: baseline.seedJrocRevision,
    defaults: {
      availability: 'unverified', behavior: 'not-run',
      applicability: { platform: 'review-required', feature: 'review-required' }
    },
    limitations: [
      'This is a documentation-derived candidate inventory, not a complete semantic specification or a compatibility result.',
      'Overload headings form one API family; optional forms, options, inherited APIs and external web specifications require separately registered behavior cases.',
      'Documentation sections are retained for applicability and extraction review, not counted as executable APIs.',
      'Contract evidence is a snapshot of declarations/configuration, not proof of JavaScript accessibility or complete implementation.',
      'No executable behavior corpus or compatibility result is delivered by this inventory; loader behavior registrations are pending execution.'
    ],
    outcomes: baseline.outcomes,
    contracts,
    documents: [...documents.values()].map(x => ({ ...x, sections: [...x.sections].sort() }))
      .sort((a, b) => a.id.localeCompare(b.id, 'en')),
    entries: [...entries.values()].sort((a, b) => a.id.localeCompare(b.id, 'en')),
    behaviors
  };
  return { ...inventory, ...inventoryDigests(inventory) };
}

async function main(args) {
  const accepted = new Set(['--check', '--input', '--download']);
  for (let i = 0; i < args.length; i++) {
    if (!accepted.has(args[i])) throw new Error(`Unknown argument ${args[i]}.`);
    if (args[i] === '--input' && !args[++i]) throw new Error('--input requires a path.');
  }
  const inputIndex = args.indexOf('--input');
  const input = inputIndex < 0 ? defaultInput : path.resolve(args[inputIndex + 1]);
  if (args.includes('--download')) {
    const response = await fetch(config.reference.documentationUrl, { signal: AbortSignal.timeout(60000) });
    if (!response.ok) throw new Error(`Official documentation download failed: HTTP ${response.status}.`);
    const bytes = Buffer.from(await response.arrayBuffer());
    verifyDocumentation(bytes, config);
    fs.mkdirSync(path.dirname(input), { recursive: true });
    fs.writeFileSync(input, bytes);
  }
  const data = verifyDocumentation(fs.readFileSync(input), config);
  const manifest = buildInventory(data, config, readContractEvidence(repoRoot));
  validateArtifact(manifest, 'api-inventory');
  const output = `${JSON.stringify(manifest, null, 2)}\n`;
  if (args.includes('--check')) {
    if (fs.readFileSync(outputPath, 'utf8') !== output) {
      throw new Error('Compatibility inventory is stale. Run npm run generate:node-compatibility.');
    }
    console.log(`Compatibility inventory is current (${manifest.entries.length} API candidates; ${manifest.behaviors.length} unexecuted behavior registrations).`);
  } else {
    fs.mkdirSync(path.dirname(outputPath), { recursive: true });
    fs.writeFileSync(outputPath, output);
    console.log(`Generated ${path.relative(repoRoot, outputPath)} (${manifest.entries.length} API candidates).`);
  }
}

if (require.main === module) {
  main(process.argv.slice(2)).catch(error => {
    console.error(error.message);
    process.exitCode = 1;
  });
}

module.exports = { buildInventory, readContractEvidence, verifyDocumentation, sha256 };
