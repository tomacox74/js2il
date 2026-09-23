const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const test = require('node:test');
const {
    normalizeRuntime,
    parseBenchmarkDotNetResults,
    resolveRuntimeVersion,
    parseMitataResults,
    parsePrimeResults,
    buildUpsertConflictKey,
    benchmarkDotNetClr,
    readBenchmarkDotNetJobVersions,
    dedupeRowsForUpsert,
    UPSERT_CONFLICT_KEYS
} = require('./ingestPerfToSupabase');

const fixtures = path.join(__dirname, 'fixtures', 'ingestion');
const base = { run_id: 1, run_attempt: 1 };

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
    {
        const rows = parseBenchmarkDotNetResults(
            path.join(fixtures, 'previous'),
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
    }
});

test('keeps dual CLR rows separate and records exact child versions', () => {
    assert.deepEqual(UPSERT_CONFLICT_KEYS, [
        'run_id', 'run_attempt', 'source', 'scenario', 'runtime',
        'dotnet_runtime', 'benchmark_profile', 'metric'
    ]);
    const rows = parseBenchmarkDotNetResults(path.join(fixtures, 'dual'), base, {}, { jroc: '0.12.14' }, 'dotnet-runtime-comparison');
    const means = dedupeRowsForUpsert(rows).filter(row => row.scenario === 'primejavascript-onepass' && row.metric === 'mean_ns');
    assert.equal(means.length, 2);
    assert.deepEqual(means.map(row => [
        row.dotnet_runtime, row.dotnet_runtime_version, row.runtime,
        row.runtime_version, row.benchmark_profile, row.value
    ]), [
        ['net10.0', '10.0.12', 'jroc-execute', '0.12.14', 'dotnet-runtime-comparison', 156442698],
        ['net11.0', '11.0.0-rc.1.26425.128', 'jroc-execute', '0.12.14', 'dotnet-runtime-comparison', 160283968]
    ]);
    assert.deepEqual(UPSERT_CONFLICT_KEYS.filter(key => means[0][key] !== means[1][key]), ['dotnet_runtime']);
    assert.notEqual(buildUpsertConflictKey(means[0]), buildUpsertConflictKey(means[1]));
});

test('defaults all sources to default profile, retaining Prime scenario and nullable CLR', () => {
    const bdn = parseBenchmarkDotNetResults(path.join(fixtures, 'dual'), base, {}, {});
    const mitata = parseMitataResults(path.join(fixtures, 'mitata.json'), base, {}, {});
    const prime = parsePrimeResults(path.join(fixtures, 'prime.json'), base, {}, {});
    for (const rows of [bdn, mitata, prime]) {
        assert.ok(rows.length > 0);
        assert.ok(rows.every(row => row.benchmark_profile === 'default'));
    }
    assert.ok(mitata.every(row => row.dotnet_runtime === null));
    assert.ok(prime.every(row => row.dotnet_runtime === null && row.scenario === 'prime-javascript'));
    assert.equal(parsePrimeResults(path.join(fixtures, 'prime.json'), base, {}, {}, 'experimental')[0].benchmark_profile, 'experimental');
    assert.equal(parseMitataResults(path.join(fixtures, 'mitata.json'), base, {}, {}, 'experimental')[0].benchmark_profile, 'experimental');
    const duplicate = { ...prime[0], value: 20 };
    assert.deepEqual(dedupeRowsForUpsert([prime[0], duplicate]), [duplicate]);
    assert.notEqual(buildUpsertConflictKey(prime[0]), buildUpsertConflictKey({ ...prime[0], benchmark_profile: 'experimental' }));
});

test('comparison rejects missing or ambiguous child CLR identity', () => {
    const benchmark = { MethodTitle: 'jroc-execute' };
    const versions = new Map([['net10.0', '10.0.12']]);
    assert.throws(() => benchmarkDotNetClr(benchmark, versions, 'dotnet-runtime-comparison'), /Missing or ambiguous child CLR/);
    assert.throws(() => benchmarkDotNetClr({ ...benchmark, DisplayInfo: '.NET 10.0 and .NET 11.0' }, versions, 'dotnet-runtime-comparison'), /Missing or ambiguous child CLR/);
    assert.throws(() => benchmarkDotNetClr({ ...benchmark, DisplayInfo: '.NET 11.0' }, versions, 'dotnet-runtime-comparison'), /Missing or ambiguous child CLR/);
    assert.throws(() => benchmarkDotNetClr({ ...benchmark, DisplayInfo: '.NET 10.0', RuntimeVersion: '.NET 11.0.0' }, versions, 'dotnet-runtime-comparison'), /Missing or ambiguous child CLR/);
    assert.throws(() => benchmarkDotNetClr({ ...benchmark, DisplayInfo: '.NET 10.0', RuntimeVersion: '10.0.13' }, versions, 'dotnet-runtime-comparison'), /Missing or ambiguous child CLR/);
    assert.deepEqual(benchmarkDotNetClr({
        ...benchmark, DisplayInfo: '.NET 10.0', RuntimeVersion: '.NET 10.0.12 (10.0.12, 10.0.1226.42308)'
    }, versions, 'dotnet-runtime-comparison'), { dotnet_runtime: 'net10.0', dotnet_runtime_version: '10.0.12' });
    assert.deepEqual(benchmarkDotNetClr({
        ...benchmark, DisplayInfo: '.NET 11.0', RuntimeVersion: '.NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628)'
    }, new Map(), 'dotnet-runtime-comparison'), { dotnet_runtime: 'net11.0', dotnet_runtime_version: '11.0.0-rc.1.26425.128' });
    assert.throws(() => readBenchmarkDotNetJobVersions(
        path.join(fixtures, 'conflicting'), 'conflicting-report-full-compressed.json'
    ), /Conflicting CLR builds/);
});

test('CLI applies benchmark profile to all three sources without uploading', () => {
    const cases = [
        ['benchmarkdotnet', path.join(fixtures, 'dual')],
        ['mitata', path.join(fixtures, 'mitata.json')],
        ['prime-script', path.join(fixtures, 'prime.json')]
    ];
    for (const [source, input] of cases) {
        for (const profileArgs of [[], ['--benchmark-profile', 'dotnet-runtime-comparison']]) {
            const result = spawnSync(process.execPath, [
                path.join(__dirname, 'ingestPerfToSupabase.js'), '--source', source, '--input', input, ...profileArgs
            ], { encoding: 'utf8', env: { ...process.env, SUPABASE_URL: '', SUPABASE_SERVICE_ROLE_KEY: '' } });
            assert.equal(result.status, 0, `${source}: ${result.stderr}`);
            assert.match(result.stdout, /Prepared \d+ rows/);
        }
    }
});

test('real preview.2 Prime report exposes per-child builds when available', { skip: !fs.existsSync(
    path.join(__dirname, 'Benchmarks', 'BenchmarkDotNet.Artifacts', 'results', 'Benchmarks.PrimeExecuteBenchmark-report-full-compressed.json')
) }, () => {
    const results = path.join(__dirname, 'Benchmarks', 'BenchmarkDotNet.Artifacts', 'results');
    const file = 'Benchmarks.PrimeExecuteBenchmark-report-full-compressed.json';
    const report = JSON.parse(fs.readFileSync(path.join(results, file), 'utf8'));
    const versions = readBenchmarkDotNetJobVersions(results, file);
    assert.deepEqual(report.Benchmarks.map(row => benchmarkDotNetClr(row, versions, 'dotnet-runtime-comparison'))
        .sort((a, b) => a.dotnet_runtime.localeCompare(b.dotnet_runtime)), [
        { dotnet_runtime: 'net10.0', dotnet_runtime_version: '10.0.12' },
        { dotnet_runtime: 'net11.0', dotnet_runtime_version: '11.0.0-rc.1.26425.128' }
    ]);
});

test('legacy 0.15.8 DefaultJob JSON does not borrow the host CLR identity', { skip: !fs.existsSync(
    path.join(__dirname, 'Benchmarks', 'BenchmarkDotNet.Artifacts', 'results', 'Benchmarks.DromaeoExecutionBenchmarks-report-full-compressed.json')
) }, () => {
    const results = path.join(__dirname, 'Benchmarks', 'BenchmarkDotNet.Artifacts', 'results');
    const file = 'Benchmarks.DromaeoExecutionBenchmarks-report-full-compressed.json';
    const report = JSON.parse(fs.readFileSync(path.join(results, file), 'utf8'));
    assert.equal(report.HostEnvironmentInfo.BenchmarkDotNetVersion, '0.15.8');
    assert.match(report.HostEnvironmentInfo.RuntimeVersion, /10\.0\.11/);
    assert.ok(report.Benchmarks.every(row => row.DisplayInfo.includes('DefaultJob')));
    const versions = readBenchmarkDotNetJobVersions(results, file);
    assert.ok(report.Benchmarks.every(row => {
        const clr = benchmarkDotNetClr(row, versions, 'default');
        return clr.dotnet_runtime === null && clr.dotnet_runtime_version === null;
    }));
});
