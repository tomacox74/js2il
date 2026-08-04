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

function requireSection(module, sectionName) {
    const section = module.modules?.find(candidate => candidate.name === sectionName);
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

function extractSignature(method) {
    const signature = method.textRaw.replace(/^`|`$/g, '');
    const openParen = signature.indexOf('(');
    const closeParen = signature.lastIndexOf(')');
    if (openParen < 0 || closeParen < openParen) {
        throw new Error(`Cannot parse official Node.js signature '${method.textRaw}'.`);
    }

    return {
        signature,
        memberName: signature.slice(0, openParen).replace(contract.documentationPrefix, ''),
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

function getOptionalParameterNames(memberName, parameters, descriptors) {
    const names = optionalParameterNames(parameters);
    const override = overrides.methodOptionalParameters?.[memberName];
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

function mapType(type, isReturnType = false) {
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

function generateMethodOverloads(methods) {
    const generated = [];
    const signatures = new Set();

    for (const method of methods) {
        const parsed = extractSignature(method);
        const signature = method.signatures?.[0];
        if (!signature) {
            throw new Error(`Official Node.js method '${parsed.signature}' has no structured signature.`);
        }

        const descriptors = new Map(
            (signature.params ?? []).map(parameter => [parameter.name, parameter]));
        const returnType = mapMethodReturnType(parsed.memberName, signature);
        const optionalNames = getOptionalParameterNames(
            parsed.memberName,
            parsed.parameters,
            signature.params ?? []);

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
                    type: optionalNames.has(name) ? 'object?' : mapType(descriptor.type)
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
                `    [NodeModuleMember("${parsed.memberName}")]`,
                `    ${returnType} ${methodName}(${parameters.map(parameter => `${parameter.type} ${parameter.name}`).join(', ')});`
            ].join('\n'));
        }
    }

    return generated;
}

function generateMethodsWithOptionalAndRestParameters(methods) {
    const generated = [];
    const signatures = new Set();

    for (const method of methods) {
        const parsed = extractSignature(method);
        const signature = method.signatures?.[0];
        if (!signature) {
            throw new Error(`Official Node.js method '${parsed.signature}' has no structured signature.`);
        }

        const descriptors = signature.params ?? [];
        const optionalNames = getOptionalParameterNames(
            parsed.memberName,
            parsed.parameters,
            descriptors);
        const restParameter = descriptors.find(parameter => parameter.name.startsWith('...'));
        const positionalParameters = descriptors.filter(
            parameter => !parameter.name.startsWith('...'));
        const requiredCount = positionalParameters.filter(
            parameter => !optionalNames.has(parameter.name)).length;
        const returnType = mapMethodReturnType(parsed.memberName, signature);
        const methodName = csharpMemberName(parsed.memberName);

        for (let parameterCount = requiredCount;
            parameterCount <= positionalParameters.length;
            parameterCount++) {
            const parameters = positionalParameters
                .slice(0, parameterCount)
                .map(parameter => {
                    const type = optionalNames.has(parameter.name)
                        ? 'object?'
                        : mapType(parameter.type);
                    return `${type} ${csharpIdentifier(parameter.name)}`;
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
                `    [NodeModuleMember("${parsed.memberName}")]`,
                `    ${returnType} ${methodName}(${parameters.join(', ')});`
            ].join('\n'));
        }
    }

    return generated;
}

function generateNormalizedMethods() {
    const methods = overrides.normalizedMethods ?? [];
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

function mapMethodReturnType(memberName, signature) {
    const override = overrides.methodReturnTypes?.[memberName];
    if (override && (!override.type || !override.source)) {
        throw new Error(
            `Return type override for '${contract.documentationPrefix}${memberName}' ` +
            'must include type and source.');
    }

    return mapType(override?.type ?? signature.return?.type, true);
}

function generateProperties(properties) {
    return properties.map(property => {
        const match = property.textRaw.match(/^`([^`]+)`(?: Type:)? \{([^}]+)\}/);
        if (!match) {
            throw new Error(
                `Cannot parse official Node.js property signature '${property.textRaw}'.`);
        }

        const [, propertyName, propertyType] = match;
        const access = overrides.propertyAccess?.[propertyName] ?? 'read-only';
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

function generateInterface(documentation) {
    const rootCategory = contract.rootCategory ?? 'modules';
    const module = documentation[rootCategory]?.find(candidate => candidate.name === lock.module);
    if (!module) {
        throw new Error(
            `Official documentation does not contain ${rootCategory === 'globals' ? 'global' : 'module'} '${lock.module}'.`);
    }

    let methodGroups;
    let standardProperties;

    if (contract.kind === 'process') {
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
        '/// <remarks>',
        '/// Nested option, result, and handle contracts intentionally remain dynamic in this proof of concept.',
        '/// They will be strongly typed by the work tracked in GitHub issue #1660.',
        '/// </remarks>',
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
                    const declaration = parameter.endsWith(' = null')
                        ? parameter.slice(0, -' = null'.length)
                        : parameter;
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
        .map(parameter => `${parameter.name}!`);

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
            if (member.hasSetter) {
                if (implementation.getterOnly) {
                    return [
                        `    ${declaration}`,
                        '    {',
                        `        get => ${target};`,
                        `        set => throw CreateNotImplementedException("${member.nodeMemberName}");`,
                        '    }'
                    ].join('\n');
                }

                return [
                    `    ${declaration}`,
                    '    {',
                    `        get => ${target};`,
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
                `        => ${target};`
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

async function main() {
    const source = await loadDocumentation();
    const hash = crypto.createHash('sha256').update(source).digest('hex');
    if (hash !== lock.sha256) {
        throw new Error(
            `Official Node.js documentation hash mismatch. Expected ${lock.sha256}, received ${hash}.`);
    }

    const generatedInterface = generateInterface(JSON.parse(source.toString('utf8')));
    const generatedIntrinsicImplementation = generateIntrinsicImplementation(generatedInterface);
    const outputs = new Map([
        [interfaceOutputPath, generatedInterface],
        [intrinsicImplementationOutputPath, generatedIntrinsicImplementation]
    ]);

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
