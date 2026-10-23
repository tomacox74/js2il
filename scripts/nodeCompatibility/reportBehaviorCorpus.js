'use strict';

const fs = require('node:fs');
const path = require('node:path');
const { verifyBundle, readBounded } = require('../differential-test/compatibility');
const { validateCorpus } = require('./behaviorCorpus');
const { sha256 } = require('./artifacts');
const baseline = require('./baseline.json');

function counts(values, keys) {
  return Object.fromEntries(keys.map(key => [key, values.filter(value => value === key).length]));
}

function createReport(directory) {
  const bundle = verifyBundle(directory);
  const read = reference => readBounded(directory, reference.path, 128 * 1024 * 1024);
  const manifest = JSON.parse(read(bundle.provenance.corpusManifest));
  const inventory = JSON.parse(read(bundle.provenance.inventory));
  const environment = JSON.parse(read(bundle.provenance.environment));
  const metadata = validateCorpus(manifest, file => {
    const reference = bundle.provenance.corpusFiles.find(item => item.path === `inputs/corpus/files/${file}`);
    if (!reference) throw new Error(`Missing corpus input ${file}.`);
    return read(reference);
  }, inventory);
  if (!metadata) throw new Error('A behavior report requires a provenance-backed behavior corpus, not runner-validation evidence.');
  const results = bundle.results.map(reference => ({ reference, result: JSON.parse(read(reference)) }));
  const selectedIds = new Set(bundle.cases.map(item => item.id));
  const selectedTargets = new Set(manifest.cases.filter(item => selectedIds.has(item.id)).flatMap(item => item.targets));
  const mappedApis = inventory.entries.filter(item => selectedTargets.has(item.id)).map(item => item.id);
  const unmappedApis = inventory.entries.filter(item => !selectedTargets.has(item.id)).map(item => item.id);
  const mappedBehaviors = inventory.behaviors.filter(item => selectedTargets.has(item.id)).map(item => item.id);
  const retainedProcess = observation => observation ? {
    ...observation,
    stdout: { ...observation.stdout, base64: read(observation.stdout).toString('base64') },
    stderr: { ...observation.stderr, base64: read(observation.stderr).toString('base64') }
  } : null;
  const retainedObservables = observations => observations.map(item => ({
    ...item, ...(item.artifact ? { base64: read(item.artifact).toString('base64') } : {})
  }));
  return {
    schemaVersion: 1, suiteId: bundle.suiteId, artifactKind: 'behavior-slice-report',
    scopeId: bundle.scopeId, corpusId: metadata.corpusId, runId: bundle.runId, asOf: bundle.finishedAt,
    provenance: bundle.provenance, bundleSha256: sha256(readBounded(directory, 'bundle.json', 128 * 1024 * 1024)),
    environment,
    evidenceAvailability: 'Full bundle retained in the named local run directory, not published central evidence. This report retains lossless stdout/stderr/observable snapshots; compiler binaries remain in the full bundle.',
    selection: metadata.selection,
    registeredScenarios: manifest.cases.length, selectedScenarios: bundle.cases.length, attempts: results.length,
    upstream: {
      repository: metadata.upstream.repository, sourceRevision: metadata.upstream.sourceRevision,
      license: metadata.upstream.license, inspectedFiles: metadata.upstream.files.length,
      classifications: counts(metadata.upstream.files.map(file => file.classification), ['runnable', 'adaptation-required', 'platform-specific', 'excluded']),
      unknownApplicability: metadata.upstream.files.filter(file => file.applicability.state === 'unknown').length,
      note: 'Upstream files are discovery leads, not executed/ported cases. Their classifications never contribute to scenario outcome counts.'
    },
    inventory: {
      totalApiCandidates: inventory.entries.length, mappedApiCandidates: mappedApis, unmappedApiCandidates: unmappedApis,
      mappedLoaderBehaviors: mappedBehaviors,
      note: 'Mappings indicate selected assertion intent, not whole-API support or proof every mapped operation was reached. Unmapped APIs are untested by this slice; compile/runtime failures can prevent mapped operations from executing.'
    },
    outcomes: counts(bundle.cases.map(item => item.outcome), baseline.outcomes),
    cases: bundle.cases.map(summary => ({
      ...summary, targets: manifest.cases.find(item => item.id === summary.id).targets,
      attempts: results.filter(item => item.result.scenarioId === summary.id).map(({ reference, result }) => ({
        attemptId: result.attemptId, attemptNumber: result.attemptNumber, resultArtifact: reference,
        outcome: result.outcome, phase: result.phase, detail: result.detail,
        policySha256: result.policySha256,
        reference: retainedProcess(result.reference), compilation: retainedProcess(result.compilation),
        execution: retainedProcess(result.execution),
        referenceObservables: retainedObservables(result.referenceObservables),
        jrocObservables: retainedObservables(result.jrocObservables)
      }))
    }))
  };
}

function markdown(report) {
  const group = name => report.cases.filter(item => item.id.startsWith(`node:scenario/public-api/${name}/`));
  const lines = [
    '# Node public-API first-slice evidence', '',
    `As of ${report.asOf}; JROC checkout \`${report.provenance.checkoutRevision}\`.`,
    `Corpus \`${report.corpusId}\`, executable digest \`${report.provenance.corpusRevision}\`.`, '',
    '**Local-unverified observations, not an overall Node compatibility score or trusted catalogue acceptance.**', '',
    `${report.selectedScenarios}/${report.registeredScenarios} original scenarios selected; ${report.attempts} retained attempts.`,
    'Counts are per scenario, never per target mapping or attempt. Non-pass outcomes remain visible.', '',
    '| Area | Scenarios | Pass | Mismatch | Compile failure | Other |',
    '| --- | ---: | ---: | ---: | ---: | ---: |'
  ];
  for (const name of ['path', 'buffer', 'events', 'loader']) {
    const cases = group(name);
    const n = outcome => cases.filter(item => item.outcome === outcome).length;
    lines.push(`| ${name} | ${cases.length} | ${n('pass')} | ${n('mismatch')} | ${n('compile-failure')} | ${cases.length - n('pass') - n('mismatch') - n('compile-failure')} |`);
  }
  lines.push('', '## Scope and limitations', '',
    `${report.upstream.inspectedFiles} pinned upstream discovery leads: ${Object.entries(report.upstream.classifications).map(([name, count]) => `${count} ${name}`).join(', ')}. ${report.upstream.unknownApplicability} have unknown whole-file applicability.`,
    'No upstream test file is claimed as executed. Original fixture assertions are independently written; see the retained corpus metadata for exact URLs, revisions, hashes, license provenance and triage reasons.',
    `${report.inventory.mappedApiCandidates.length}/${report.inventory.totalApiCandidates} API candidates have selected scenario mappings; ${report.inventory.unmappedApiCandidates.length} are not selected/tested by this slice. ${report.inventory.mappedLoaderBehaviors.length} loader behavior registrations have mappings.`,
    report.inventory.note,
    `Recorded profile: Node ${report.environment.nodeVersion}, ${report.environment.platform} ${report.environment.architecture}, .NET SDK ${report.environment.dotnetSdk}, installed .NET runtimes ${report.environment.dotnetRuntimes.join(', ')}. Exact timings and unconstrained scheduling are not compared. There is no web-platform API in this slice, so no WPT case is claimed.`,
    'Upstream private-internal/flag-dependent and oversized tests are triaged separately, never counted as JROC successes or failures. This is not a package-workflow scorecard or a CI regression baseline.',
    report.evidenceAvailability, '',
    '## Scenario outcomes', '', '| Scenario | Outcome | Phase(s) |', '| --- | --- | --- |');
  for (const item of report.cases) {
    lines.push(`| ${item.id.replace('node:scenario/public-api/', '')} | ${item.outcome} | ${[...new Set(item.attempts.map(attempt => attempt.phase))].join(', ')} |`);
  }
  lines.push('', 'The paired diagnostic bytes, target mappings, attempt history and raw-artifact hashes are retained in the sibling JSON report. Read stdout/stderr `base64` fields losslessly; `Buffer.from(value, "base64").toString("utf8")` is useful for text diagnostics.',
    'A compiler rejection can prevent a scenario from reaching the intended API assertion; inspect diagnostics before attributing the failure to a particular operation. Missing runtime APIs can appear as mismatches rather than the compiler-only `unsupported` classification.', '');
  return lines.join('\n');
}

function main(args) {
  if (args.length !== 2) throw new Error('Usage: node reportBehaviorCorpus.js <bundle-directory> <new-output-directory>');
  const report = createReport(path.resolve(args[0]));
  const json = `${JSON.stringify(report, null, 2)}\n`;
  if (Buffer.byteLength(json) > 8 * 1024 * 1024) throw new Error('Report exceeds its 8 MiB retention budget.');
  fs.mkdirSync(args[1], { recursive: false });
  fs.writeFileSync(path.join(args[1], 'report.json'), json, { flag: 'wx' });
  fs.writeFileSync(path.join(args[1], 'report.md'), markdown(report), { flag: 'wx' });
  console.log(`Retained first-slice report: ${path.resolve(args[1])}`);
}

if (require.main === module) {
  try { main(process.argv.slice(2)); } catch (failure) { console.error(failure.message); process.exitCode = 1; }
}

module.exports = { createReport, markdown };
