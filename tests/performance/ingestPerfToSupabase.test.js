const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');
const {
    normalizeRuntime,
    parseBenchmarkDotNetResults,
    resolveRuntimeVersion
} = require('./ingestPerfToSupabase');

test('preserves execution-phase runtime identities', () => {
    assert.equal(normalizeRuntime('jint-execute'), 'jint-execute');
    assert.equal(normalizeRuntime('yantrajs-execute'), 'yantrajs-execute');
});

test('keeps existing Jint phase distinctions', () => {
    assert.equal(normalizeRuntime('Jint prepare'), 'jint-prepare');
    assert.equal(normalizeRuntime('Jint execute (prepared)'), 'jint-execute-prepared');
});

test('keeps previous Jroc results distinct from current Jroc results', () => {
    assert.equal(normalizeRuntime('JrocPrevious (compile+execute)'), 'jroc-previous-total');
    assert.equal(normalizeRuntime('jroc-previous-execute'), 'jroc-previous-execute');
    assert.equal(normalizeRuntime('JrocPrevious'), 'jroc-previous');
});

test('assigns the previous package version to previous Jroc results', () => {
    const versions = {
        jroc: '0.12.14',
        jroc_previous: '0.12.13'
    };

    assert.equal(resolveRuntimeVersion('jroc-total', versions), '0.12.14');
    assert.equal(resolveRuntimeVersion('jroc-previous-total', versions), '0.12.13');
    assert.equal(resolveRuntimeVersion('jroc-previous-execute', versions), '0.12.13');
});

test('parses current and previous Jroc reports as separate same-run rows', () => {
    const resultsDirectory = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-ingestion-'));
    try {
        const report = {
            Benchmarks: [
                {
                    Method: 'Jroc_Total',
                    MethodTitle: 'jroc (compile+execute)',
                    Parameters: 'ScriptName=minimal',
                    Statistics: { Mean: 100, Median: 99, StandardDeviation: 1 }
                },
                {
                    Method: 'Jroc_Total',
                    MethodTitle: 'JrocPrevious (compile+execute)',
                    Parameters: 'ScriptName=minimal',
                    Statistics: { Mean: 110, Median: 109, StandardDeviation: 1 }
                }
            ]
        };
        fs.writeFileSync(
            path.join(resultsDirectory, 'report.json'),
            JSON.stringify(report),
            'utf8');

        const rows = parseBenchmarkDotNetResults(
            resultsDirectory,
            { run_id: 1, run_attempt: 1 },
            {},
            { jroc: '0.12.14', jroc_previous: '0.12.13' });
        const means = rows
            .filter(row => row.metric === 'mean_ns')
            .sort((left, right) => left.runtime.localeCompare(right.runtime));

        assert.deepEqual(
            means.map(row => [row.runtime, row.runtime_version, row.value]),
            [
                ['jroc-previous-total', '0.12.13', 110],
                ['jroc-total', '0.12.14', 100]
            ]);
    } finally {
        fs.rmSync(resultsDirectory, { recursive: true, force: true });
    }
});
