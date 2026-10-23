'use strict';

const crypto = require('node:crypto');
const Ajv = require('ajv');
const schema = require('./artifact.schema.json');

const ajv = new Ajv({ allErrors: true });
ajv.addFormat('date-time', value => {
  const timestamp = Date.parse(value);
  return Number.isFinite(timestamp) && new Date(timestamp).toISOString() === value;
});
ajv.addSchema(schema);
const validators = {
  'api-inventory': ajv.compile({ $ref: `${schema.$id}#/definitions/inventory` }),
  'reference-environment': ajv.compile({ $ref: `${schema.$id}#/definitions/environment` })
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
  } else {
    assertUnique(artifact.lockfiles, 'path', 'lockfile');
    if (artifact.referenceEnvironmentEligible !== !artifact.worktreeDirty) {
      throw new Error('Reference environment eligibility contradicts working-tree provenance.');
    }
    if (artifact.nodeVersions.node !== artifact.nodeVersion) {
      throw new Error('Node component version contradicts reference environment version.');
    }
  }
}

module.exports = { sha256, hashJson, inventoryDigests, validateArtifact };
