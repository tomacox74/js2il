'use strict';

const crypto = require('node:crypto');
const Ajv = require('ajv');
const schema = require('./artifact.schema.json');
const executionSchema = require('./execution.schema.json');
const behaviorSchema = require('./behavior.schema.json');

const ajv = new Ajv({ allErrors: true });
ajv.addFormat('date-time', value => {
  const timestamp = Date.parse(value);
  return Number.isFinite(timestamp) && new Date(timestamp).toISOString() === value;
});
ajv.addSchema(schema);
ajv.addSchema(executionSchema);
ajv.addSchema(behaviorSchema);
const validators = {
  'api-inventory': ajv.compile({ $ref: `${schema.$id}#/definitions/inventory` }),
  'reference-environment': ajv.compile({ $ref: `${schema.$id}#/definitions/environment` }),
  'scenario-manifest': ajv.compile({ $ref: `${executionSchema.$id}#/definitions/manifest` }),
  'execution-result': ajv.compile({ $ref: `${executionSchema.$id}#/definitions/result` }),
  'execution-bundle': ajv.compile({ $ref: `${executionSchema.$id}#/definitions/bundle` }),
  'behavior-corpus': ajv.compile({ $ref: behaviorSchema.$id })
};

function sha256(bytes) {
  return crypto.createHash('sha256').update(bytes).digest('hex');
}

function hashJson(value) {
  return sha256(JSON.stringify(value, (_key, item) => {
    if (item === undefined || typeof item === 'number' && !Number.isFinite(item)) {
      throw new Error('Cannot hash undefined or non-finite JSON values.');
    }
    return item && typeof item === 'object' && !Array.isArray(item)
      ? Object.fromEntries(Object.keys(item).sort().map(key => [key, item[key]]))
      : item;
  }));
}

function inventoryDigests(inventory) {
  return {
    referenceProfileSha256: hashJson(inventory.reference),
    targetInventorySha256: hashJson({
      suiteId: inventory.suiteId,
      scopeId: inventory.scopeId,
      applicability: inventory.defaults.applicability,
      documents: inventory.documents.map(({ documentationUrl, sourceUrl, ...document }) => document),
      entries: inventory.entries.map(({ contracts, ...entry }) => entry),
      behaviors: inventory.behaviors.map(({ status, ...behavior }) => behavior)
    }),
    implementationEvidenceSha256: hashJson({
      suiteId: inventory.suiteId,
      contracts: inventory.contracts,
      associations: inventory.entries.filter(entry => entry.contracts)
        .map(entry => ({ id: entry.id, contracts: entry.contracts }))
    })
  };
}

function assertUnique(items, key, label) {
  const values = items.map(item => item[key]);
  if (new Set(values).size !== values.length) throw new Error(`Duplicate ${label} ${key}.`);
}

function validateArtifact(artifact, kind) {
  const validator = validators[kind];
  if (!validator) throw new Error(`Unknown artifact kind ${kind}.`);
  if (!validator(artifact)) {
    throw new Error(`Invalid ${kind} artifact: ${ajv.errorsText(validator.errors, { separator: '; ' })}`);
  }
  if (kind === 'api-inventory') {
    assertUnique(artifact.entries, 'id', 'API');
    assertUnique(artifact.behaviors, 'id', 'behavior');
    assertUnique(artifact.documents, 'id', 'document');
    assertUnique(artifact.contracts, 'source', 'contract');
    const entries = new Set(artifact.entries.map(entry => entry.id));
    const sources = new Set(artifact.contracts.map(contract => contract.source));
    for (const entry of artifact.entries) {
      if (entry.owner && !entries.has(entry.owner)) throw new Error(`Unknown API owner ${entry.owner}.`);
      for (const contract of entry.contracts ?? []) {
        if (!sources.has(contract.source)) throw new Error(`Unknown contract source ${contract.source}.`);
      }
    }
    for (const [key, expected] of Object.entries(inventoryDigests(artifact))) {
      if (artifact[key] !== expected) throw new Error(`Inventory ${key} mismatch.`);
    }
  } else if (kind === 'reference-environment') {
    assertUnique(artifact.lockfiles, 'path', 'lockfile');
    if (artifact.referenceEnvironmentEligible !== !artifact.worktreeDirty) {
      throw new Error('Reference environment eligibility contradicts working-tree provenance.');
    }
    if (artifact.nodeVersions.node !== artifact.nodeVersion) {
      throw new Error('Node component version contradicts reference environment version.');
    }
  } else if (kind === 'scenario-manifest') {
    assertUnique(artifact.cases, 'id', 'scenario');
    for (const scenario of artifact.cases) {
      if (!scenario.files.includes(scenario.entry)) throw new Error(`Entry is absent from files for ${scenario.id}.`);
      if (artifact.corpusMetadata && !scenario.files.includes(artifact.corpusMetadata)) throw new Error('Every scenario must retain corpus metadata.');
      assertUnique(scenario.observables, 'path', 'observable');
      for (const observable of scenario.observables) {
        if (scenario.files.includes(observable.path)) throw new Error('An observable cannot overwrite an input file.');
        if (observable.order?.some(([before, after]) => before === after)) throw new Error('An event cannot precede itself.');
      }
    }
  } else if (kind === 'behavior-corpus') {
    assertUnique(artifact.fixtures, 'path', 'fixture');
    assertUnique(artifact.scenarios, 'id', 'scenario');
    assertUnique(artifact.upstream.files, 'path', 'upstream file');
    const sources = new Set(artifact.upstream.files.map(file => file.path));
    const prefix = `https://raw.githubusercontent.com/nodejs/node/${artifact.upstream.sourceRevision}/`;
    for (const file of [artifact.upstream.license, ...artifact.upstream.files]) {
      if (file.url !== prefix + file.path) throw new Error('Upstream URL must bind the exact source revision and path.');
    }
    for (const file of artifact.upstream.files) {
      if (file.classification === 'runnable'
        && (file.executionEligibility !== 'eligible' || file.applicability.state !== 'applicable')) {
        throw new Error('Runnable upstream leads require reviewed applicability and execution eligibility.');
      }
      if (file.classification === 'excluded'
        && (file.applicability.state !== 'excluded' || file.executionEligibility !== 'outside-profile')) {
        throw new Error('Upstream exclusions must preserve separate applicability and eligibility reasons.');
      }
    }
    for (const scenario of artifact.scenarios) {
      if (scenario.origin.relatedUpstreamPaths.some(file => !sources.has(file))) throw new Error('Unknown upstream discovery lead.');
    }
    for (const file of artifact.dependencies.harness) {
      if (!artifact.fixtures.some(fixture => fixture.path === file)) throw new Error('Missing harness dependency.');
    }
  } else if (kind === 'execution-result') {
    if (artifact.policySha256 !== hashJson(artifact.policy)) throw new Error('Comparison policy digest mismatch.');
    if (artifact.scenarioId !== artifact.policy.scenario.id
      || hashJson(artifact.targets) !== hashJson(artifact.policy.scenario.targets)) {
      throw new Error('Result scenario identity contradicts comparison policy.');
    }
    if (artifact.policy.scenario.applicability.state !== 'applicable'
      && !['not-run', 'infrastructure-failure'].includes(artifact.outcome)) {
      throw new Error('Unknown/excluded applicability cannot establish a product outcome.');
    }
    if (artifact.policy.scenario.applicability.state !== 'applicable'
      && [artifact.reference, artifact.compilation, artifact.execution].some(observation => observation !== null)) {
      throw new Error('Unknown/excluded scenarios must not execute fixture processes.');
    }
    if (['pass', 'mismatch', 'compile-failure', 'unsupported', 'execution-timeout'].includes(artifact.outcome)
      && (!artifact.reference || artifact.reference.state !== 'completed'
        || artifact.reference.exitCode !== artifact.policy.scenario.expectedExitCode)) {
      throw new Error('Product outcome requires a completed valid reference.');
    }
    if (artifact.outcome === 'pass'
      && (!artifact.compilation || artifact.compilation.state !== 'completed' || artifact.compilation.exitCode !== 0
        || !artifact.execution || artifact.execution.state !== 'completed'
        || artifact.execution.exitCode !== artifact.reference.exitCode || !artifact.compiledArtifacts.length || !artifact.coverageReport)) {
      throw new Error('Pass requires successful compilation and completed paired execution.');
    }
    if (['pass', 'mismatch', 'compile-failure', 'unsupported', 'execution-timeout'].includes(artifact.outcome)) {
      const expected = artifact.policy.scenario.observables;
      if (artifact.referenceObservables.length !== expected.length
        || artifact.referenceObservables.some((item, index) => item.error || !item.artifact
          || item.path !== expected[index].path || item.format !== expected[index].format)) {
        throw new Error('Product outcome requires complete valid reference observables.');
      }
      if (artifact.outcome === 'pass' && (artifact.jrocObservables.length !== expected.length
        || artifact.jrocObservables.some((item, index) => item.error || !item.artifact
          || item.path !== expected[index].path || item.format !== expected[index].format))) {
        throw new Error('Pass requires complete paired observables.');
      }
    }
  } else if (kind === 'execution-bundle') {
    assertUnique(artifact.files, 'path', 'bundle artifact');
    assertUnique(artifact.results, 'path', 'result');
    assertUnique(artifact.cases, 'id', 'scenario');
  }
}

module.exports = { sha256, hashJson, inventoryDigests, validateArtifact };
