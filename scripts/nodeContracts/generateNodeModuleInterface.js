#!/usr/bin/env node

const crypto = require('crypto');
const childProcess = require('child_process');
const fs = require('fs');
const path = require('path');

const repoRoot = path.resolve(__dirname, '..', '..');
const contractDefinitions = [
    {
        flag: null,
        kind: 'fs',
        moduleSpecifier: 'fs',
        documentationPrefix: 'fs.',
        interfaceName: 'IFsModule',
        intrinsicClassName: 'FS',
        displayName: 'node:fs',
        outputStem: 'Fs',
        overrideStem: 'fs',
        lockStem: 'fs',
        contractAlias: 'FsContract',
        documentationModule: 'fs'
    },
    {
        flag: '--promises',
        kind: 'fs-promises',
        moduleSpecifier: 'fs/promises',
        documentationPrefix: 'fsPromises.',
        interfaceName: 'IFsPromisesModule',
        intrinsicClassName: 'FSPromises',
        displayName: 'node:fs/promises',
        outputStem: 'FsPromises',
        overrideStem: 'fsPromises',
        lockStem: 'fs',
        contractAlias: 'FsContract',
        documentationModule: 'fs'
    },
    {
        flag: '--console',
        kind: 'console',
        moduleSpecifier: 'console',
        documentationPrefix: 'console.',
        interfaceName: 'IConsoleModule',
        intrinsicClassName: 'ConsoleModule',
        displayName: 'node:console',
        outputStem: 'Console',
        overrideStem: 'console',
        lockStem: 'console',
        contractAlias: 'ConsoleContract',
        documentationModule: 'console'
    },
    {
        flag: '--path',
        kind: 'top-level-optional-rest',
        moduleSpecifier: 'path',
        documentationPrefix: 'path.',
        interfaceName: 'IPathModule',
        intrinsicClassName: 'Path',
        displayName: 'node:path',
        outputStem: 'Path',
        overrideStem: 'path',
        lockStem: 'path',
        contractAlias: 'PathContract',
        documentationModule: 'path',
        methodGroupHeading: 'Path methods'
    },
    {
        flag: '--child-process',
        kind: 'child-process',
        moduleSpecifier: 'child_process',
        documentationPrefix: 'child_process.',
        interfaceName: 'IChildProcessModule',
        intrinsicClassName: 'ChildProcess',
        displayName: 'node:child_process',
        outputStem: 'ChildProcess',
        overrideStem: 'childProcess',
        lockStem: 'childProcess',
        contractAlias: 'ChildProcessContract',
        documentationModule: 'child_process'
    },
    {
        flag: '--perf-hooks',
        kind: 'perf-hooks',
        moduleSpecifier: 'perf_hooks',
        documentationPrefix: 'perf_hooks.',
        interfaceName: 'IPerfHooksModule',
        intrinsicClassName: 'PerfHooks',
        displayName: 'node:perf_hooks',
        outputStem: 'PerfHooks',
        overrideStem: 'perfHooks',
        lockStem: 'perfHooks',
        contractAlias: 'PerfHooksContract',
        documentationModule: 'perf_hooks'
    },
    {
        flag: '--process',
        kind: 'process',
        moduleSpecifier: 'process',
        documentationPrefix: 'process.',
        interfaceName: 'IProcessModule',
        intrinsicClassName: 'Process',
        displayName: 'node:process',
        outputStem: 'Process',
        overrideStem: 'process',
        lockStem: 'process',
        contractAlias: 'ProcessContract',
        documentationModule: 'process',
        rootCategory: 'globals'
    },
    {
        flag: '--buffer',
        kind: 'buffer',
        moduleSpecifier: 'buffer',
        documentationPrefix: 'buffer.',
        interfaceName: 'IBufferModule',
        intrinsicClassName: 'BufferModule',
        displayName: 'node:buffer',
        outputStem: 'Buffer',
        overrideStem: 'buffer',
        lockStem: 'buffer',
        contractAlias: 'BufferContract',
        documentationModule: 'buffer'
    },
    {
        flag: '--events',
        kind: 'events',
        moduleSpecifier: 'events',
        documentationPrefix: 'events.',
        interfaceName: 'IEventsModule',
        intrinsicClassName: 'Events',
        displayName: 'node:events',
        outputStem: 'Events',
        overrideStem: 'events',
        lockStem: 'events',
        contractAlias: 'EventsContract',
        documentationModule: 'events'
    },
    {
        flag: '--os',
        kind: 'top-level-overloads',
        moduleSpecifier: 'os',
        documentationPrefix: 'os.',
        interfaceName: 'IOsModule',
        intrinsicClassName: 'OS',
        displayName: 'node:os',
        outputStem: 'Os',
        overrideStem: 'os',
        lockStem: 'os',
        contractAlias: 'OsContract',
        documentationModule: 'os',
        methodGroupHeading: 'Operating system methods'
    },
    {
        flag: '--stream',
        kind: 'stream',
        moduleSpecifier: 'stream',
        documentationPrefix: 'stream.',
        interfaceName: 'IStreamModule',
        intrinsicClassName: 'Stream',
        displayName: 'node:stream',
        outputStem: 'Stream',
        overrideStem: 'stream',
        lockStem: 'stream',
        contractAlias: 'StreamContract',
        documentationModule: 'stream'
    },
    {
        flag: '--stream-promises',
        kind: 'stream-promises',
        moduleSpecifier: 'stream/promises',
        documentationPrefix: 'stream.',
        interfaceName: 'IStreamPromisesModule',
        intrinsicClassName: 'StreamPromises',
        displayName: 'node:stream/promises',
        outputStem: 'StreamPromises',
        overrideStem: 'streamPromises',
        lockStem: 'stream',
        contractAlias: 'StreamPromisesContract',
        documentationModule: 'stream'
    },
    {
        flag: '--util',
        kind: 'util',
        moduleSpecifier: 'util',
        documentationPrefix: 'util.',
        interfaceName: 'IUtilModule',
        intrinsicClassName: 'Util',
        displayName: 'node:util',
        outputStem: 'Util',
        overrideStem: 'util',
        lockStem: 'util',
        contractAlias: 'UtilContract',
        documentationModule: 'util'
    },
    {
        flag: '--util-types',
        kind: 'util-types',
        moduleSpecifier: 'util/types',
        documentationPrefix: 'util.types.',
        interfaceName: 'IUtilTypesModule',
        intrinsicClassName: 'UtilTypesModule',
        displayName: 'node:util/types',
        outputStem: 'UtilTypes',
        overrideStem: 'utilTypes',
        lockStem: 'util',
        contractAlias: 'UtilTypesContract',
        documentationModule: 'util'
    },
    {
        flag: '--zlib',
        kind: 'zlib',
        moduleSpecifier: 'zlib',
        documentationPrefix: 'zlib.',
        interfaceName: 'IZlibModule',
        intrinsicClassName: 'Zlib',
        displayName: 'node:zlib',
        outputStem: 'Zlib',
        overrideStem: 'zlib',
        lockStem: 'zlib',
        contractAlias: 'ZlibContract',
        documentationModule: 'zlib'
    },
    {
        flag: '--string-decoder',
        kind: 'string-decoder',
        moduleSpecifier: 'string_decoder',
        documentationPrefix: 'string_decoder.',
        interfaceName: 'IStringDecoderModule',
        intrinsicClassName: 'StringDecoderModule',
        displayName: 'node:string_decoder',
        outputStem: 'StringDecoder',
        overrideStem: 'stringDecoder',
        lockStem: 'stringDecoder',
        contractAlias: 'StringDecoderContract',
        documentationModule: 'string_decoder'
    },
    {
        flag: '--timers',
        kind: 'timers',
        moduleSpecifier: 'timers',
        documentationPrefix: 'timers.',
        interfaceName: 'ITimersModule',
        intrinsicClassName: 'TimersModule',
        displayName: 'node:timers',
        outputStem: 'Timers',
        overrideStem: 'timers',
        lockStem: 'timers',
        contractAlias: 'TimersContract',
        documentationModule: 'timers'
    },
    {
        flag: '--timers-promises',
        kind: 'timers-promises',
        moduleSpecifier: 'timers/promises',
        documentationPrefix: 'timersPromises.',
        interfaceName: 'ITimersPromisesModule',
        intrinsicClassName: 'TimersPromises',
        displayName: 'node:timers/promises',
        outputStem: 'TimersPromises',
        overrideStem: 'timersPromises',
        lockStem: 'timers',
        contractAlias: 'TimersPromisesContract',
        documentationModule: 'timers'
    },
    {
        flag: '--url',
        kind: 'normalized-api',
        moduleSpecifier: 'url',
        documentationPrefix: 'url.',
        interfaceName: 'IUrlModule',
        intrinsicClassName: 'Url',
        displayName: 'node:url',
        outputStem: 'Url',
        overrideStem: 'url',
        lockStem: 'url',
        contractAlias: 'UrlContract',
        documentationModule: 'url',
        methodSections: ['the_whatwg_url_api', 'legacy_url_api']
    },
    {
        flag: '--querystring',
        kind: 'normalized-api',
        moduleSpecifier: 'querystring',
        documentationPrefix: 'querystring.',
        interfaceName: 'IQueryStringModule',
        intrinsicClassName: 'QueryString',
        displayName: 'node:querystring',
        outputStem: 'QueryString',
        overrideStem: 'querystring',
        lockStem: 'querystring',
        contractAlias: 'QueryStringContract',
        documentationModule: 'querystring'
    },
    {
        flag: '--assert',
        kind: 'normalized-api',
        moduleSpecifier: 'assert',
        documentationPrefix: 'assert.',
        interfaceName: 'IAssertModule',
        intrinsicClassName: 'AssertModule',
        displayName: 'node:assert',
        outputStem: 'Assert',
        overrideStem: 'assert',
        lockStem: 'assert',
        contractAlias: 'AssertContract',
        documentationModule: 'assert'
    },
    {
        flag: '--async-hooks',
        kind: 'normalized-api',
        moduleSpecifier: 'async_hooks',
        documentationPrefix: 'async_hooks.',
        interfaceName: 'IAsyncHooksModule',
        intrinsicClassName: 'AsyncHooks',
        displayName: 'node:async_hooks',
        outputStem: 'AsyncHooks',
        overrideStem: 'asyncHooks',
        lockStem: 'asyncHooks',
        contractAlias: 'AsyncHooksContract',
        documentationModule: 'async_hooks',
        secondaryDocumentationModule: 'async_context',
        methodClasses: ['AsyncHook'],
        selectedMethodPrefix: '`async_hooks.'
    },
    {
        flag: '--dns',
        kind: 'normalized-api',
        moduleSpecifier: 'dns',
        documentationPrefix: 'dns.',
        interfaceName: 'IDnsModule',
        intrinsicClassName: 'Dns',
        displayName: 'node:dns',
        outputStem: 'Dns',
        overrideStem: 'dns',
        lockStem: 'dns',
        contractAlias: 'DnsContract',
        documentationModule: 'dns'
    },
    {
        flag: '--diagnostics-channel',
        kind: 'normalized-api',
        moduleSpecifier: 'diagnostics_channel',
        documentationPrefix: 'diagnostics_channel.',
        interfaceName: 'IDiagnosticsChannelModule',
        intrinsicClassName: 'DiagnosticsChannel',
        displayName: 'node:diagnostics_channel',
        outputStem: 'DiagnosticsChannel',
        overrideStem: 'diagnosticsChannel',
        lockStem: 'diagnosticsChannel',
        contractAlias: 'DiagnosticsChannelContract',
        documentationModule: 'diagnostics_channel',
        methodSections: ['public_api/overview']
    },
    {
        flag: '--net',
        kind: 'normalized-api',
        moduleSpecifier: 'net',
        documentationPrefix: 'net.',
        interfaceName: 'INetModule',
        intrinsicClassName: 'Net',
        displayName: 'node:net',
        outputStem: 'Net',
        overrideStem: 'net',
        lockStem: 'net',
        contractAlias: 'NetContract',
        documentationModule: 'net'
    },
    {
        flag: '--tls',
        kind: 'normalized-api',
        moduleSpecifier: 'tls',
        documentationPrefix: 'tls.',
        interfaceName: 'ITlsModule',
        intrinsicClassName: 'Tls',
        displayName: 'node:tls',
        outputStem: 'Tls',
        overrideStem: 'tls',
        lockStem: 'tls',
        contractAlias: 'TlsContract',
        documentationModule: 'tls'
    },
    {
        flag: '--http',
        kind: 'documented-api',
        moduleSpecifier: 'http',
        documentationPrefix: 'http.',
        interfaceName: 'IHttpModule',
        intrinsicClassName: 'Http',
        displayName: 'node:http',
        outputStem: 'Http',
        overrideStem: 'http',
        lockStem: 'http',
        contractAlias: 'HttpContract',
        documentationModule: 'http'
    },
    {
        flag: '--https',
        kind: 'documented-api',
        moduleSpecifier: 'https',
        documentationPrefix: 'https.',
        interfaceName: 'IHttpsModule',
        intrinsicClassName: 'Https',
        displayName: 'node:https',
        outputStem: 'Https',
        overrideStem: 'https',
        lockStem: 'https',
        contractAlias: 'HttpsContract',
        documentationModule: 'https'
    },
    {
        flag: '--crypto',
        kind: 'documented-api',
        moduleSpecifier: 'crypto',
        documentationPrefix: 'crypto.',
        interfaceName: 'ICryptoModule',
        intrinsicClassName: 'Crypto',
        displayName: 'node:crypto',
        outputStem: 'Crypto',
        overrideStem: 'crypto',
        lockStem: 'crypto',
        contractAlias: 'CryptoContract',
        documentationModule: 'crypto',
        methodSection: '`node:crypto`_module_methods_and_properties'
    }
];
const args = process.argv.slice(2);

if (args.includes('--all')) {
    const unknownArguments = args.filter(argument => !['--all', '--check'].includes(argument));
    if (unknownArguments.length > 0) {
        throw new Error(`--all cannot be combined with '${unknownArguments[0]}'.`);
    }

    for (const definition of contractDefinitions) {
        const childArguments = [
            __filename,
            ...(definition.flag ? [definition.flag] : []),
            ...(args.includes('--check') ? ['--check'] : [])
        ];
        const result = childProcess.spawnSync(process.execPath, childArguments, {
            cwd: repoRoot,
            stdio: 'inherit'
        });
        if (result.status !== 0) {
            process.exit(result.status ?? 1);
        }
    }
    process.exit(0);
}

const modeFlags = contractDefinitions
    .map(definition => definition.flag)
    .filter(Boolean);

for (let index = 0; index < args.length; index++) {
    const argument = args[index];
    if (argument === '--input') {
        if (++index >= args.length) {
            throw new Error('--input requires a file path.');
        }
        continue;
    }
    if (argument === '--secondary-input') {
        if (++index >= args.length) {
            throw new Error('--secondary-input requires a file path.');
        }
        continue;
    }

    if (![
        '--check',
        ...modeFlags
    ].includes(argument)) {
        throw new Error(`Unknown argument '${argument}'.`);
    }
}

const selectedContracts = contractDefinitions.filter(
    definition => definition.flag && args.includes(definition.flag));
if (selectedContracts.length > 1) {
    throw new Error(
        `${modeFlags.join(', ')} cannot be used together.`);
}

const contract = selectedContracts[0] ?? contractDefinitions[0];
const lockPath = path.join(__dirname, `${contract.lockStem}.node24.lock.json`);
const overridesPath = path.join(__dirname, `${contract.overrideStem}.node24.overrides.json`);
const interfaceOutputPath = path.join(
    repoRoot,
    'src',
    'JavaScriptRuntime',
    'Node',
    'Contracts',
    `I${contract.outputStem}Module.Generated.cs`);
const intrinsicImplementationOutputPath = path.join(
    repoRoot,
    'src',
    'JavaScriptRuntime',
    'Node',
    `${contract.intrinsicClassName}.I${contract.outputStem}Module.Generated.cs`);

const lock = JSON.parse(fs.readFileSync(lockPath, 'utf8'));
const overrides = JSON.parse(fs.readFileSync(overridesPath, 'utf8'));
const contractAlias = contract.contractAlias;
const documentationModule = contract.documentationModule;
const generatorSource = fs.readFileSync(__filename, 'utf8').replaceAll('\r\n', '\n');
const generatorSha256 = crypto
    .createHash('sha256')
    .update(generatorSource)
    .digest('hex');
const checkOnly = args.includes('--check');
const inputIndex = args.indexOf('--input');
const inputPath = inputIndex >= 0 ? args[inputIndex + 1] : null;
const secondaryInputIndex = args.indexOf('--secondary-input');
const secondaryInputPath = secondaryInputIndex >= 0 ? args[secondaryInputIndex + 1] : null;

if (inputIndex >= 0 && !inputPath) {
    throw new Error('--input requires a file path.');
}

async function loadDocumentation() {
    if (inputPath) {
        return fs.readFileSync(path.resolve(inputPath));
    }

    const response = await fetch(lock.sourceUrl);
    if (!response.ok) {
        throw new Error(`Failed to download ${lock.sourceUrl}: HTTP ${response.status}`);
    }

    return Buffer.from(await response.arrayBuffer());
}

async function loadSecondaryDocumentation() {
    if (!contract.secondaryDocumentationModule) {
        return null;
    }
    if (secondaryInputPath) {
        return fs.readFileSync(path.resolve(secondaryInputPath));
    }

    const response = await fetch(lock.secondarySourceUrl);
    if (!response.ok) {
        throw new Error(
            `Failed to download ${lock.secondarySourceUrl}: HTTP ${response.status}`);
    }

    return Buffer.from(await response.arrayBuffer());
}

function requireSection(module, sectionName) {
    const section = sectionName.split('/').reduce(
        (container, name) => container?.modules?.find(candidate => candidate.name === name),
        module);
    if (!section) {
        throw new Error(
            `Official ${contract.moduleSpecifier} documentation is missing the '${sectionName}' section.`);
    }

    return section;
}

function requireMiscSection(container, sectionName) {
    const section = container.miscs?.find(candidate => candidate.name === sectionName);
    if (!section) {
        throw new Error(
            `Official ${contract.moduleSpecifier} documentation is missing the '${sectionName}' section.`);
    }

    return section;
}

function assertCount(actual, expected, description) {
    if (actual !== expected) {
        throw new Error(
            `Official ${contract.moduleSpecifier} documentation ${description} changed: ` +
            `expected ${expected}, found ${actual}. ` +
            'Review the Node.js API change and update the generator and lock intentionally.');
    }
}

function collectDocumentedLegacyZlibConstants(module) {
    const names = new Set();

    function visit(value) {
        if (!value || typeof value !== 'object') {
            return;
        }

        if (typeof value.desc === 'string') {
            for (const match of value.desc.matchAll(/zlib\.constants\.([A-Za-z0-9_]+)/g)) {
                if (!match[1].startsWith('BROTLI')) {
                    names.add(match[1]);
                }
            }
        }

        for (const child of Object.values(value)) {
            if (child && typeof child === 'object') {
                visit(child);
            }
        }
    }

    visit(module);
    return [...names].sort();
}

function expandOptionalSegments(value) {
    const start = value.indexOf('[');
    if (start < 0) {
        return [value];
    }

    let depth = 0;
    let end = -1;
    for (let index = start; index < value.length; index++) {
        if (value[index] === '[') {
            depth++;
        } else if (value[index] === ']') {
            depth--;
            if (depth === 0) {
                end = index;
                break;
            }
        }
    }

    if (end < 0) {
        throw new Error(`Unbalanced optional segment in '${value}'.`);
    }

    const prefix = value.slice(0, start);
    const optional = value.slice(start + 1, end);
    const suffix = value.slice(end + 1);

    return [
        ...expandOptionalSegments(prefix + suffix),
        ...expandOptionalSegments(prefix + optional + suffix)
    ];
}

function extractSignature(method, documentationPrefix = contract.documentationPrefix) {
    const signature = method.textRaw.replace(/^`|`$/g, '');
    const openParen = signature.indexOf('(');
    const closeParen = signature.lastIndexOf(')');
    if (openParen < 0 || closeParen < openParen) {
        throw new Error(`Cannot parse official Node.js signature '${method.textRaw}'.`);
    }

    return {
        signature,
        memberName: signature.slice(0, openParen).replace(documentationPrefix, ''),
        parameters: signature.slice(openParen + 1, closeParen)
    };
}

function optionalParameterNames(parameters) {
    const names = new Set();
    let depth = 0;
    let token = '';
    let tokenIsOptional = false;

    function addToken() {
        const name = token.trim();
        if (name && tokenIsOptional) {
            names.add(name);
        }
        token = '';
        tokenIsOptional = false;
    }

    for (const character of parameters) {
        if (character === '[') {
            depth++;
        } else if (character === ']') {
            depth--;
        } else if (character === ',') {
            addToken();
        } else {
            if (token.trim().length === 0 && !/\s/.test(character)) {
                tokenIsOptional = depth > 0;
            }
            token += character;
        }
    }
    addToken();

    if (depth !== 0) {
        throw new Error(`Unbalanced optional segment in '${parameters}'.`);
    }

    return names;
}

function getOptionalParameterNames(memberName, parameters, descriptors, configuration = overrides) {
    const names = optionalParameterNames(parameters);
    const override = configuration.methodOptionalParameters?.[memberName];
    if (!override) {
        return names;
    }

    if (!Array.isArray(override.parameters) || !override.source) {
        throw new Error(
            `Optional parameter override for '${contract.documentationPrefix}${memberName}' ` +
            'must include parameters and source.');
    }

    const descriptorNames = new Set(descriptors.map(parameter => parameter.name));
    for (const parameterName of override.parameters) {
        if (!descriptorNames.has(parameterName)) {
            throw new Error(
                `Optional parameter override for '${contract.documentationPrefix}${memberName}' ` +
                `references unknown parameter '${parameterName}'.`);
        }
        names.add(parameterName);
    }

    return names;
}

function resolveContractType(type) {
    const value = String(type ?? '');
    if (!value.startsWith('contract:')) {
        return null;
    }

    const reference = value.slice('contract:'.length);
    const separator = reference.lastIndexOf('/');
    const moduleSpecifier = separator < 0
        ? contract.moduleSpecifier
        : reference.slice(0, separator);
    const typeName = separator < 0 ? reference : reference.slice(separator + 1);
    const definition = contractDefinitions.find(
        candidate => candidate.moduleSpecifier === moduleSpecifier);
    if (!definition) {
        throw new Error(
            `Unknown nested contract module '${moduleSpecifier}' referenced by '${value}'.`);
    }

    const referencedOverrides = definition === contract
        ? overrides
        : JSON.parse(fs.readFileSync(
            path.join(__dirname, `${definition.overrideStem}.node24.overrides.json`),
            'utf8'));
    const nestedContract = (referencedOverrides.nestedContracts ?? [])
        .find(candidate => candidate.typeName === typeName);
    if (!nestedContract?.interfaceName) {
        throw new Error(
            `Unknown nested contract type '${typeName}' for node:${moduleSpecifier}.`);
    }

    return `global::Jroc.Runtime.Node.Contracts.${nestedContract.interfaceName}`;
}

function mapType(type, isReturnType = false) {
    const contractType = resolveContractType(type);
    if (contractType) {
        return contractType;
    }

    const normalized = String(type ?? '')
        .replaceAll('\\', '')
        .replace(/\s+/g, '')
        .toLowerCase();
    if (!normalized || normalized === 'undefined') {
        return isReturnType ? 'void' : 'object?';
    }

    const unionTypes = normalized.split('|');
    if (unionTypes.length > 1) {
        if (unionTypes.includes('null') || unionTypes.includes('undefined')) {
            return 'object?';
        }

        const mappedTypes = new Set(unionTypes.map(unionType => mapType(unionType)));
        return mappedTypes.size === 1 ? mappedTypes.values().next().value : 'object?';
    }

    if (normalized === 'number' || normalized === 'integer') {
        return 'double';
    }

    if (normalized === 'boolean') {
        return 'bool';
    }

    if (normalized === 'string') {
        return 'string';
    }

    if (normalized === 'bigint') {
        return 'global::System.Numerics.BigInteger';
    }

    if (normalized === 'function' || normalized === 'eventlistener') {
        return 'global::System.Delegate';
    }

    if (normalized === 'symbol') {
        return 'global::JavaScriptRuntime.Symbol';
    }

    if (normalized === 'null') {
        return 'global::JavaScriptRuntime.JsNull';
    }

    if (normalized === 'buffer') {
        return 'global::JavaScriptRuntime.Node.Buffer';
    }

    if (normalized === 'arraybuffer') {
        return 'global::JavaScriptRuntime.ArrayBuffer';
    }

    if (normalized === 'sharedarraybuffer') {
        return 'global::JavaScriptRuntime.SharedArrayBuffer';
    }

    if (normalized === 'dataview') {
        return 'global::JavaScriptRuntime.DataView';
    }

    if (normalized === 'date') {
        return 'global::JavaScriptRuntime.Date';
    }

    if (normalized === 'regexp') {
        return 'global::JavaScriptRuntime.RegExp';
    }

    if (normalized.startsWith('promise')) {
        return 'global::JavaScriptRuntime.IJavaScriptPromise';
    }

    if (normalized === 'iterator' || normalized === 'iterable') {
        return 'global::JavaScriptRuntime.IJavaScriptIterator';
    }

    if (normalized === 'asynciterator' || normalized === 'asynciterable') {
        return 'global::JavaScriptRuntime.IJavaScriptAsyncIterator';
    }

    if (normalized.endsWith('[]') || normalized.startsWith('array<') || normalized === 'array') {
        return 'global::JavaScriptRuntime.IJavaScriptArray';
    }

    return 'object?';
}

const csharpKeywords = new Set([
    'base', 'bool', 'break', 'byte', 'case', 'catch', 'char', 'checked', 'class',
    'const', 'continue', 'decimal', 'default', 'delegate', 'do', 'double', 'else',
    'enum', 'event', 'explicit', 'extern', 'false', 'finally', 'fixed', 'float',
    'for', 'foreach', 'goto', 'if', 'implicit', 'in', 'int', 'interface', 'internal',
    'is', 'lock', 'long', 'namespace', 'new', 'null', 'object', 'operator', 'out',
    'override', 'params', 'private', 'protected', 'public', 'readonly', 'ref',
    'return', 'sbyte', 'sealed', 'short', 'sizeof', 'stackalloc', 'static', 'string',
    'struct', 'switch', 'this', 'throw', 'true', 'try', 'typeof', 'uint', 'ulong',
    'unchecked', 'unsafe', 'ushort', 'using', 'virtual', 'void', 'volatile', 'while'
]);

function csharpIdentifier(name) {
    const sanitized = name.replace(/[^A-Za-z0-9_]/g, '_');
    const identifier = /^[0-9]/.test(sanitized) ? `_${sanitized}` : sanitized;
    return csharpKeywords.has(identifier) ? `@${identifier}` : identifier;
}

function csharpMemberName(nodeMemberName) {
    const parts = nodeMemberName.split('.');
    const memberName = parts[0] + parts.slice(1)
        .map(part => part.length === 0 ? '' : part[0].toUpperCase() + part.slice(1))
        .join('');
    return csharpIdentifier(memberName);
}

function xmlEscape(value) {
    return value
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;');
}

function getMemberContractMetadata(memberName, parameters, configuration = overrides) {
    const parameterContracts = configuration.parameterContracts?.[memberName] ?? {};
    const parameterAttributes = new Map();
    for (const [parameterName, parameterContract] of Object.entries(parameterContracts)) {
        const contractType = resolveContractType(parameterContract.type);
        if (!contractType || !parameterContract.source) {
            throw new Error(
                `Parameter contract '${memberName}.${parameterName}' must include a nested contract type and source.`);
        }
        parameterAttributes.set(
            parameterName,
            `[global::Jroc.Runtime.Node.Contracts.NodeModuleParameterContract(typeof(${contractType}))]`);
    }

    const resultContracts = configuration.resultContracts?.[memberName] ?? [];
    const normalizedResultContracts = Array.isArray(resultContracts)
        ? resultContracts
        : [resultContracts];
    const resultAttributes = normalizedResultContracts.map(resultContract => {
        const contractType = resolveContractType(resultContract.type)
            ?? mapType(resultContract.type);
        const kind = {
            promise: 'Promise',
            callback: 'Callback',
            iterator: 'Iterator',
            'async-iterator': 'AsyncIterator'
        }[resultContract.kind];
        if (!kind || !contractType || !resultContract.source) {
            throw new Error(
                `Result contract '${memberName}' must include a supported kind, type, and source.`);
        }
        const callbackParameter = resultContract.kind === 'callback'
            ? `, "${resultContract.callbackParameter ?? 'callback'}"`
            : '';
        return `[global::Jroc.Runtime.Node.Contracts.NodeModuleResultContract(` +
            `global::Jroc.Runtime.Node.Contracts.NodeModuleResultKind.${kind}, typeof(${contractType})${callbackParameter})]`;
    });

    return { parameterAttributes, resultAttributes };
}

function generateMethodOverloads(
    methods,
    configuration = overrides,
    documentationPrefix = contract.documentationPrefix) {
    const generated = [];
    const signatures = new Set();

    for (const method of methods) {
        const parsed = extractSignature(method, documentationPrefix);
        const signature = method.signatures?.[0];
        if (!signature) {
            throw new Error(`Official Node.js method '${parsed.signature}' has no structured signature.`);
        }

        const descriptors = new Map(
            (signature.params ?? []).map(parameter => [parameter.name, parameter]));
        const returnType = mapMethodReturnType(parsed.memberName, signature, configuration);
        const optionalNames = getOptionalParameterNames(
            parsed.memberName,
            parsed.parameters,
            signature.params ?? [],
            configuration);
        const contractMetadata = getMemberContractMetadata(
            parsed.memberName,
            signature.params ?? [],
            configuration);

        for (const expanded of expandOptionalSegments(parsed.parameters)) {
            const parameterNames = expanded
                .split(',')
                .map(name => name.trim())
                .filter(Boolean);
            const parameters = parameterNames.map(name => {
                const descriptor = descriptors.get(name);
                if (!descriptor) {
                    throw new Error(
                        `Official Node.js signature '${parsed.signature}' references unknown parameter '${name}'.`);
                }

                return {
                    name: csharpIdentifier(name),
                    type: optionalNames.has(name) ? 'object?' : mapType(descriptor.type),
                    attribute: contractMetadata.parameterAttributes.get(name)
                };
            });

            const methodName = csharpMemberName(parsed.memberName);
            const signatureKey = `${methodName}(${parameters.map(parameter => parameter.type).join(',')})`;
            if (signatures.has(signatureKey)) {
                continue;
            }
            signatures.add(signatureKey);

            generated.push([
                '    /// <summary>',
                `    /// Node.js signature: <c>${xmlEscape(parsed.signature)}</c>.`,
                '    /// </summary>',
                ...(method.meta?.deprecated
                    ? [`    [global::System.Obsolete("Deprecated by Node.js since ${method.meta.deprecated.join(', ')}.")]`]
                    : []),
                ...contractMetadata.resultAttributes.map(attribute => `    ${attribute}`),
                `    [NodeModuleMember("${parsed.memberName}")]`,
                `    ${returnType} ${methodName}(${parameters.map(parameter =>
                    `${parameter.attribute ? `${parameter.attribute} ` : ''}${parameter.type} ${parameter.name}`).join(', ')});`
            ].join('\n'));
        }
    }

    return generated;
}

function generateMethodsWithOptionalAndRestParameters(
    methods,
    configuration = overrides,
    documentationPrefix = contract.documentationPrefix) {
    const generated = [];
    const signatures = new Set();

    for (const method of methods) {
        const parsed = extractSignature(method, documentationPrefix);
        const signature = method.signatures?.[0];
        if (!signature) {
            throw new Error(`Official Node.js method '${parsed.signature}' has no structured signature.`);
        }

        const descriptors = signature.params ?? [];
        const optionalNames = getOptionalParameterNames(
            parsed.memberName,
            parsed.parameters,
            descriptors,
            configuration);
        const restParameter = descriptors.find(parameter => parameter.name.startsWith('...'));
        const positionalParameters = descriptors.filter(
            parameter => !parameter.name.startsWith('...'));
        const requiredCount = positionalParameters.filter(
            parameter => !optionalNames.has(parameter.name)).length;
        const returnType = mapMethodReturnType(parsed.memberName, signature, configuration);
        const methodName = csharpMemberName(parsed.memberName);
        const contractMetadata = getMemberContractMetadata(
            parsed.memberName,
            descriptors,
            configuration);

        for (let parameterCount = requiredCount;
            parameterCount <= positionalParameters.length;
            parameterCount++) {
            const parameters = positionalParameters
                .slice(0, parameterCount)
                .map(parameter => {
                    const type = optionalNames.has(parameter.name)
                        ? 'object?'
                        : mapType(parameter.type);
                    const attribute = contractMetadata.parameterAttributes.get(parameter.name);
                    return `${attribute ? `${attribute} ` : ''}${type} ${csharpIdentifier(parameter.name)}`;
                });

            if (restParameter && parameterCount === positionalParameters.length) {
                parameters.push(`params object?[] ${csharpIdentifier(restParameter.name.slice(3))}`);
            }

            const signatureKey = `${methodName}(${parameters
                .map(parameter => parameter.replace(
                    /^(params )?(.+?) @?[A-Za-z_][A-Za-z0-9_]*$/,
                    '$1$2'))
                .join(',')})`;
            if (signatures.has(signatureKey)) {
                continue;
            }
            signatures.add(signatureKey);

            generated.push([
                '    /// <summary>',
                `    /// Node.js signature: <c>${xmlEscape(parsed.signature)}</c>.`,
                '    /// </summary>',
                ...(method.meta?.deprecated
                    ? [`    [global::System.Obsolete("Deprecated by Node.js since ${method.meta.deprecated.join(', ')}.")]`]
                    : []),
                ...contractMetadata.resultAttributes.map(attribute => `    ${attribute}`),
                `    [NodeModuleMember("${parsed.memberName}")]`,
                `    ${returnType} ${methodName}(${parameters.join(', ')});`
            ].join('\n'));
        }
    }

    return generated;
}

function generateNormalizedMethods() {
    const methods = normalizedMethodOverloads();
    return methods.map(method => {
        if (!method.name
            || !Array.isArray(method.signatures)
            || method.signatures.length === 0
            || !Array.isArray(method.parameters)
            || !method.returnType
            || !method.source) {
            throw new Error(
                `Normalized method for ${contract.moduleSpecifier} must include ` +
                'name, signatures, parameters, returnType, and source.');
        }

        const parameters = method.parameters.map(parameter => {
            if (!parameter.name || !parameter.type) {
                throw new Error(
                    `Normalized method '${method.name}' has an incomplete parameter.`);
            }

            if (parameter.rest) {
                return `params object?[] ${csharpIdentifier(parameter.name)}`;
            }

            const type = parameter.optional ? 'object?' : mapType(parameter.type);
            return `${type} ${csharpIdentifier(parameter.name)}`;
        });

        return [
            '    /// <summary>',
            `    /// Node.js ${method.signatures.length === 1 ? 'signature' : 'signatures'}: ` +
                method.signatures
                    .map(signature => `<c>${xmlEscape(signature)}</c>`)
                    .join(', ') + '.',
            '    /// </summary>',
            `    /// <remarks>Source: <c>${xmlEscape(method.source)}</c>.</remarks>`,
            ...(method.deprecated
                ? [`    [global::System.Obsolete("${method.deprecated}")]`]
                : []),
            `    [NodeModuleMember("${method.name}")]`,
            `    ${mapType(method.returnType, true)} ${csharpMemberName(method.name)}(${parameters.join(', ')});`
        ].join('\n');
    });
}

function normalizedMethodOverloads() {
    return (overrides.normalizedMethods ?? []).flatMap(method => {
        if (method.minimumParameterCount !== undefined) {
            if (!Number.isInteger(method.minimumParameterCount)
                || method.minimumParameterCount < 0
                || method.minimumParameterCount > (method.parameters?.length ?? -1)) {
                throw new Error(
                    `Normalized method '${method.name}' has an invalid minimumParameterCount.`);
            }

            return Array.from(
                { length: method.parameters.length - method.minimumParameterCount + 1 },
                (_, index) => ({
                    ...method,
                    parameters: method.parameters.slice(
                        0,
                        method.minimumParameterCount + index),
                    minimumParameterCount: undefined
                }));
        }

        if (!method.overloads) {
            return [method];
        }

        if (!Array.isArray(method.overloads) || method.overloads.length === 0) {
            throw new Error(
                `Normalized method '${method.name}' must include at least one overload.`);
        }

        return method.overloads.map(overload => ({
            ...method,
            ...overload,
            signatures: overload.signatures ?? method.signatures,
            source: overload.source ?? method.source,
            overloads: undefined
        }));
    });
}

function removeNormalizedMethods(methods) {
    const normalizedNames = new Set(
        (overrides.normalizedMethods ?? []).map(method => method.name));
    const documentedNames = new Set(
        methods.map(method => extractSignature(method).memberName));
    for (const normalizedName of normalizedNames) {
        if (!documentedNames.has(normalizedName)) {
            throw new Error(
                `Normalized method '${contract.documentationPrefix}${normalizedName}' ` +
                'does not match a selected official method record.');
        }
    }

    return methods.filter(method => !normalizedNames.has(extractSignature(method).memberName));
}

function applyMethodSignatureOverrides(methods) {
    const signatureOverrides = overrides.methodSignatureOverrides ?? {};
    return methods.map(method => {
        const signatureOverride = signatureOverrides[method.textRaw];
        if (!signatureOverride) {
            return method;
        }

        if (!Array.isArray(signatureOverride.parameters)
            || !signatureOverride.returnType
            || !signatureOverride.source) {
            throw new Error(
                `Signature override for '${method.textRaw}' must include ` +
                'parameters, returnType, and source.');
        }

        const originalSignature = method.signatures?.[0] ?? {};
        return {
            ...method,
            signatures: [{
                ...originalSignature,
                params: signatureOverride.parameters,
                return: { type: signatureOverride.returnType }
            }]
        };
    });
}

function mapMethodReturnType(memberName, signature, configuration = overrides) {
    const override = configuration.methodReturnTypes?.[memberName];
    if (override && (!override.type || !override.source)) {
        throw new Error(
            `Return type override for '${contract.documentationPrefix}${memberName}' ` +
            'must include type and source.');
    }

    return mapType(override?.type ?? signature.return?.type, true);
}

function generateProperties(properties, configuration = overrides) {
    return properties.map(property => {
        const match = property.textRaw.match(/^`([^`]+)`(?: Type:)? \{([^}]+)\}/);
        if (!match) {
            throw new Error(
                `Cannot parse official Node.js property signature '${property.textRaw}'.`);
        }

        const [, propertyName, propertyType] = match;
        const access = configuration.propertyAccess?.[propertyName] ?? 'read-only';
        if (!['read-only', 'read-write'].includes(access)) {
            throw new Error(
                `Unsupported access '${access}' for ${contract.moduleSpecifier} property '${propertyName}'.`);
        }

        return [
            '    /// <summary>',
            `    /// Node.js property: <c>${xmlEscape(property.textRaw)}</c>.`,
            '    /// </summary>',
            ...(property.meta?.deprecated
                ? [`    [global::System.Obsolete("Deprecated by Node.js since ${property.meta.deprecated.join(', ')}.")]`]
                : []),
            `    [NodeModuleMember("${propertyName}")]`,
            `    ${mapType(propertyType)} ${csharpMemberName(propertyName)} { get;${access === 'read-write' ? ' set;' : ''} }`
        ].join('\n');
    });
}

function generateConfiguredNestedProperties(nestedContract) {
    return (nestedContract.properties ?? []).map(property => {
        if (!property.name || !property.type || !property.source) {
            throw new Error(
                `Nested contract '${nestedContract.typeName}' has an incomplete property definition.`);
        }

        const access = property.access ?? 'read-only';
        if (!['read-only', 'read-write'].includes(access)) {
            throw new Error(
                `Nested contract '${nestedContract.typeName}.${property.name}' has unsupported access '${access}'.`);
        }

        return [
            '    /// <summary>',
            `    /// ${property.summary ?? `Gets <c>${xmlEscape(property.name)}</c>.`}`,
            '    /// </summary>',
            `    /// <remarks>Source: <c>${xmlEscape(property.source)}</c>.</remarks>`,
            `    [NodeModuleMember("${property.name}")]`,
            `    ${mapType(property.type)} ${csharpMemberName(property.name)} { get;${access === 'read-write' ? ' set;' : ''} }`
        ].join('\n');
    });
}

function generateConfiguredNestedMethods(nestedContract) {
    return (nestedContract.methods ?? []).map(method => {
        if (!method.name
            || !Array.isArray(method.signatures)
            || method.signatures.length === 0
            || !Array.isArray(method.parameters)
            || !method.returnType
            || !method.source) {
            throw new Error(
                `Nested contract '${nestedContract.typeName}' has an incomplete method definition.`);
        }

        const metadata = getMemberContractMetadata(method.name, method.parameters, nestedContract);
        const parameters = method.parameters.map((parameter, index) => {
            if (!parameter.name || !parameter.type) {
                throw new Error(
                    `Nested contract '${nestedContract.typeName}.${method.name}' has an incomplete parameter.`);
            }
            if (parameter.rest && index !== method.parameters.length - 1) {
                throw new Error(
                    `Nested contract '${nestedContract.typeName}.${method.name}' has a non-final rest parameter.`);
            }

            const attribute = metadata.parameterAttributes.get(parameter.name);
            return `${attribute ? `${attribute} ` : ''}${parameter.rest ? 'params object?[]' : parameter.optional ? 'object?' : mapType(parameter.type)} ${csharpIdentifier(parameter.name)}`;
        });

        return [
            '    /// <summary>',
            `    /// Node.js ${method.signatures.length === 1 ? 'signature' : 'signatures'}: ` +
                method.signatures.map(signature => `<c>${xmlEscape(signature)}</c>`).join(', ') + '.',
            '    /// </summary>',
            `    /// <remarks>Source: <c>${xmlEscape(method.source)}</c>.</remarks>`,
            ...metadata.resultAttributes.map(attribute => `    ${attribute}`),
            `    [NodeModuleMember("${method.name}")]`,
            `    ${mapType(method.returnType, true)} ${csharpMemberName(method.name)}(${parameters.join(', ')});`
        ].join('\n');
    });
}

function resolveNestedDocumentationContainer(module, nestedContract, secondaryModule) {
    const documentation = nestedContract.documentation;
    if (!documentation) {
        return null;
    }

    const documentationModule = documentation.document === 'secondary'
        ? secondaryModule
        : module;
    if (!documentationModule) {
        throw new Error(
            `Nested contract '${nestedContract.typeName}' requires unavailable secondary documentation.`);
    }
    const section = documentation.section
        ? requireSection(documentationModule, documentation.section)
        : documentationModule;
    if (!documentation.class) {
        return section;
    }

    const documentedClass = (section.classes ?? []).find(
        candidate => candidate.name === documentation.class);
    if (!documentedClass) {
        throw new Error(
            `Official ${contract.moduleSpecifier} documentation is missing nested class '${documentation.class}'.`);
    }

    return documentedClass;
}

function generateNestedContract(module, nestedContract, secondaryModule) {
    if (!nestedContract.typeName || !nestedContract.interfaceName || !nestedContract.source) {
        throw new Error(
            'Every nested contract must include typeName, interfaceName, and source.');
    }

    const documentedContainer = resolveNestedDocumentationContainer(
        module,
        nestedContract,
        secondaryModule);
    if (documentedContainer && nestedContract.methodCount !== undefined) {
        assertCount(
            documentedContainer.methods?.length ?? 0,
            nestedContract.methodCount,
            `nested ${nestedContract.typeName} method count`);
    }
    if (documentedContainer && nestedContract.propertyCount !== undefined) {
        assertCount(
            documentedContainer.properties?.length ?? 0,
            nestedContract.propertyCount,
            `nested ${nestedContract.typeName} property count`);
    }
    if (documentedContainer && nestedContract.classMethodCount !== undefined) {
        assertCount(
            documentedContainer.classMethods?.length ?? 0,
            nestedContract.classMethodCount,
            `nested ${nestedContract.typeName} static method count`);
    }
    if (documentedContainer && nestedContract.constructorSignatureCount !== undefined) {
        assertCount(
            documentedContainer.signatures?.length ?? 0,
            nestedContract.constructorSignatureCount,
            `nested ${nestedContract.typeName} constructor signature count`);
    }
    const documentationPrefix = nestedContract.documentationPrefix
        ?? `${nestedContract.typeName.toLowerCase()}.`;
    const documentedMethods = documentedContainer && !nestedContract.documentationOnly
        ? generateMethodOverloads(
            documentedContainer.methods ?? [],
            nestedContract,
            documentationPrefix)
        : [];
    const configuredMethods = generateConfiguredNestedMethods(nestedContract);
    const documentedProperties = documentedContainer && !nestedContract.documentationOnly
        ? generateProperties(documentedContainer.properties ?? [], nestedContract)
        : [];
    const configuredProperties = generateConfiguredNestedProperties(nestedContract);
    const properties = [...documentedProperties, ...configuredProperties];

    return [
        '// <auto-generated />',
        `// Generated from the official Node.js ${lock.nodeVersion} ${nestedContract.documentation?.document === 'secondary' ? contract.secondaryDocumentationModule : documentationModule} API documentation.`,
        `// Source: ${nestedContract.documentation?.document === 'secondary' ? lock.secondarySourceUrl : lock.sourceUrl}`,
        `// SHA-256: ${nestedContract.documentation?.document === 'secondary' ? lock.secondarySha256 : lock.sha256}`,
        '',
        '#nullable enable',
        '',
        'namespace Jroc.Runtime.Node.Contracts;',
        '',
        '/// <summary>',
        `/// Defines the documented <c>${contract.displayName}.${nestedContract.typeName}</c> contract.`,
        '/// </summary>',
        `/// <remarks>Source: <c>${xmlEscape(nestedContract.source)}</c>.</remarks>`,
        `[global::System.CodeDom.Compiler.GeneratedCode("generateNodeModuleInterface.js", "sha256:${generatorSha256}")]`,
        `[NodeModuleType("${contract.moduleSpecifier}", "${nestedContract.typeName}")]`,
        `public interface ${nestedContract.interfaceName} : IJavaScriptValueHost`,
        '{',
        ...properties.flatMap(property => [property, '']),
        ...documentedMethods.flatMap(method => [method, '']),
        ...configuredMethods.flatMap(method => [method, '']),
        '}',
        ''
    ].join('\n');
}

function generatedNestedContractOutputPath(nestedContract) {
    return path.join(
        repoRoot,
        'src',
        'JavaScriptRuntime',
        'Node',
        'Contracts',
        `${nestedContract.interfaceName}.Generated.cs`);
}

function generateNestedHostAdapters(nestedContracts) {
    const hostedContracts = nestedContracts.filter(nestedContract => nestedContract.host);
    if (hostedContracts.length === 0) {
        return null;
    }

    const className = `${contract.outputStem}NestedContractAdapters`;
    const members = hostedContracts.flatMap(nestedContract => {
        const hostName = `${nestedContract.interfaceName.slice(1)}Host`;
        const properties = nestedContract.properties ?? [];
        const hostedProperties = properties.flatMap(property => {
            const propertyType = mapType(property.type);
            const value = `global::JavaScriptRuntime.ObjectRuntime.GetProperty(_value!, "${property.name}")`;
            const convertedValue = propertyType === 'bool'
                ? `global::JavaScriptRuntime.TypeUtilities.ToBoolean(${value})`
                : propertyType === 'double'
                    ? `global::JavaScriptRuntime.TypeUtilities.ToNumber(${value})`
                    : propertyType === 'string'
                        ? `global::JavaScriptRuntime.DotNet2JSConversions.ToString(${value})`
                        : `(${propertyType})${value}!`;
            return [
                `        public ${propertyType} ${csharpMemberName(property.name)}`,
                `            => ${convertedValue};`,
                ''
            ];
        });
        return [
            `    public static ${nestedContract.interfaceName} As${nestedContract.interfaceName.slice(1)}(object? value)`,
            `        => value as ${nestedContract.interfaceName} ?? new ${hostName}(value);`,
            '',
            `    private sealed class ${hostName} : ${nestedContract.interfaceName}`,
            '    {',
            '        private readonly object? _value;',
            '',
            `        public ${hostName}(object? value)`,
            '        {',
            '            _value = value;',
            '        }',
            '',
            '        object? IJavaScriptValueHost.JavaScriptValue => _value;',
            '',
            ...hostedProperties,
            '    }',
            ''
        ];
    });

    return [
        '// <auto-generated />',
        `// Generated from the official Node.js ${lock.nodeVersion} ${documentationModule} API documentation.`,
        `// Source: ${lock.sourceUrl}`,
        `// SHA-256: ${lock.sha256}`,
        '',
        '#nullable enable',
        '',
        'namespace Jroc.Runtime.Node.Contracts;',
        '',
        `[global::System.CodeDom.Compiler.GeneratedCode("generateNodeModuleInterface.js", "sha256:${generatorSha256}")]`,
        `public static class ${className}`,
        '{',
        ...members,
        '}',
        ''
    ].join('\n');
}

function generateInterface(documentation) {
    const rootCategory = contract.rootCategory ?? 'modules';
    const module = documentation[rootCategory]?.find(candidate => candidate.name === lock.module);
    if (!module) {
        throw new Error(
            `Official documentation does not contain ${rootCategory === 'globals' ? 'global' : 'module'} '${lock.module}'.`);
    }

    let methodGroups;
    let standardProperties;

    if (contract.kind === 'normalized-api') {
        const methodSections = (contract.methodSections ?? [])
            .map(sectionName => requireSection(module, sectionName));
        const methodContainers = methodSections.length > 0 ? methodSections : [module];
        const methodClasses = (contract.methodClasses ?? []).map(className => {
            const documentedClass = module.classes?.find(candidate => candidate.name === className);
            if (!documentedClass) {
                throw new Error(
                    `Official ${contract.moduleSpecifier} documentation is missing class '${className}'.`);
            }
            return documentedClass;
        });
        const selectedMethods = [
            ...methodContainers.flatMap(container => container.methods ?? []),
            ...methodClasses.flatMap(documentedClass => documentedClass.methods ?? [])
        ].filter(method => !contract.selectedMethodPrefix
            || method.textRaw.startsWith(contract.selectedMethodPrefix));

        assertCount(module.methods?.length ?? 0, lock.rootMethodCount, 'root method count');
        assertCount(module.properties?.length ?? 0, lock.rootPropertyCount, 'root property count');
        assertCount(module.classes?.length ?? 0, lock.rootClassCount, 'root class count');
        assertCount(module.modules?.length ?? 0, lock.rootSectionCount, 'root section count');
        assertCount(
            selectedMethods.length,
            lock.selectedMethodRecordCount,
            'selected method record count');
        assertCount(
            normalizedMethodOverloads().length,
            lock.normalizedMethodOverloadCount,
            'normalized method overload count');
        assertCount(
            overrides.properties.length,
            lock.exportPropertyOverrideCount,
            'export property override count');

        const expectedSectionCounts = lock.methodSectionMethodCounts ?? {};
        for (const [index, sectionName] of (contract.methodSections ?? []).entries()) {
            assertCount(
                methodSections[index].methods?.length ?? 0,
                expectedSectionCounts[sectionName],
                `'${sectionName}' method count`);
        }

        methodGroups = [{
            heading: `${contract.displayName} methods`,
            methods: [
                ...generateMethodOverloads(removeNormalizedMethods(selectedMethods)),
                ...generateNormalizedMethods()
            ]
        }];
        standardProperties = [];
    } else if (contract.kind === 'documented-api') {
        const api = contract.methodSection
            ? requireSection(module, contract.methodSection)
            : module;
        const selectedMethods = api.methods ?? [];

        assertCount(module.methods?.length ?? 0, lock.rootMethodCount, 'root method count');
        assertCount(module.properties?.length ?? 0, lock.rootPropertyCount, 'root property count');
        assertCount(module.classes?.length ?? 0, lock.rootClassCount, 'root class count');
        assertCount(module.modules?.length ?? 0, lock.rootSectionCount, 'root section count');
        assertCount(
            selectedMethods.length,
            lock.selectedMethodRecordCount,
            'selected method record count');
        assertCount(
            api.properties?.length ?? 0,
            lock.selectedPropertyCount,
            'selected property count');
        assertCount(
            new Set(selectedMethods.map(method => extractSignature(method).memberName)).size,
            lock.uniqueMethodCount,
            'unique method count');
        assertCount(
            normalizedMethodOverloads().length,
            lock.normalizedMethodOverloadCount,
            'normalized method overload count');
        assertCount(
            Object.keys(overrides.methodSignatureOverrides ?? {}).length,
            lock.methodSignatureOverrideCount,
            'method signature override count');
        assertCount(
            selectedMethods.filter(method => {
                const signature = extractSignature(method);
                return signature.parameters.trim().length > 0
                    && (method.signatures?.[0]?.params?.length ?? 0) === 0;
            }).length,
            lock.malformedMethodRecordCount,
            'malformed method record count');
        assertCount(
            Object.keys(overrides.methodReturnTypes ?? {}).length,
            lock.methodReturnTypeOverrideCount,
            'method return type override count');
        assertCount(
            overrides.properties.length,
            lock.exportPropertyOverrideCount,
            'export property override count');

        methodGroups = [{
            heading: `${contract.displayName} methods`,
            methods: [
                ...generateMethodOverloads(
                    applyMethodSignatureOverrides(
                        removeNormalizedMethods(selectedMethods))),
                ...generateNormalizedMethods()
            ]
        }];
        standardProperties = [];
    } else if (contract.kind === 'process') {
        const excludedProperties = new Set(overrides.excludedProperties);
        const standardProcessProperties = (module.properties ?? [])
            .filter(property => {
                const match = property.textRaw.match(/^`([^`]+)`/);
                return !match || !excludedProperties.has(match[1]);
            });
        const topLevelProcessMethods = (module.methods ?? [])
            .filter(method => !extractSignature(method).memberName.includes('.'));
        const processEvents = requireSection(module, 'process_events');

        assertCount(module.methods?.length ?? 0, lock.methodCount, 'method count');
        assertCount(
            topLevelProcessMethods.length,
            lock.topLevelMethodCount,
            'top-level method count');
        assertCount(
            (module.methods?.length ?? 0) - topLevelProcessMethods.length,
            lock.excludedNestedMethodCount,
            'excluded nested method count');
        assertCount(module.properties?.length ?? 0, lock.rawPropertyCount, 'raw property count');
        assertCount(
            standardProcessProperties.length,
            lock.standardPropertyCount,
            'normalized standard property count');
        assertCount(excludedProperties.size, lock.excludedPropertyCount, 'excluded property count');
        assertCount(
            overrides.properties.length,
            lock.overridePropertyCount,
            'override property count');
        assertCount(processEvents.events?.length ?? 0, lock.eventCount, 'event count');

        methodGroups = [{
            heading: 'Process methods',
            methods: generateMethodsWithOptionalAndRestParameters(topLevelProcessMethods)
        }];
        standardProperties = generateProperties(standardProcessProperties);
    } else if (contract.kind === 'stream') {
        const consumerApi = requireMiscSection(module, 'API for stream consumers');
        const duplexAndTransformStreams = requireMiscSection(
            consumerApi,
            'duplex_and_transform_streams');
        const topLevelMethods = (module.methods ?? [])
            .filter(method => {
                const parsed = extractSignature(method);
                return parsed.signature.startsWith('stream.')
                    && !parsed.memberName.includes('.');
            });
        const duplexPairMethods = (duplexAndTransformStreams.methods ?? [])
            .filter(method => extractSignature(method).memberName === 'duplexPair');

        assertCount(module.methods?.length ?? 0, lock.rawMethodCount, 'raw method count');
        assertCount(
            topLevelMethods.length,
            lock.topLevelMethodRecordCount,
            'top-level method record count');
        assertCount(
            (module.methods?.length ?? 0) - topLevelMethods.length,
            lock.excludedNestedMethodCount,
            'excluded nested method count');
        assertCount(
            consumerApi.methods?.length ?? 0,
            lock.consumerMethodCount,
            'consumer API method count');
        assertCount(
            duplexPairMethods.length,
            lock.duplexPairMethodCount,
            'duplexPair method count');
        assertCount(
            overrides.properties.length,
            lock.exportPropertyCount,
            'export property override count');

        methodGroups = [{
            heading: 'Stream methods',
            methods: [
                ...generateMethodOverloads(removeNormalizedMethods([
                    ...topLevelMethods,
                    ...duplexPairMethods
                ])),
                ...generateNormalizedMethods()
            ]
        }];
        standardProperties = [];
    } else if (contract.kind === 'stream-promises') {
        const typesOfStreams = requireSection(module, 'types_of_streams');

        assertCount(
            typesOfStreams.methods?.length ?? 0,
            lock.promiseMethodCount,
            'promise API method count');

        methodGroups = [{
            heading: 'Stream promise methods',
            methods: [
                ...generateMethodOverloads(
                    removeNormalizedMethods(typesOfStreams.methods ?? [])),
                ...generateNormalizedMethods()
            ]
        }];
        standardProperties = [];
    } else if (contract.kind === 'util') {
        const deprecatedApi = requireSection(module, 'deprecated_apis');

        assertCount(module.methods?.length ?? 0, lock.methodCount, 'method count');
        assertCount(module.properties?.length ?? 0, lock.propertyCount, 'property count');
        assertCount(module.classes?.length ?? 0, lock.classCount, 'class count');
        assertCount(module.modules?.length ?? 0, lock.sectionCount, 'section count');
        assertCount(
            deprecatedApi.methods?.length ?? 0,
            lock.deprecatedMethodCount,
            'deprecated method count');
        assertCount(
            overrides.properties.length,
            lock.exportPropertyCount,
            'export property override count');

        methodGroups = [
            {
                heading: 'Utility methods',
                methods: [
                    ...generateMethodOverloads(
                        removeNormalizedMethods(module.methods ?? [])),
                    ...generateNormalizedMethods()
                ]
            },
            {
                heading: 'Deprecated utility methods',
                methods: generateMethodOverloads(deprecatedApi.methods ?? [])
            }
        ];
        standardProperties = [];
    } else if (contract.kind === 'util-types') {
        const typesProperty = (module.properties ?? [])
            .find(property => property.name === 'types');
        if (!typesProperty) {
            throw new Error(
                "Official util documentation is missing the 'util.types' property.");
        }

        assertCount(module.properties?.length ?? 0, lock.propertyCount, 'property count');
        assertCount(
            typesProperty.methods?.length ?? 0,
            lock.typeMethodCount,
            'util.types method count');

        methodGroups = [{
            heading: 'Type predicates',
            methods: generateMethodOverloads(
                applyMethodSignatureOverrides(typesProperty.methods ?? []))
        }];
        standardProperties = [];
    } else if (contract.kind === 'zlib') {
        const convenienceMethods = requireMiscSection(module, 'Convenience methods');
        const legacyConstants = collectDocumentedLegacyZlibConstants(module);

        assertCount(module.methods?.length ?? 0, lock.methodCount, 'method count');
        assertCount(module.properties?.length ?? 0, lock.propertyCount, 'property count');
        assertCount(module.classes?.length ?? 0, lock.classCount, 'class count');
        assertCount(module.miscs?.length ?? 0, lock.sectionCount, 'section count');
        assertCount(
            convenienceMethods.methods?.length ?? 0,
            lock.convenienceMethodCount,
            'convenience method count');
        assertCount(
            legacyConstants.length,
            lock.deprecatedTopLevelConstantCount,
            'deprecated top-level constant count');
        assertCount(
            overrides.properties.length,
            lock.exportPropertyOverrideCount,
            'export property override count');

        methodGroups = [{
            heading: 'Compression methods',
            methods: generateMethodOverloads(module.methods)
        }];
        standardProperties = legacyConstants.map(constantName => [
            '    /// <summary>',
            `    /// Gets the deprecated top-level <c>zlib.${constantName}</c> constant.`,
            '    /// </summary>',
            '    /// <remarks>',
            '    /// Use the corresponding member on <c>zlib.constants</c> instead.',
            '    /// </remarks>',
            '    [global::System.Obsolete("Access through zlib.constants instead.")]',
            `    [NodeModuleMember("${constantName}")]`,
            `    double ${constantName} { get; }`
        ].join('\n'));
    } else if (contract.kind === 'string-decoder') {
        const stringDecoderClass = module.classes?.find(
            candidate => candidate.name === 'StringDecoder');
        if (!stringDecoderClass) {
            throw new Error(
                "Official string_decoder documentation is missing the 'StringDecoder' class.");
        }

        assertCount(module.classes?.length ?? 0, lock.classCount, 'class count');
        assertCount(
            stringDecoderClass.methods?.length ?? 0,
            lock.classMethodCount,
            'StringDecoder method count');
        assertCount(
            stringDecoderClass.properties?.length ?? 0,
            lock.classPropertyCount,
            'StringDecoder property count');
        assertCount(
            overrides.properties.length,
            lock.exportPropertyOverrideCount,
            'export property override count');

        methodGroups = [{
            heading: 'String decoder module methods',
            methods: []
        }];
        standardProperties = [];
    } else if (contract.kind === 'timers') {
        const schedulingTimers = requireSection(module, 'scheduling_timers');
        const cancellingTimers = requireSection(module, 'cancelling_timers');

        assertCount(module.classes?.length ?? 0, lock.classCount, 'class count');
        assertCount(module.modules?.length ?? 0, lock.sectionCount, 'section count');
        assertCount(
            schedulingTimers.methods?.length ?? 0,
            lock.schedulingMethodCount,
            'scheduling method count');
        assertCount(
            cancellingTimers.methods?.length ?? 0,
            lock.cancellingMethodCount,
            'cancelling method count');

        methodGroups = [
            {
                heading: 'Scheduling timers',
                methods: generateMethodsWithOptionalAndRestParameters(
                    schedulingTimers.methods ?? [])
            },
            {
                heading: 'Cancelling timers',
                methods: generateMethodsWithOptionalAndRestParameters(
                    cancellingTimers.methods ?? [])
            }
        ];
        standardProperties = [];
    } else if (contract.kind === 'timers-promises') {
        const promisesApi = requireSection(module, 'timers_promises_api');
        const topLevelMethods = (promisesApi.methods ?? [])
            .filter(method => !extractSignature(method).memberName.includes('.'));

        assertCount(
            promisesApi.methods?.length ?? 0,
            lock.promiseMethodCount,
            'promise API method count');
        assertCount(
            topLevelMethods.length,
            lock.promiseTopLevelMethodCount,
            'promise top-level method count');
        assertCount(
            (promisesApi.methods?.length ?? 0) - topLevelMethods.length,
            lock.promiseNestedMethodCount,
            'promise nested method count');
        assertCount(
            overrides.properties.length,
            lock.promiseExportPropertyCount,
            'promise export property count');

        methodGroups = [{
            heading: 'Promise timer methods',
            methods: [
                ...generateMethodOverloads(removeNormalizedMethods(topLevelMethods)),
                ...generateNormalizedMethods()
            ]
        }];
        standardProperties = [];
    } else if (contract.kind === 'buffer') {
        const moduleApis = requireSection(module, '`node:buffer`_module_apis');
        const bufferConstants = requireSection(moduleApis, 'buffer_constants');

        assertCount(module.classes?.length ?? 0, lock.rootClassCount, 'root class count');
        assertCount(module.modules?.length ?? 0, lock.rootSectionCount, 'root section count');
        assertCount(moduleApis.methods?.length ?? 0, lock.methodCount, 'module API method count');
        assertCount(moduleApis.properties?.length ?? 0, lock.propertyCount, 'module API property count');
        assertCount(moduleApis.classes?.length ?? 0, lock.moduleClassCount, 'module API class count');
        assertCount(moduleApis.modules?.length ?? 0, lock.moduleSectionCount, 'module API section count');
        assertCount(
            bufferConstants.properties?.length ?? 0,
            lock.constantPropertyCount,
            'buffer constants property count');
        assertCount(
            overrides.properties.length,
            lock.exportPropertyOverrideCount,
            'export property override count');

        methodGroups = [{
            heading: 'Buffer module methods',
            methods: generateMethodsWithOptionalAndRestParameters(moduleApis.methods)
        }];
        standardProperties = generateProperties(moduleApis.properties);
    } else if (contract.kind === 'events') {
        assertCount(module.methods?.length ?? 0, lock.methodCount, 'method count');
        assertCount(module.properties?.length ?? 0, lock.rawPropertyCount, 'raw property count');
        assertCount(module.classes?.length ?? 0, lock.classCount, 'class count');
        assertCount(module.modules?.length ?? 0, lock.sectionCount, 'section count');
        assertCount(
            overrides.properties.length,
            lock.exportPropertyCount,
            'export property override count');

        methodGroups = [{
            heading: 'Event methods',
            methods: generateMethodsWithOptionalAndRestParameters(module.methods)
        }];
        standardProperties = [];
    } else if (contract.kind === 'perf-hooks') {
        assertCount(module.methods?.length ?? 0, lock.methodCount, 'method count');
        assertCount(module.properties?.length ?? 0, lock.propertyCount, 'property count');
        assertCount(module.classes?.length ?? 0, lock.classCount, 'class count');
        assertCount(
            overrides.properties.length,
            lock.exportPropertyCount,
            'export property override count');

        methodGroups = [{
            heading: 'Performance hooks methods',
            methods: generateMethodOverloads(module.methods)
        }];
        standardProperties = [];
    } else if (contract.kind === 'child-process') {
        const asynchronousApi = requireSection(module, 'asynchronous_process_creation');
        const synchronousApi = requireSection(module, 'synchronous_process_creation');

        assertCount(module.classes?.length ?? 0, lock.classCount, 'class count');
        assertCount(
            asynchronousApi.methods?.length ?? 0,
            lock.asynchronousMethodCount,
            'asynchronous method count');
        assertCount(
            synchronousApi.methods?.length ?? 0,
            lock.synchronousMethodCount,
            'synchronous method count');

        methodGroups = [
            {
                heading: 'Asynchronous process creation',
                methods: generateMethodOverloads(asynchronousApi.methods)
            },
            {
                heading: 'Synchronous process creation',
                methods: generateMethodOverloads(synchronousApi.methods)
            }
        ];
        standardProperties = [[
            '    /// <summary>',
            '    /// Gets the exported ChildProcess constructor.',
            '    /// </summary>',
            '    [NodeModuleMember("ChildProcess")]',
            '    object? ChildProcess { get; }'
        ].join('\n')];
    } else if (contract.kind === 'top-level-optional-rest') {
        assertCount(module.methods?.length ?? 0, lock.methodCount, 'method count');
        assertCount(module.properties?.length ?? 0, lock.propertyCount, 'property count');

        methodGroups = [{
            heading: contract.methodGroupHeading,
            methods: generateMethodsWithOptionalAndRestParameters(module.methods)
        }];
        standardProperties = generateProperties(module.properties);
    } else if (contract.kind === 'top-level-overloads') {
        assertCount(module.methods?.length ?? 0, lock.methodCount, 'method count');
        assertCount(module.properties?.length ?? 0, lock.propertyCount, 'property count');
        assertCount(module.modules?.length ?? 0, lock.sectionCount, 'section count');

        methodGroups = [{
            heading: contract.methodGroupHeading,
            methods: generateMethodOverloads(module.methods)
        }];
        standardProperties = generateProperties(module.properties);
    } else if (contract.kind === 'console') {
        const consoleClass = module.classes?.find(candidate => candidate.name === 'Console');
        if (!consoleClass) {
            throw new Error("Official console documentation is missing the 'Console' class.");
        }
        const inspectorApi = requireSection(module, 'inspector_only_methods');

        assertCount(module.classes?.length ?? 0, lock.classCount, 'class count');
        assertCount(consoleClass.methods?.length ?? 0, lock.classMethodCount, 'class method count');
        assertCount(
            inspectorApi.methods?.length ?? 0,
            lock.inspectorMethodCount,
            'inspector method count');

        methodGroups = [
            {
                heading: 'Console methods',
                methods: generateMethodsWithOptionalAndRestParameters(consoleClass.methods)
            },
            {
                heading: 'Inspector-only methods',
                methods: generateMethodsWithOptionalAndRestParameters(inspectorApi.methods)
            }
        ];
        standardProperties = [[
            '    /// <summary>',
            '    /// Gets the exported Console constructor.',
            '    /// </summary>',
            '    [NodeModuleMember("Console")]',
            '    object? Console { get; }'
        ].join('\n')];
    } else if (contract.kind === 'fs-promises') {
        const promisesApi = requireSection(module, 'promises_api');
        assertCount(promisesApi.methods?.length ?? 0, lock.promiseMethodCount, 'promise method count');
        assertCount(promisesApi.properties?.length ?? 0, lock.promisePropertyCount, 'promise property count');
        assertCount(promisesApi.classes?.length ?? 0, lock.promiseClassCount, 'promise class count');

        methodGroups = [{
            heading: 'Promise API',
            methods: generateMethodOverloads(promisesApi.methods)
        }];
        standardProperties = [[
            '    /// <summary>',
            '    /// Gets the file system constants object.',
            '    /// </summary>',
            '    [NodeModuleMember("constants")]',
            '    object? constants { get; }'
        ].join('\n')];
    } else {
        const callbackApi = requireSection(module, 'callback_api');
        const synchronousApi = requireSection(module, 'synchronous_api');
        const commonObjects = requireSection(module, 'common_objects');

        assertCount(callbackApi.methods?.length ?? 0, lock.callbackMethodCount, 'callback method count');
        assertCount(synchronousApi.methods?.length ?? 0, lock.synchronousMethodCount, 'synchronous method count');
        assertCount(commonObjects.classes?.length ?? 0, lock.commonObjectClassCount, 'common object class count');

        methodGroups = [
            {
                heading: 'Callback API',
                methods: generateMethodOverloads(callbackApi.methods)
            },
            {
                heading: 'Synchronous API',
                methods: generateMethodOverloads(synchronousApi.methods)
            }
        ];
        standardProperties = [
            [
                '    /// <summary>',
                '    /// Gets the file system constants object.',
                '    /// </summary>',
                '    [NodeModuleMember("constants")]',
                '    object? constants { get; }'
            ].join('\n'),
            [
                '    /// <summary>',
                '    /// Gets the promise-based file system API.',
                '    /// </summary>',
                '    [NodeModuleMember("promises")]',
                '    object? promises { get; }'
            ].join('\n')
        ];
    }

    const overrideProperties = overrides.properties.map(property => {
        const access = property.access ?? 'read-only';
        if (!['read-only', 'read-write'].includes(access)) {
            throw new Error(
                `Unsupported access '${access}' for ${contract.moduleSpecifier} override property '${property.name}'.`);
        }

        if (!property.source) {
            throw new Error(
                `${contract.moduleSpecifier} override property '${property.name}' requires a source.`);
        }

        const summary = property.summary
            ?? (property.deprecated
                ? `Gets the deprecated <c>${contract.documentationPrefix}${xmlEscape(property.name)}</c> constant.`
                : `Gets the exported <c>${contract.documentationPrefix}${xmlEscape(property.name)}</c> member.`);

        return [
            '    /// <summary>',
            `    /// ${summary}`,
            '    /// </summary>',
            `    /// <remarks>Source: <c>${xmlEscape(property.source)}</c>.</remarks>`,
            ...(property.deprecated
                ? [`    [global::System.Obsolete("${property.deprecated}")]`]
                : []),
            `    [NodeModuleMember("${property.name}")]`,
            `    ${mapType(property.type)} ${csharpMemberName(property.name)} { get;${access === 'read-write' ? ' set;' : ''} }`
        ].join('\n');
    });

    return [
        '// <auto-generated />',
        `// Generated from the official Node.js ${lock.nodeVersion} ${documentationModule} API documentation.`,
        `// Source: ${lock.sourceUrl}`,
        `// SHA-256: ${lock.sha256}`,
        '',
        '#nullable enable',
        '',
        'namespace Jroc.Runtime.Node.Contracts;',
        '',
        '/// <summary>',
        `/// Defines the public top-level <c>${contract.displayName}</c> module contract from Node.js ${lock.nodeVersion}.`,
        '/// </summary>',
        `[global::System.CodeDom.Compiler.GeneratedCode("generateNodeModuleInterface.js", "sha256:${generatorSha256}")]`,
        `[NodeModuleInterface("${contract.moduleSpecifier}")]`,
        `public interface ${contract.interfaceName}`,
        '{',
        ...standardProperties.flatMap(property => [property, '']),
        ...overrideProperties.flatMap(property => [property, '']),
        ...methodGroups.flatMap(group => [
            `    // ${group.heading}`,
            ...group.methods.flatMap(method => [method, ''])
        ]),
        '}',
        ''
    ].join('\n');
}

function parseContractMembers(interfaceSource) {
    const members = [];
    let nodeMemberName = null;

    for (const line of interfaceSource.split('\n')) {
        const attributeMatch = line.match(/^    \[NodeModuleMember\("([^"]+)"\)\]$/);
        if (attributeMatch) {
            nodeMemberName = attributeMatch[1];
            continue;
        }

        if (!nodeMemberName) {
            continue;
        }

        const propertyMatch = line.match(
            /^    (.+) (@?[A-Za-z_][A-Za-z0-9_]*) \{ get;( set;)? \}$/);
        if (propertyMatch) {
            members.push({
                kind: 'property',
                nodeMemberName,
                returnType: propertyMatch[1],
                csharpName: propertyMatch[2],
                hasSetter: Boolean(propertyMatch[3]),
                parameters: []
            });
            nodeMemberName = null;
            continue;
        }

        const methodMatch = line.match(/^    (.+) (@?[A-Za-z_][A-Za-z0-9_]*)\((.*)\);$/);
        if (methodMatch) {
            const parameters = methodMatch[3]
                ? methodMatch[3].split(', ').map(parameter => {
                    const declaration = parameter
                        .replace(/\[global::Jroc\.Runtime\.Node\.Contracts\.NodeModuleParameterContract\(typeof\([^)]+\)\)\]\s*/g, '')
                        .replace(/ = null$/, '');
                    const parameterMatch = declaration.match(
                        /^(params )?(.+?) (@?[A-Za-z_][A-Za-z0-9_]*)$/);
                    if (!parameterMatch) {
                        throw new Error(`Cannot parse generated C# parameter '${parameter}'.`);
                    }

                    return {
                        declaration: parameter,
                        implementationDeclaration:
                            `${parameterMatch[1] ?? ''}${parameterMatch[2]} ${parameterMatch[3]}`,
                        name: parameterMatch[3]
                    };
                })
                : [];

            members.push({
                kind: 'method',
                nodeMemberName,
                returnType: methodMatch[1],
                csharpName: methodMatch[2],
                parameters
            });
            nodeMemberName = null;
        }
    }

    return members;
}

function renderImplementedMethodBody(member, implementation) {
    const configuredArgumentCount = implementation.argumentCount ?? member.parameters.length;
    const argumentCount = Math.min(configuredArgumentCount, member.parameters.length);
    const argumentsList = member.parameters
        .slice(0, argumentCount)
        .map((parameter, index) => implementation.unwrapArguments?.includes(index)
            ? `global::Jroc.Runtime.Node.Contracts.NodeModuleContractHosting.Unwrap(${parameter.name}!)!`
            : `${parameter.name}!`);

    while (argumentsList.length < (implementation.minimumArgumentCount ?? 0)) {
        argumentsList.push('null!');
    }

    const target = implementation.target ?? member.csharpName;
    let invocation;
    if (implementation.style === 'argument-array') {
        invocation = `${target}(new object[] { ${argumentsList.join(', ')} })`;
    } else if (implementation.style === 'direct') {
        invocation = `${target}(${argumentsList.join(', ')})`;
    } else {
        throw new Error(
            `Unsupported intrinsic invocation style '${implementation.style}' for '${member.nodeMemberName}'.`);
    }

    if (member.returnType === 'void') {
        return implementation.returnsVoid ? invocation : `_ = ${invocation}`;
    }

    if (member.returnType === 'object?') {
        return invocation;
    }

    if (implementation.returnsDirectly) {
        return invocation;
    }

    if (member.returnType === 'bool') {
        return `global::JavaScriptRuntime.TypeUtilities.ToBoolean(${invocation})`;
    }

    if (member.returnType === 'double') {
        return `global::JavaScriptRuntime.TypeUtilities.ToNumber(${invocation})`;
    }

    if (member.returnType === 'string') {
        return `global::JavaScriptRuntime.DotNet2JSConversions.ToString(${invocation})`;
    }

    return `(${member.returnType})${invocation}!`;
}

function generateIntrinsicImplementation(interfaceSource) {
    const intrinsicImplementations = new Map(
        Object.entries(overrides.intrinsicImplementations));
    const members = parseContractMembers(interfaceSource);
    const generatedMembers = members.map(member => {
        const implementation = intrinsicImplementations.get(member.nodeMemberName);
        const isImplemented = implementation
            && (!implementation.parameterCounts
                || implementation.parameterCounts.includes(member.parameters.length));
        const declaration = member.kind === 'property'
            ? `${member.returnType} ${contractAlias}.${member.csharpName}`
            : `${member.returnType} ${contractAlias}.${member.csharpName}(${member.parameters
                .map(parameter => parameter.implementationDeclaration)
                .join(', ')})`;

        if (!isImplemented) {
            if (member.kind === 'property' && member.hasSetter) {
                return [
                    `    ${declaration}`,
                    '    {',
                    `        get => throw CreateNotImplementedException("${member.nodeMemberName}");`,
                    `        set => throw CreateNotImplementedException("${member.nodeMemberName}");`,
                    '    }'
                ].join('\n');
            }

            return [
                `    ${declaration}`,
                `        => throw CreateNotImplementedException("${member.nodeMemberName}");`
            ].join('\n');
        }

        if (member.kind === 'property') {
            if (implementation.style !== 'direct') {
                throw new Error(
                    `Intrinsic property '${member.nodeMemberName}' must use direct invocation.`);
            }

            const target = implementation.target ?? member.csharpName;
            const getValue = member.returnType === 'object?'
                ? target
                : `(${member.returnType})${target}!`;
            if (member.hasSetter) {
                if (implementation.getterOnly) {
                    return [
                        `    ${declaration}`,
                        '    {',
                        `        get => ${getValue};`,
                        `        set => throw CreateNotImplementedException("${member.nodeMemberName}");`,
                        '    }'
                    ].join('\n');
                }

                return [
                    `    ${declaration}`,
                    '    {',
                    `        get => ${getValue};`,
                    `        set => ${target} = value;`,
                    '    }'
                ].join('\n');
            }

            if (implementation.getterOnly) {
                throw new Error(
                    `Intrinsic property '${member.nodeMemberName}' cannot be getter-only because its contract is read-only.`);
            }

            return [
                `    ${declaration}`,
                `        => ${getValue};`
            ].join('\n');
        }

        return [
            `    ${declaration}`,
            `        => ${renderImplementedMethodBody(member, implementation)};`
        ].join('\n');
    });

    return [
        '// <auto-generated />',
        `// Generated from the official Node.js ${lock.nodeVersion} ${documentationModule} API documentation.`,
        `// Source: ${lock.sourceUrl}`,
        `// SHA-256: ${lock.sha256}`,
        '',
        '#nullable enable',
        '#pragma warning disable CS0618',
        '',
        `using ${contractAlias} = Jroc.Runtime.Node.Contracts.${contract.interfaceName};`,
        '',
        'namespace JavaScriptRuntime.Node;',
        '',
        `public sealed partial class ${contract.intrinsicClassName} : ${contractAlias}`,
        '{',
        ...generatedMembers.flatMap(member => [member, '']),
        '    private static global::System.NotImplementedException CreateNotImplementedException(string memberName)',
        `        => new($"The intrinsic ${contract.displayName} module does not implement '${contract.documentationPrefix}{memberName}'.");`,
        '}',
        ''
    ].join('\n');
}

function generateNestedIntrinsicImplementation(interfaceSource, nestedContract) {
    if (!nestedContract.intrinsicType) {
        return null;
    }

    const intrinsicImplementations = new Map(
        Object.entries(nestedContract.intrinsicImplementations ?? {}));
    const members = parseContractMembers(interfaceSource);
    const generatedMembers = members.map(member => {
        const implementation = intrinsicImplementations.get(member.nodeMemberName);
        const isImplemented = implementation
            && (!implementation.parameterCounts
                || implementation.parameterCounts.includes(member.parameters.length));
        const declaration = member.kind === 'property'
            ? `${member.returnType} NestedContract.${member.csharpName}`
            : `${member.returnType} NestedContract.${member.csharpName}(${member.parameters
                .map(parameter => parameter.implementationDeclaration)
                .join(', ')})`;

        if (!isImplemented) {
            return [
                `        ${declaration}`,
                `            => throw CreateNotImplementedException("${member.nodeMemberName}");`
            ].join('\n');
        }

        if (member.kind === 'property') {
            if (implementation.style !== 'direct') {
                throw new Error(
                    `Nested intrinsic property '${member.nodeMemberName}' must use direct invocation.`);
            }

            const target = implementation.target ?? member.csharpName;
            const getValue = member.returnType === 'object?'
                ? target
                : `(${member.returnType})${target}!`;
            return [
                `        ${declaration}`,
                `            => ${getValue};`
            ].join('\n');
        }

        return [
            `        ${declaration}`,
            `            => ${renderImplementedMethodBody(member, implementation)};`
        ].join('\n');
    });

    const typeParts = nestedContract.intrinsicType.split('.');
    const typeEnvelope = typeParts.flatMap((typePart, index) => [
        index === typeParts.length - 1
            ? `${'    '.repeat(index)}public sealed partial class ${typePart} : NestedContract`
            : `${'    '.repeat(index)}public sealed partial class ${typePart}`,
        `${'    '.repeat(index)}{`
    ]);
    const closingBraces = typeParts
        .map((_, index) => `${'    '.repeat(index)}}`)
        .reverse();
    const memberIndent = '    '.repeat(typeParts.length);

    return [
        '// <auto-generated />',
        `// Generated from the official Node.js ${lock.nodeVersion} ${documentationModule} API documentation.`,
        `// Source: ${lock.sourceUrl}`,
        `// SHA-256: ${lock.sha256}`,
        '',
        '#nullable enable',
        '#pragma warning disable CS0618',
        '',
        `using NestedContract = Jroc.Runtime.Node.Contracts.${nestedContract.interfaceName};`,
        '',
        'namespace JavaScriptRuntime.Node;',
        '',
        ...typeEnvelope,
        ...generatedMembers.map(member => member.replaceAll('\n        ', `\n${memberIndent}`)
            .replace(/^        /, memberIndent)),
        '',
        `${memberIndent}private static global::System.NotImplementedException CreateNotImplementedException(string memberName)`,
        `${memberIndent}    => new($"The intrinsic ${contract.displayName}.${nestedContract.typeName} contract does not implement '${nestedContract.documentationPrefix ?? `${nestedContract.typeName.toLowerCase()}.`}{memberName}'.");`,
        ...closingBraces,
        ''
    ].join('\n');
}

async function main() {
    const source = await loadDocumentation();
    const hash = crypto.createHash('sha256').update(source).digest('hex');
    if (hash !== lock.sha256) {
        throw new Error(
            `Official Node.js documentation hash mismatch. Expected ${lock.sha256}, received ${hash}.`);
    }

    const documentation = JSON.parse(source.toString('utf8'));
    const secondarySource = await loadSecondaryDocumentation();
    let secondaryModule = null;
    if (secondarySource) {
        const secondaryHash = crypto.createHash('sha256').update(secondarySource).digest('hex');
        if (secondaryHash !== lock.secondarySha256) {
            throw new Error(
                `Official Node.js secondary documentation hash mismatch. Expected ${lock.secondarySha256}, received ${secondaryHash}.`);
        }
        const secondaryDocumentation = JSON.parse(secondarySource.toString('utf8'));
        secondaryModule = secondaryDocumentation.modules?.find(
            candidate => candidate.name === lock.secondaryModule);
        if (!secondaryModule) {
            throw new Error(
                `Official secondary documentation does not contain module '${lock.secondaryModule}'.`);
        }
    }
    const generatedInterface = generateInterface(documentation);
    const generatedIntrinsicImplementation = generateIntrinsicImplementation(generatedInterface);
    const outputs = new Map([
        [interfaceOutputPath, generatedInterface],
        [intrinsicImplementationOutputPath, generatedIntrinsicImplementation]
    ]);
    const rootCategory = contract.rootCategory ?? 'modules';
    const module = documentation[rootCategory]?.find(candidate => candidate.name === lock.module);
    if (!module) {
        throw new Error(
            `Official documentation does not contain ${rootCategory === 'globals' ? 'global' : 'module'} '${lock.module}'.`);
    }

    const nestedContracts = overrides.nestedContracts ?? [];
    for (const nestedContract of nestedContracts) {
        const nestedInterface = generateNestedContract(module, nestedContract, secondaryModule);
        outputs.set(generatedNestedContractOutputPath(nestedContract), nestedInterface);

        const nestedIntrinsicImplementation = generateNestedIntrinsicImplementation(
            nestedInterface,
            nestedContract);
        if (nestedIntrinsicImplementation) {
            outputs.set(
                path.join(
                    repoRoot,
                    'src',
                    'JavaScriptRuntime',
                    'Node',
                    `${nestedContract.intrinsicType}.${nestedContract.interfaceName}.Generated.cs`),
                nestedIntrinsicImplementation);
        }
    }

    const nestedHostAdapters = generateNestedHostAdapters(nestedContracts);
    if (nestedHostAdapters) {
        outputs.set(
            path.join(
                repoRoot,
                'src',
                'JavaScriptRuntime',
                'Node',
                'Contracts',
                `${contract.outputStem}NestedContractAdapters.Generated.cs`),
            nestedHostAdapters);
    }

    if (checkOnly) {
        const staleOutputs = [...outputs]
            .filter(([outputPath, generated]) =>
                !fs.existsSync(outputPath) || fs.readFileSync(outputPath, 'utf8') !== generated)
            .map(([outputPath]) => path.relative(repoRoot, outputPath));

        if (staleOutputs.length > 0) {
            const modeArgument = contract.flag ? ` ${contract.flag}` : '';
            const generationCommand =
                `node scripts/nodeContracts/generateNodeModuleInterface.js${modeArgument}`;
            throw new Error(
                `${staleOutputs.join(', ')} ${staleOutputs.length === 1 ? 'is' : 'are'} stale. Run ` +
                `\`${generationCommand}\`.`);
        }

        for (const outputPath of outputs.keys()) {
            console.log(`${path.relative(repoRoot, outputPath)} is current.`);
        }
        return;
    }

    for (const [outputPath, generated] of outputs) {
        fs.mkdirSync(path.dirname(outputPath), { recursive: true });
        fs.writeFileSync(outputPath, generated);
        console.log(`Generated ${path.relative(repoRoot, outputPath)}.`);
    }
}

main().catch(error => {
    console.error(error.message);
    process.exitCode = 1;
});
