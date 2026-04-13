'use strict';

const { parseTest262Metadata } = require('./test262/metadataParser');

function quoteString(value) {
  return '"' + value
    .replace(/\\/g, '\\\\')
    .replace(/\n/g, '\\n')
    .replace(/\r/g, '\\r')
    .replace(/"/g, '\\"') + '"';
}

function formatScalar(value) {
  if (value === null) {
    return 'null';
  }

  if (value === undefined) {
    return 'undefined';
  }

  if (typeof value === 'string') {
    return quoteString(value);
  }

  return String(value);
}

function formatNegative(negative) {
  if (!negative) {
    return 'null';
  }

  return '{ phase: ' + formatScalar(negative.phase)
    + ', type: ' + formatScalar(negative.type)
    + ' }';
}

function formatIssue(issue) {
  if (!issue) {
    return 'null';
  }

  return '{ code: ' + formatScalar(issue.code)
    + ', source: ' + formatScalar(issue.source)
    + ', reason: ' + formatScalar(issue.reason)
    + ' }';
}

function writeParsedResult(name, parsed) {
  console.log('name: ' + formatScalar(name));
  console.log('filePath: ' + formatScalar(parsed.filePath));
  console.log('fileName: ' + formatScalar(parsed.fileName));
  console.log('hasFrontmatter: ' + formatScalar(parsed.hasFrontmatter));
  console.log('unknownKeys.length: ' + formatScalar(parsed.unknownKeys.length));
  console.log('unknownKeys[0]: ' + formatScalar(parsed.unknownKeys[0]));
  console.log('description: ' + formatScalar(parsed.description));
  console.log('info: ' + formatScalar(parsed.info));
  console.log('esid: ' + formatScalar(parsed.esid));
  console.log('es5id: ' + formatScalar(parsed.es5id));
  console.log('includes.length: ' + formatScalar(parsed.includes.length));
  console.log('includes[0]: ' + formatScalar(parsed.includes[0]));
  console.log('includes[1]: ' + formatScalar(parsed.includes[1]));
  console.log('flags.length: ' + formatScalar(parsed.flags.length));
  console.log('flags[0]: ' + formatScalar(parsed.flags[0]));
  console.log('flags[1]: ' + formatScalar(parsed.flags[1]));
  console.log('flags[2]: ' + formatScalar(parsed.flags[2]));
  console.log('features.length: ' + formatScalar(parsed.features.length));
  console.log('features[0]: ' + formatScalar(parsed.features[0]));
  console.log('features[1]: ' + formatScalar(parsed.features[1]));
  console.log('locale.length: ' + formatScalar(parsed.locale.length));
  console.log('locale[0]: ' + formatScalar(parsed.locale[0]));
  console.log('defines.length: ' + formatScalar(parsed.defines.length));
  console.log('defines[0]: ' + formatScalar(parsed.defines[0]));
  console.log('negative: ' + formatNegative(parsed.negative));
  console.log('execution.sourceType: ' + formatScalar(parsed.execution.sourceType));
  console.log('execution.strictMode: ' + formatScalar(parsed.execution.strictMode));
  console.log('execution.defaultHarnessIncludes.length: ' + formatScalar(parsed.execution.defaultHarnessIncludes.length));
  console.log('execution.defaultHarnessIncludes[0]: ' + formatScalar(parsed.execution.defaultHarnessIncludes[0]));
  console.log('execution.defaultHarnessIncludes[1]: ' + formatScalar(parsed.execution.defaultHarnessIncludes[1]));
  console.log('execution.harnessIncludes.length: ' + formatScalar(parsed.execution.harnessIncludes.length));
  console.log('execution.harnessIncludes[0]: ' + formatScalar(parsed.execution.harnessIncludes[0]));
  console.log('execution.harnessIncludes[1]: ' + formatScalar(parsed.execution.harnessIncludes[1]));
  console.log('execution.harnessIncludes[2]: ' + formatScalar(parsed.execution.harnessIncludes[2]));
  console.log('execution.harnessIncludes[3]: ' + formatScalar(parsed.execution.harnessIncludes[3]));
  console.log('execution.requiresDefaultHarness: ' + formatScalar(parsed.execution.requiresDefaultHarness));
  console.log('execution.requiresAsyncHarness: ' + formatScalar(parsed.execution.requiresAsyncHarness));
  console.log('execution.requiresAgent: ' + formatScalar(parsed.execution.requiresAgent));
  console.log('execution.canBlock: ' + formatScalar(parsed.execution.canBlock));
  console.log('execution.isModule: ' + formatScalar(parsed.execution.isModule));
  console.log('execution.isRaw: ' + formatScalar(parsed.execution.isRaw));
  console.log('execution.isAsync: ' + formatScalar(parsed.execution.isAsync));
  console.log('execution.isGenerated: ' + formatScalar(parsed.execution.isGenerated));
  console.log('execution.isNonDeterministic: ' + formatScalar(parsed.execution.isNonDeterministic));
  console.log('execution.isFixture: ' + formatScalar(parsed.execution.isFixture));
  console.log('mvpBlockers.length: ' + formatScalar(parsed.mvpBlockers.length));
  console.log('mvpBlockers[0]: ' + formatIssue(parsed.mvpBlockers[0]));
  console.log('mvpBlockers[1]: ' + formatIssue(parsed.mvpBlockers[1]));
  console.log('mvpBlockers[2]: ' + formatIssue(parsed.mvpBlockers[2]));
  console.log('mvpBlockers[3]: ' + formatIssue(parsed.mvpBlockers[3]));
  console.log('unsupported.length: ' + formatScalar(parsed.unsupported.length));
  console.log('unsupported[0]: ' + formatIssue(parsed.unsupported[0]));
  console.log('unsupported[1]: ' + formatIssue(parsed.unsupported[1]));
}

const basicStrict = parseTest262Metadata(`/*---
description: basic strict fixture
esid: sec-basic-example
includes: [propertyHelper.js, compareArray.js]
flags: [onlyStrict]
features: [let, class]
---*/
assert(true);
`, { filePath: 'test/language/example/basic-strict.js' });

const negativeParse = parseTest262Metadata(`/*---
info: |
  When the parser sees an unterminated string literal
  it should surface a parse-phase SyntaxError.
es5id: 8.4_A14_T1
description: parse negative sample
negative:
  phase: parse
  type: SyntaxError
---*/
$DONOTEVALUATE();
var str = ";
`, { filePath: 'test/language/types/string/example-negative.js' });

const asyncGenerated = parseTest262Metadata(`/*---
description: async completion sample
flags: [async, generated]
includes: [propertyHelper.js]
features: [Promise]
---*/
Promise.resolve().then(function () {
  print('Test262:AsyncTestComplete');
});
`, { filePath: 'test/built-ins/promise/example-async.js' });

const moduleResolution = parseTest262Metadata(`/*---
description: module resolution sample
flags: [module]
features: [import-attributes]
negative:
  phase: resolution
  type: SyntaxError
---*/
export {} from './dep.js';
`, { filePath: 'test/language/module-code/example-module.js' });

const fixtureUnsupported = parseTest262Metadata(`/*---
description: fixture helper
flags: [raw, CanBlockIsFalse, mysteryFlag]
includes: [agent.js]
locale: [en-US]
defines: [helperBinding]
unknownMeta: surprise
---*/
print('helper');
`, { filePath: 'test/language/module-code/example_FIXTURE.js' });

writeParsedResult('basic-strict', basicStrict);
console.log('---');
writeParsedResult('negative-parse', negativeParse);
console.log('---');
writeParsedResult('async-generated', asyncGenerated);
console.log('---');
writeParsedResult('module-resolution', moduleResolution);
console.log('---');
writeParsedResult('fixture-unsupported', fixtureUnsupported);
