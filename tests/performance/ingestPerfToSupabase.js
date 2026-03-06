const fs = require('fs');
const os = require('os');
const path = require('path');

function parseArgs(argv) {
    const args = {};
    for (let i = 0; i < argv.length; i++) {
        const token = argv[i];
        if (token.startsWith('--')) {
            const key = token.slice(2);
            const next = argv[i + 1];
            if (!next || next.startsWith('--')) {
                args[key] = true;
                continue;
            }
            args[key] = next;
            i++;
        }
    }
    return args;
}

const HOST_COLUMN_KEYS = [
    'os_type',
    'os_platform',
    'os_release',
    'os_version',
    'os_arch',
    'cpu_model',
    'cpu_logical_cores',
    'total_memory_bytes',
    'runner_os',
    'runner_arch',
    'github_image_os',
    'github_image_version'
];

const UPSERT_CONFLICT_KEYS = ['run_id', 'run_attempt', 'source', 'scenario', 'runtime', 'metric'];

const HTML_ENTITY_MAP = {
    '&#39;': "'",
    '&quot;': '"',
    '&amp;': '&',
    '&lt;': '<',
    '&gt;': '>'
};

function slugify(value) {
    return String(value ?? 'unknown')
        .trim()
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, '-')
        .replace(/^-+|-+$/g, '') || 'unknown';
}

function decodeHtmlEntities(value) {
    let text = String(value ?? '');
    for (const [entity, literal] of Object.entries(HTML_ENTITY_MAP)) {
        text = text.split(entity).join(literal);
    }
    return text;
}

function normalizeRuntime(raw) {
    const text = String(raw ?? '').toLowerCase();
    if (text.includes('node')) return 'node';
    if (text.includes('yantra')) return 'yantrajs';
    if (text.includes('jint') && text.includes('prepared')) return 'jint-execute-prepared';
    if (text.includes('jint') && text.includes('prepare')) return 'jint-prepare';
    if (text.includes('jint')) return 'jint';
    if (text.includes('js2il') && text.includes('execute') && text.includes('pre-compiled')) return 'js2il-execute';
    if (text.includes('js2il') && text.includes('execute') && text.includes('precompiled')) return 'js2il-execute';
    if (text.includes('js2il') && text.includes('compile') && text.includes('execute')) return 'js2il-total';
    if (text.includes('js2il') && text.includes('compile')) return 'js2il-compile';
    if (text.includes('js2il') && text.includes('execute')) return 'js2il-execute';
    if (text.includes('js2il')) return 'js2il';
    return slugify(raw);
}

function parseScriptNameFromDisplay(displayInfo) {
    const text = String(displayInfo ?? '');
    const match = text.match(/ScriptName\s*[:=]\s*([A-Za-z0-9._-]+)/i);
    if (!match) return null;
    const candidate = match[1];
    if (candidate.includes('...')) return null;
    return slugify(candidate);
}

function parseScriptNameFromFullName(fullName) {
    const text = String(fullName ?? '');
    const quoted = text.match(/ScriptName\s*:\s*"([^"]+)"/i);
    if (quoted) {
        return slugify(quoted[1]);
    }

    const unquoted = text.match(/ScriptName\s*:\s*([^)]+)/i);
    return unquoted ? slugify(unquoted[1]) : null;
}

function parseScriptNameFromParameters(parameters) {
    if (parameters === null || parameters === undefined) return null;

    if (typeof parameters === 'string') {
        const match = parameters.match(/(?:^|,\s*)ScriptName\s*=\s*([^,\]]+)/i);
        if (!match) return null;
        const candidate = decodeHtmlEntities(match[1]).trim();
        if (!candidate || candidate.includes('...')) return null;
        return slugify(candidate.replace(/^"(.*)"$/, '$1'));
    }

    if (typeof parameters === 'object') {
        const candidate = parameters.ScriptName ?? parameters.scriptName;
        if (candidate) {
            return slugify(candidate);
        }
    }

    return null;
}

function parseDurationToNs(value) {
    if (value === null || value === undefined) return null;
    const raw = decodeHtmlEntities(String(value)).trim();
    if (!raw || raw === 'NA') return null;

    const normalized = raw.replace(/,/g, '');
    const match = normalized.match(/^(-?\d+(?:\.\d+)?)\s*(ns|us|μs|ms|s)$/i);
    if (!match) {
        const numeric = Number.parseFloat(normalized);
        return Number.isFinite(numeric) ? numeric : null;
    }

    const amount = Number.parseFloat(match[1]);
    if (!Number.isFinite(amount)) return null;

    const unit = match[2].toLowerCase();
    const multiplier = unit === 'ns'
        ? 1
        : unit === 'us' || unit === 'μs'
            ? 1_000
            : unit === 'ms'
                ? 1_000_000
                : 1_000_000_000;

    return amount * multiplier;
}

function parseMemoryToBytes(value) {
    if (value === null || value === undefined) return null;
    if (typeof value === 'number' && Number.isFinite(value)) return value;

    const raw = decodeHtmlEntities(String(value)).trim();
    if (!raw || raw === 'NA' || raw === '-') return null;

    const normalized = raw.replace(/,/g, '');
    const match = normalized.match(/^(-?\d+(?:\.\d+)?)\s*(B|KB|MB|GB|TB)$/i);
    if (!match) {
        const numeric = Number.parseFloat(normalized);
        return Number.isFinite(numeric) ? numeric : null;
    }

    const amount = Number.parseFloat(match[1]);
    if (!Number.isFinite(amount)) return null;

    const unit = match[2].toLowerCase();
    const multiplier = unit === 'b'
        ? 1
        : unit === 'kb'
            ? 1024
            : unit === 'mb'
                ? 1024 * 1024
                : unit === 'gb'
                    ? 1024 * 1024 * 1024
                    : 1024 * 1024 * 1024 * 1024;

    return amount * multiplier;
}

function getNumber(value) {
    if (typeof value === 'number' && Number.isFinite(value)) return value;
    if (typeof value === 'string') {
        const normalized = decodeHtmlEntities(value).trim().replace(/,/g, '');
        if (!normalized || normalized === 'NA' || normalized === '-') return null;
        const parsed = Number.parseFloat(normalized);
        if (Number.isFinite(parsed)) return parsed;
    }
    return null;
}

function firstNumber(...values) {
    for (const value of values) {
        const parsed = getNumber(value);
        if (parsed !== null) {
            return parsed;
        }
    }
    return null;
}

function firstMemoryBytes(...values) {
    for (const value of values) {
        const parsed = parseMemoryToBytes(value);
        if (parsed !== null) {
            return parsed;
        }
    }
    return null;
}

function getBaseContext() {
    return {
        run_id: Number.parseInt(process.env.GITHUB_RUN_ID ?? `${Date.now()}`, 10),
        run_attempt: Number.parseInt(process.env.GITHUB_RUN_ATTEMPT ?? '1', 10),
        workflow: process.env.GITHUB_WORKFLOW ?? 'local',
        repo: process.env.GITHUB_REPOSITORY ?? 'local/local',
        branch: process.env.GITHUB_REF_NAME ?? null,
        sha: process.env.GITHUB_SHA ?? 'local'
    };
}

function compactObject(obj) {
    return Object.fromEntries(
        Object.entries(obj).filter(([, value]) => value !== null && value !== undefined && value !== '')
    );
}

function buildUpsertConflictKey(row) {
    return UPSERT_CONFLICT_KEYS
        .map(key => String(row?.[key] ?? ''))
        .join('||');
}

function dedupeRowsForUpsert(rows) {
    const byKey = new Map();
    for (const row of rows) {
        byKey.set(buildUpsertConflictKey(row), row);
    }
    return Array.from(byKey.values());
}

function getHostMetadata() {
    const cpus = os.cpus() ?? [];
    const firstCpu = cpus.length > 0 ? cpus[0] : null;

    return compactObject({
        os_type: os.type(),
        os_platform: os.platform(),
        os_release: os.release(),
        os_version: typeof os.version === 'function' ? os.version() : null,
        os_arch: os.arch(),
        hostname: os.hostname(),
        cpu_model: firstCpu?.model ?? null,
        cpu_logical_cores: cpus.length > 0 ? cpus.length : null,
        total_memory_bytes: typeof os.totalmem === 'function' ? os.totalmem() : null,
        runner_os: process.env.RUNNER_OS ?? null,
        runner_arch: process.env.RUNNER_ARCH ?? null,
        runner_name: process.env.RUNNER_NAME ?? null,
        runner_environment: process.env.RUNNER_ENVIRONMENT ?? null,
        github_image_os: process.env.ImageOS ?? null,
        github_image_version: process.env.ImageVersion ?? null
    });
}

function buildHostColumns(hostMetadata) {
    return compactObject({
        os_type: hostMetadata.os_type ?? null,
        os_platform: hostMetadata.os_platform ?? null,
        os_release: hostMetadata.os_release ?? null,
        os_version: hostMetadata.os_version ?? null,
        os_arch: hostMetadata.os_arch ?? null,
        cpu_model: hostMetadata.cpu_model ?? null,
        cpu_logical_cores: hostMetadata.cpu_logical_cores ?? null,
        total_memory_bytes: hostMetadata.total_memory_bytes ?? null,
        runner_os: hostMetadata.runner_os ?? null,
        runner_arch: hostMetadata.runner_arch ?? null,
        github_image_os: hostMetadata.github_image_os ?? null,
        github_image_version: hostMetadata.github_image_version ?? null
    });
}

function extractBenchmarkDotNetHostMetadata(data) {
    const host = data?.HostEnvironmentInfo
        ?? data?.hostEnvironmentInfo
        ?? data?.HostEnvironment
        ?? data?.hostEnvironment
        ?? null;

    if (!host || typeof host !== 'object') {
        return {};
    }

    return compactObject({
        bdn_os_description: host.OsVersion ?? host.osVersion ?? host.Os ?? host.os ?? null,
        bdn_runtime_version: host.RuntimeVersion ?? host.runtimeVersion ?? host.RuntimeMoniker ?? host.runtimeMoniker ?? null,
        bdn_cpu_model: host.ProcessorName ?? host.processorName ?? null,
        bdn_cpu_physical_cores: host.PhysicalCoreCount ?? host.physicalCoreCount ?? host.PhysicalProcessorCount ?? host.physicalProcessorCount ?? null,
        bdn_cpu_logical_cores: host.LogicalCoreCount ?? host.logicalCoreCount ?? null,
        bdn_benchmarkdotnet_version: host.BenchmarkDotNetVersion ?? host.benchmarkDotNetVersion ?? null
    });
}

function createRow(base, source, scenario, runtime, metric, value, unit, runAt, meta = {}, hostColumns = {}) {
    return {
        ...base,
        ...hostColumns,
        source,
        scenario: slugify(scenario),
        runtime: normalizeRuntime(runtime),
        metric,
        value,
        unit,
        run_at: runAt,
        meta
    };
}

function parsePrimeResults(inputPath, base, hostMetadata) {
    if (!fs.existsSync(inputPath)) {
        console.log(`Prime results file not found: ${inputPath}`);
        return [];
    }

    const payload = JSON.parse(fs.readFileSync(inputPath, 'utf8'));
    const results = payload.results ?? {};
    const runAt = payload.timestamp ?? new Date().toISOString();
    const scenario = 'prime-javascript';
    const rows = [];
    const hostColumns = buildHostColumns(hostMetadata);

    for (const [runtimeKey, runtimeData] of Object.entries(results)) {
        if (!runtimeData || typeof runtimeData !== 'object') {
            continue;
        }

        const runtime = runtimeKey;
        const passes = getNumber(runtimeData.passes);
        const passesPerSecond = getNumber(runtimeData.passesPerSecond);
        const compileDuration = getNumber(runtimeData.compileDuration);

        if (passes !== null) {
            rows.push(createRow(base, 'prime-script', scenario, runtime, 'passes', passes, 'count', runAt, {
                host: hostMetadata
            }, hostColumns));
        }
        if (passesPerSecond !== null) {
            rows.push(createRow(base, 'prime-script', scenario, runtime, 'passes_per_second', passesPerSecond, 'count_per_sec', runAt, {
                host: hostMetadata
            }, hostColumns));
        }
        if (compileDuration !== null) {
            rows.push(createRow(base, 'prime-script', scenario, runtime, 'compile_duration_ms', compileDuration, 'ms', runAt, {
                host: hostMetadata
            }, hostColumns));
        }
    }

    return rows;
}

function extractBenchmarksFromJson(data) {
    if (!data) return [];
    if (Array.isArray(data)) {
        return data.flatMap(extractBenchmarksFromJson);
    }
    if (Array.isArray(data.Benchmarks)) return data.Benchmarks;
    if (Array.isArray(data.benchmarks)) return data.benchmarks;
    if (Array.isArray(data.Reports)) return data.Reports.flatMap(extractBenchmarksFromJson);
    return [];
}

function parseBenchmarkDotNetResults(resultsDir, base, hostMetadata) {
    if (!fs.existsSync(resultsDir)) {
        console.log(`BenchmarkDotNet results directory not found: ${resultsDir}`);
        return [];
    }

    const files = fs.readdirSync(resultsDir)
        .filter(file => file.endsWith('.json'))
        .map(file => path.join(resultsDir, file));

    const rows = [];
    for (const filePath of files) {
        let data;
        try {
            data = JSON.parse(fs.readFileSync(filePath, 'utf8'));
        } catch (error) {
            console.log(`Skipping invalid JSON file: ${filePath} (${error.message})`);
            continue;
        }

        const benchmarks = extractBenchmarksFromJson(data);
        if (benchmarks.length === 0) {
            continue;
        }
        const bdnHostMetadata = compactObject({
            ...hostMetadata,
            ...extractBenchmarkDotNetHostMetadata(data)
        });
        const hostColumns = buildHostColumns(bdnHostMetadata);

        for (const benchmark of benchmarks) {
            const params = benchmark.Parameters ?? benchmark.parameters ?? benchmark.Params ?? {};
            const fullName = benchmark.FullName ?? benchmark.fullName ?? '';
            const displayInfo = benchmark.DisplayInfo ?? benchmark.displayInfo ?? fullName ?? '';
            const scenario = parseScriptNameFromParameters(params)
                ?? parseScriptNameFromFullName(fullName)
                ?? parseScriptNameFromDisplay(displayInfo)
                ?? 'unknown';
            const runtimeRaw = benchmark.Description
                ?? benchmark.description
                ?? benchmark.MethodTitle
                ?? benchmark.methodTitle
                ?? benchmark.Method
                ?? benchmark.method
                ?? displayInfo
                ?? 'unknown';
            const stats = benchmark.Statistics ?? benchmark.statistics ?? benchmark.ResultStatistics ?? benchmark.resultStatistics ?? {};
            const runAt = new Date().toISOString();
            const meta = {
                report_file: path.basename(filePath),
                benchmark_type: benchmark.Type ?? benchmark.type ?? null,
                benchmark_method: benchmark.Method ?? benchmark.method ?? null,
                params,
                host: bdnHostMetadata
            };

            const mean = getNumber(stats.Mean ?? stats.mean);
            const median = getNumber(stats.Median ?? stats.median);
            const stdDev = getNumber(stats.StandardDeviation ?? stats.standardDeviation ?? stats.StdDev ?? stats.stdDev);
            const memoryStats = benchmark.Memory ?? benchmark.memory ?? benchmark.MemoryStatistics ?? benchmark.memoryStatistics ?? {};
            const gcStats = benchmark.GcStats ?? benchmark.gcStats ?? memoryStats.GcStats ?? memoryStats.gcStats ?? {};
            const allocatedBytes = firstMemoryBytes(
                benchmark.AllocatedBytes,
                benchmark.allocatedBytes,
                memoryStats.AllocatedBytes,
                memoryStats.allocatedBytes,
                memoryStats.BytesAllocatedPerOperation,
                memoryStats.bytesAllocatedPerOperation,
                memoryStats.Allocated,
                memoryStats.allocated
            );
            const allocatedNativeMemoryBytes = firstMemoryBytes(
                benchmark.AllocatedNativeMemory,
                benchmark.allocatedNativeMemory,
                memoryStats.AllocatedNativeMemory,
                memoryStats.allocatedNativeMemory,
                memoryStats.NativeMemoryBytes,
                memoryStats.nativeMemoryBytes
            );
            const gen0Collections = firstNumber(
                benchmark.Gen0,
                benchmark.gen0,
                benchmark.Gen0Collections,
                benchmark.gen0Collections,
                gcStats.Gen0Collections,
                gcStats.gen0Collections,
                memoryStats.Gen0Collections,
                memoryStats.gen0Collections
            );
            const gen1Collections = firstNumber(
                benchmark.Gen1,
                benchmark.gen1,
                benchmark.Gen1Collections,
                benchmark.gen1Collections,
                gcStats.Gen1Collections,
                gcStats.gen1Collections,
                memoryStats.Gen1Collections,
                memoryStats.gen1Collections
            );
            const gen2Collections = firstNumber(
                benchmark.Gen2,
                benchmark.gen2,
                benchmark.Gen2Collections,
                benchmark.gen2Collections,
                gcStats.Gen2Collections,
                gcStats.gen2Collections,
                memoryStats.Gen2Collections,
                memoryStats.gen2Collections
            );

            if (mean !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtimeRaw, 'mean_ns', mean, 'ns', runAt, meta, hostColumns));
            }
            if (median !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtimeRaw, 'median_ns', median, 'ns', runAt, meta, hostColumns));
            }
            if (stdDev !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtimeRaw, 'stddev_ns', stdDev, 'ns', runAt, meta, hostColumns));
            }
            if (allocatedBytes !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtimeRaw, 'allocated_bytes', allocatedBytes, 'bytes', runAt, meta, hostColumns));
            }
            if (allocatedNativeMemoryBytes !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtimeRaw, 'allocated_native_memory_bytes', allocatedNativeMemoryBytes, 'bytes', runAt, meta, hostColumns));
            }
            if (gen0Collections !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtimeRaw, 'gen0_collections_per_1000_ops', gen0Collections, 'collections_per_1000_ops', runAt, meta, hostColumns));
            }
            if (gen1Collections !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtimeRaw, 'gen1_collections_per_1000_ops', gen1Collections, 'collections_per_1000_ops', runAt, meta, hostColumns));
            }
            if (gen2Collections !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtimeRaw, 'gen2_collections_per_1000_ops', gen2Collections, 'collections_per_1000_ops', runAt, meta, hostColumns));
            }
        }
    }

    if (rows.length > 0) {
        return rows;
    }

    return parseBenchmarkDotNetMarkdownResults(resultsDir, base, hostMetadata);
}

function parseBenchmarkDotNetMarkdownResults(resultsDir, base, hostMetadata) {
    const files = fs.readdirSync(resultsDir)
        .filter(file => file.endsWith('.md'))
        .map(file => path.join(resultsDir, file));

    const rows = [];

    for (const filePath of files) {
        const text = fs.readFileSync(filePath, 'utf8');
        const lines = text.split(/\r?\n/);
        const headerIndex = lines.findIndex(line => line.includes('| Method') && line.includes('| ScriptName'));
        if (headerIndex < 0) {
            continue;
        }
        const headers = lines[headerIndex]
            .split('|')
            .map(column => decodeHtmlEntities(column.trim()))
            .slice(1, -1);
        const normalizedHeaders = headers.map(header => header.toLowerCase().replace(/[^a-z0-9]+/g, ''));
        const methodIndex = normalizedHeaders.indexOf('method');
        const scenarioIndex = normalizedHeaders.indexOf('scriptname');
        const meanIndex = normalizedHeaders.indexOf('mean');
        const stdDevIndex = normalizedHeaders.indexOf('stddev');
        const medianIndex = normalizedHeaders.indexOf('median');
        const allocatedIndex = normalizedHeaders.indexOf('allocated');
        const allocatedNativeMemoryIndex = normalizedHeaders.indexOf('allocatednativememory');
        const gen0Index = normalizedHeaders.indexOf('gen0');
        const gen1Index = normalizedHeaders.indexOf('gen1');
        const gen2Index = normalizedHeaders.indexOf('gen2');

        if (methodIndex < 0 || scenarioIndex < 0) {
            continue;
        }

        const hostColumns = buildHostColumns(hostMetadata);
        const runAt = new Date().toISOString();

        for (let i = headerIndex + 2; i < lines.length; i++) {
            const line = lines[i];
            if (!line.startsWith('|')) {
                if (rows.length > 0) {
                    break;
                }
                continue;
            }

            const columns = line
                .split('|')
                .map(column => decodeHtmlEntities(column.trim()))
                .slice(1, -1);
            if (columns.length < headers.length) {
                continue;
            }

            const method = columns[methodIndex];
            const scenario = columns[scenarioIndex];
            const meanText = meanIndex >= 0 ? columns[meanIndex] : null;
            const stdDevText = stdDevIndex >= 0 ? columns[stdDevIndex] : null;
            const medianText = medianIndex >= 0 ? columns[medianIndex] : null;
            const allocatedText = allocatedIndex >= 0 ? columns[allocatedIndex] : null;
            const allocatedNativeMemoryText = allocatedNativeMemoryIndex >= 0 ? columns[allocatedNativeMemoryIndex] : null;
            const gen0Text = gen0Index >= 0 ? columns[gen0Index] : null;
            const gen1Text = gen1Index >= 0 ? columns[gen1Index] : null;
            const gen2Text = gen2Index >= 0 ? columns[gen2Index] : null;

            if (!scenario || scenario === '-' || scenario === 'ScriptName') {
                continue;
            }

            if (!method && !scenario) {
                continue;
            }

            const runtime = method || 'unknown';
            const meta = {
                report_file: path.basename(filePath),
                source_format: 'benchmarkdotnet-markdown',
                host: hostMetadata
            };

            const meanNs = parseDurationToNs(meanText);
            const medianNs = parseDurationToNs(medianText);
            const stdDevNs = parseDurationToNs(stdDevText);
            const allocatedBytes = parseMemoryToBytes(allocatedText);
            const allocatedNativeMemoryBytes = parseMemoryToBytes(allocatedNativeMemoryText);
            const gen0Collections = getNumber(gen0Text);
            const gen1Collections = getNumber(gen1Text);
            const gen2Collections = getNumber(gen2Text);

            if (meanNs !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtime, 'mean_ns', meanNs, 'ns', runAt, meta, hostColumns));
            }
            if (medianNs !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtime, 'median_ns', medianNs, 'ns', runAt, meta, hostColumns));
            }
            if (stdDevNs !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtime, 'stddev_ns', stdDevNs, 'ns', runAt, meta, hostColumns));
            }
            if (allocatedBytes !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtime, 'allocated_bytes', allocatedBytes, 'bytes', runAt, meta, hostColumns));
            }
            if (allocatedNativeMemoryBytes !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtime, 'allocated_native_memory_bytes', allocatedNativeMemoryBytes, 'bytes', runAt, meta, hostColumns));
            }
            if (gen0Collections !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtime, 'gen0_collections_per_1000_ops', gen0Collections, 'collections_per_1000_ops', runAt, meta, hostColumns));
            }
            if (gen1Collections !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtime, 'gen1_collections_per_1000_ops', gen1Collections, 'collections_per_1000_ops', runAt, meta, hostColumns));
            }
            if (gen2Collections !== null) {
                rows.push(createRow(base, 'benchmarkdotnet', scenario, runtime, 'gen2_collections_per_1000_ops', gen2Collections, 'collections_per_1000_ops', runAt, meta, hostColumns));
            }
        }
    }

    return rows;
}

async function upsertRows(rows) {
    const supabaseUrl = process.env.SUPABASE_URL;
    const supabaseKey = process.env.SUPABASE_SERVICE_ROLE_KEY;

    if (!supabaseUrl || !supabaseKey) {
        console.log('Skipping Supabase ingestion: SUPABASE_URL or SUPABASE_SERVICE_ROLE_KEY is missing.');
        return false;
    }

    const url = `${supabaseUrl.replace(/\/$/, '')}/rest/v1/perf_results?on_conflict=${UPSERT_CONFLICT_KEYS.join(',')}`;
    const chunkSize = 500;

    const uploadChunked = async (rowsToUpload) => {
        for (let i = 0; i < rowsToUpload.length; i += chunkSize) {
            const chunk = rowsToUpload.slice(i, i + chunkSize);
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    apikey: supabaseKey,
                    Authorization: `Bearer ${supabaseKey}`,
                    'Content-Type': 'application/json',
                    Prefer: 'resolution=merge-duplicates,return=minimal'
                },
                body: JSON.stringify(chunk)
            });

            if (!response.ok) {
                const body = await response.text();
                throw new Error(`Supabase upsert failed (${response.status}): ${body}`);
            }
        }
    };

    try {
        await uploadChunked(rows);
    } catch (error) {
        const errorText = String(error?.message ?? error);
        const looksLikeMissingColumn =
            errorText.includes('Could not find the') ||
            (errorText.includes('column') && errorText.includes('does not exist'));

        if (!looksLikeMissingColumn) {
            throw error;
        }

        const strippedRows = rows.map(row => {
            const clone = { ...row };
            for (const key of HOST_COLUMN_KEYS) {
                delete clone[key];
            }
            return clone;
        });

        console.log('Structured host columns not found in perf_results; retrying upload with host metadata in meta.host only.');
        await uploadChunked(strippedRows);
    }

    return true;
}

async function main() {
    const args = parseArgs(process.argv.slice(2));
    const source = String(args.source ?? '').trim();
    const input = String(args.input ?? '').trim();

    if (!source) {
        console.error('Missing required argument: --source');
        process.exit(1);
    }

    const base = getBaseContext();
    const hostMetadata = getHostMetadata();
    let rows = [];

    if (source === 'benchmarkdotnet') {
        rows = parseBenchmarkDotNetResults(input || path.join('tests', 'performance', 'Benchmarks', 'BenchmarkDotNet.Artifacts', 'results'), base, hostMetadata);
    } else if (source === 'prime-script') {
        rows = parsePrimeResults(input || path.join('tests', 'performance', 'results.json'), base, hostMetadata);
    } else {
        console.error(`Unsupported source: ${source}`);
        process.exit(1);
    }

    if (rows.length === 0) {
        console.log(`No rows to ingest for source '${source}'.`);
        return;
    }

    const dedupedRows = dedupeRowsForUpsert(rows);
    if (dedupedRows.length !== rows.length) {
        console.log(`Deduplicated ${rows.length - dedupedRows.length} rows with identical upsert keys before upload.`);
    }

    const uploaded = await upsertRows(dedupedRows);
    if (!uploaded) {
        console.log(`Prepared ${dedupedRows.length} rows from '${source}' (upload skipped).`);
        return;
    }

    console.log(`Ingested ${dedupedRows.length} rows to Supabase from '${source}'.`);
}

main().catch(error => {
    console.error(error.message);
    process.exit(1);
});
