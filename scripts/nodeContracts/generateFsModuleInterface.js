#!/usr/bin/env node

const crypto = require('crypto');
const fs = require('fs');
const path = require('path');

const repoRoot = path.resolve(__dirname, '..', '..');
const args = process.argv.slice(2);
const promisesMode = args.includes('--promises');
const contract = promisesMode
    ? {
        moduleSpecifier: 'fs/promises',
        documentationPrefix: 'fsPromises.',
        interfaceName: 'IFsPromisesModule',
        intrinsicClassName: 'FSPromises',
        displayName: 'node:fs/promises',
        outputStem: 'FsPromises',
        overrideStem: 'fsPromises'
    }
    : {
        moduleSpecifier: 'fs',
        documentationPrefix: 'fs.',
        interfaceName: 'IFsModule',
        intrinsicClassName: 'FS',
        displayName: 'node:fs',
        outputStem: 'Fs',
        overrideStem: 'fs'
    };
const lockPath = path.join(__dirname, 'fs.node24.lock.json');
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
        throw new Error(`Official fs documentation is missing the '${sectionName}' section.`);
    }

    return section;
}

function assertCount(actual, expected, description) {
    if (actual !== expected) {
        throw new Error(
            `Official fs documentation ${description} changed: expected ${expected}, found ${actual}. ` +
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

    if (normalized === 'function') {
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
    return parts[0] + parts.slice(1)
        .map(part => part.length === 0 ? '' : part[0].toUpperCase() + part.slice(1))
        .join('');
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
        const returnType = mapType(signature.return?.type, true);
        const optionalNames = optionalParameterNames(parsed.parameters);

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

function generateInterface(documentation) {
    const module = documentation.modules?.find(candidate => candidate.name === lock.module);
    if (!module) {
        throw new Error(`Official documentation does not contain module '${lock.module}'.`);
    }

    let methodGroups;
    let standardProperties;

    if (promisesMode) {
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
        if (property.access !== 'read-only') {
            throw new Error(
                `Unsupported access '${property.access}' for ${contract.moduleSpecifier} override property '${property.name}'.`);
        }

        return [
            '    /// <summary>',
            `    /// Gets the deprecated <c>${contract.documentationPrefix}${xmlEscape(property.name)}</c> constant.`,
            '    /// </summary>',
            `    /// <remarks>Source: <c>${xmlEscape(property.source)}</c>.</remarks>`,
            `    [global::System.Obsolete("${property.deprecated}")]`,
            `    [NodeModuleMember("${property.name}")]`,
            `    ${mapType(property.type)} ${csharpMemberName(property.name)} { get; }`
        ].join('\n');
    });

    return [
        '// <auto-generated />',
        `// Generated from the official Node.js ${lock.nodeVersion} fs API documentation.`,
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
        `[global::System.CodeDom.Compiler.GeneratedCode("generateFsModuleInterface.js", "sha256:${generatorSha256}")]`,
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

        const propertyMatch = line.match(/^    (.+) ([A-Za-z_][A-Za-z0-9_]*) \{ get; \}$/);
        if (propertyMatch) {
            members.push({
                kind: 'property',
                nodeMemberName,
                returnType: propertyMatch[1],
                csharpName: propertyMatch[2],
                parameters: []
            });
            nodeMemberName = null;
            continue;
        }

        const methodMatch = line.match(/^    (.+) ([A-Za-z_][A-Za-z0-9_]*)\((.*)\);$/);
        if (methodMatch) {
            const parameters = methodMatch[3]
                ? methodMatch[3].split(', ').map(parameter => {
                    const separator = parameter.lastIndexOf(' ');
                    return {
                        declaration: parameter,
                        name: parameter.slice(separator + 1)
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

    let invocation;
    if (implementation.style === 'argument-array') {
        invocation = `${member.csharpName}(new object[] { ${argumentsList.join(', ')} })`;
    } else if (implementation.style === 'direct') {
        invocation = `${member.csharpName}(${argumentsList.join(', ')})`;
    } else {
        throw new Error(
            `Unsupported intrinsic invocation style '${implementation.style}' for '${member.nodeMemberName}'.`);
    }

    if (member.returnType === 'void') {
        return `_ = ${invocation}`;
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
        const declaration = member.kind === 'property'
            ? `${member.returnType} FsContract.${member.csharpName}`
            : `${member.returnType} FsContract.${member.csharpName}(${member.parameters
                .map(parameter => parameter.declaration)
                .join(', ')})`;

        if (!implementation) {
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

            return [
                `    ${declaration}`,
                `        => ${member.csharpName};`
            ].join('\n');
        }

        return [
            `    ${declaration}`,
            `        => ${renderImplementedMethodBody(member, implementation)};`
        ].join('\n');
    });

    return [
        '// <auto-generated />',
        `// Generated from the official Node.js ${lock.nodeVersion} fs API documentation.`,
        `// Source: ${lock.sourceUrl}`,
        `// SHA-256: ${lock.sha256}`,
        '',
        '#nullable enable',
        '#pragma warning disable CS0618',
        '',
        `using FsContract = Jroc.Runtime.Node.Contracts.${contract.interfaceName};`,
        '',
        'namespace JavaScriptRuntime.Node;',
        '',
        `public sealed partial class ${contract.intrinsicClassName} : FsContract`,
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
            const generationCommand = promisesMode
                ? 'node scripts/nodeContracts/generateFsModuleInterface.js --promises'
                : 'node scripts/nodeContracts/generateFsModuleInterface.js';
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
