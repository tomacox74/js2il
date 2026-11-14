/*---
description: Native C# test262 harness helpers are available as host globals
includes: [propertyHelper.js, testTypedArray.js, testAtomics.js, tcoHelper.js, decimalToHexString.js, nans.js, promiseHelper.js, compareIterator.js, regExpUtils.js, detachArrayBuffer.js, proxyTrapsHelper.js, nativeFunctionMatcher.js, wellKnownIntrinsicObjects.js, byteConversionValues.js, deepEqual.js, resizableArrayBufferUtils.js, temporalHelpers.js, dateConstants.js, iteratorZipUtils.js, fnGlobalObject.js]
---*/

assert(true, 'assert should be callable');
assert.sameValue(1, 1, 'sameValue should use SameValue semantics');
assert.notSameValue(1, 2, 'notSameValue should use SameValue semantics');
assert.strictEqual('abc', 'abc', 'strictEqual should alias sameValue');
assert.notStrictEqual('abc', 'def', 'notStrictEqual should alias notSameValue');
assert.compareArray([1, 2, 3], [1, 2, 3], 'compareArray should compare indexed elements');
assert.deepEqual([[1, 2], [3, undefined]], [[1, 2], [3, undefined]], 'deepEqual should compare nested arrays');
assert.deepEqual({nested: {value: 1}}, {nested: {value: 1}});
assert.deepEqual({zero: -0, nan: NaN}, {zero: 0, nan: NaN});
assert.sameValue(assert.deepEqual._compare({value: 1}, {value: 2}), false);
assert.sameValue(assert.deepEqual._compare([1], {0: 1, length: 1}), false);
const deepEqualCycle1 = {};
const deepEqualCycle2 = {};
deepEqualCycle1.self = deepEqualCycle1;
deepEqualCycle2.self = deepEqualCycle2;
assert.deepEqual(deepEqualCycle1, deepEqualCycle2);
assert.deepEqual(new Set([1, 2]), new Set([1, 2]));
assert.sameValue(assert.deepEqual._compare(new Set([1, 2]), new Set([2, 1])), false);
assert.deepEqual(new Number(1), 1);
assert.sameValue(assert.deepEqual._compare.name, 'deepEqual');
assert.sameValue(assert.deepEqual._compare.length, 2);
const deepEqualOriginalSymbol = globalThis.Symbol;
const deepEqualOriginalBigInt = globalThis.BigInt;
globalThis.Symbol = undefined;
globalThis.BigInt = undefined;
assert.sameValue(assert.deepEqual._compare({}, 1), false);
globalThis.Symbol = deepEqualOriginalSymbol;
globalThis.BigInt = deepEqualOriginalBigInt;
assert.sameValue(fnGlobalObject.name, 'fnGlobalObject');
assert.sameValue(fnGlobalObject.length, 0);
assert.sameValue(fnGlobalObject(), globalThis);
assert.sameValue(fnGlobalObject(), fnGlobalObject());

$262.evalScript('var nativeScriptVar = 1; function nativeScriptFunction() { return nativeScriptVar; }');
assert.sameValue(nativeScriptFunction(), 1);
$262.evalScript('nativeScriptVar = 2; let nativeScriptLet = 3; const nativeScriptConst = 4;');
assert.sameValue(nativeScriptFunction(), 2);
assert.sameValue(nativeScriptLet, 3);
assert.sameValue(nativeScriptConst, 4);
assert.sameValue(globalThis.hasOwnProperty('nativeScriptLet'), false);
assert.throws(TypeError, function() { nativeScriptConst = 5; });
assert.throws(SyntaxError, function() { $262.evalScript('return;'); });
assert.sameValue(typeof require, 'undefined');
assert.sameValue(typeof module, 'undefined');
assert.sameValue(typeof exports, 'undefined');
$262.evalScript('let {nativeScriptProperty} = {nativeScriptProperty: 6};');
assert.sameValue(nativeScriptProperty, 6);
assert.sameValue(globalThis.hasOwnProperty('__obj'), false);
Object.defineProperty(globalThis, 'nativeScriptExistingVar', {value: 1, writable: true, configurable: true});
$262.evalScript('var nativeScriptExistingVar;');
assert.throws(SyntaxError, function() { $262.evalScript('let nativeScriptExistingVar;'); });
$262.evalScript('delete nativeScriptExistingVar; let nativeScriptAfterDeletion = 1;');
$262.evalScript('let nativeScriptExistingVar = 7;');
assert.sameValue(nativeScriptExistingVar, 7);
$262.evalScript('var nativeScriptWith = 1; with ({nativeScriptWith: 2}) { var nativeScriptWith = 3; }');
assert.sameValue(nativeScriptWith, 1);
Object.defineProperty(globalThis, 'nativeScriptHoisted', {get: function() { throw new Test262Error('getter must be replaced'); }, configurable: true});
$262.evalScript('function nativeScriptHoisted() { return 8; } var nativeScriptHoisted;');
assert.sameValue(nativeScriptHoisted(), 8);
verifyProperty(globalThis, 'nativeScriptHoisted', {writable: true, enumerable: true, configurable: false});
class NativeScriptKnownClass {
    constructor() { this.value = 1; }
    static read() { return 1; }
}
$262.evalScript('NativeScriptKnownClass = class { constructor() { this.value = 2; } static read() { return 2; } };');
assert.sameValue(new NativeScriptKnownClass().value, 2);
assert.sameValue(NativeScriptKnownClass.read(), 2);
var nativeOriginalNumber = Number;
$262.evalScript('let Number = function(value) { return 19; };');
assert.sameValue(Number(1), 19);
Number = nativeOriginalNumber;

var expectedMatch = ['a'];
expectedMatch.index = 0;
expectedMatch.input = 'a';
var iteratorDone = false;
assert.compareIterator({
    next: function() {
        if (iteratorDone) {
            return { value: undefined, done: true };
        }
        iteratorDone = true;
        return { value: expectedMatch, done: false };
    }
}, [matchValidator(['a'], 0, 'a')], 'RegExp helper validators should be available');

assert.sameValue(buildString.name, 'buildString');
assert.sameValue(buildString.length, 1);
assert.sameValue(testPropertyEscapes.name, 'testPropertyEscapes');
assert.sameValue(testPropertyEscapes.length, 3);
assert.sameValue(testPropertyOfStrings.name, 'testPropertyOfStrings');
assert.sameValue(testPropertyOfStrings.length, 1);
assert.sameValue(testExtendedCharacterClass, testPropertyOfStrings);
testPropertyOfStrings({
    regExp: /^(?:ab|\u{1F600})+$/u,
    expression: 'native string-property helper',
    matchStrings: ['ab', '\u{1F600}'],
    nonMatchStrings: ['a', 'b']
});
var nativeStringPropertyRejected = false;
try {
    testExtendedCharacterClass({
        regExp: {test: function() { return false; }},
        expression: 'must preserve assertions',
        matchStrings: ['a']
    });
} catch (error) {
    nativeStringPropertyRejected = error.message.includes('should match a');
}
assert.sameValue(nativeStringPropertyRejected, true);
var nativeStringPropertyClosed = false;
var nativeStringPropertyFailure;
try {
    testPropertyOfStrings({
        regExp: {test: function() { return false; }},
        expression: 'close the fallback iterator',
        matchStrings: {
            join: function() { return 'a'; },
            [Symbol.iterator]: function() {
                return {
                    next: function() { return {value: 'a', done: false}; },
                    return: function() {
                        nativeStringPropertyClosed = true;
                        return {done: true};
                    }
                };
            }
        }
    });
} catch (error) {
    nativeStringPropertyFailure = error;
}
assert.sameValue(nativeStringPropertyClosed, true);
assert.sameValue(nativeStringPropertyFailure.message.includes('U+000061'), true);
assert.sameValue(buildString({
    loneCodePoints: [0x61, 0x1F600, 0xD800],
    ranges: [[0x30, 0x32], [0x10FFFF, 0x10FFFF]]
}), 'a\u{1F600}\uD800012\u{10FFFF}');
assert.sameValue(buildString({loneCodePoints: null, ranges: [[2, 1]]}), '');
assert.throws(RangeError, function() {
    buildString({loneCodePoints: [0x110000], ranges: []});
});
var regExpBuildOrder = [];
var regExpLonePoints = {length: 2};
Object.defineProperty(regExpLonePoints, '0', {get: function() {
    regExpBuildOrder.push('get0');
    return {valueOf: function() { regExpBuildOrder.push('value0'); return 65; }};
}});
Object.defineProperty(regExpLonePoints, '1', {get: function() {
    regExpBuildOrder.push('get1');
    return {valueOf: function() { regExpBuildOrder.push('value1'); return 66; }};
}});
assert.sameValue(buildString({
    get loneCodePoints() { regExpBuildOrder.push('loneCodePoints'); return regExpLonePoints; },
    get ranges() { regExpBuildOrder.push('ranges'); return []; }
}), 'AB');
assert.compareArray(regExpBuildOrder, ['loneCodePoints', 'ranges', 'get0', 'get1', 'value0', 'value1']);
assert.sameValue(buildString({
    loneCodePoints: [],
    ranges: [[{valueOf: function() { return 0; }}, 10000]]
}).length, 10001);
var regExpHelperCalls = [];
var regExpHelperProbe = {
    test: function(input) {
        assert.sameValue(this, regExpHelperProbe);
        regExpHelperCalls.push(input);
        return input.length === 1;
    }
};
testPropertyEscapes(regExpHelperProbe, 'abc', {
    toString: function() { regExpHelperCalls.push('message'); return 'probe'; }
});
assert.compareArray(regExpHelperCalls, ['abc', 'a', 'message', 'b', 'message', 'c', 'message']);
var regExpHelperSymbols = [];
testPropertyEscapes({
    test: function(input) {
        if (input === 'a\u{1F600}\uD800') return false;
        regExpHelperSymbols.push(input);
        return true;
    }
}, 'a\u{1F600}\uD800', 'code points');
assert.compareArray(regExpHelperSymbols, ['a', '\u{1F600}', '\uD800']);
var regExpRawOrder = [];
assert.sameValue(buildString({
    loneCodePoints: [],
    ranges: {0: [65, 65], length: 1n}
}), 'A');
var regExpRawSymbol = {
    codePointAt: function(index) {
        assert.sameValue(this, regExpRawSymbol);
        assert.sameValue(index, 0);
        regExpRawOrder.push('codePointAt');
        return {toString: function(radix) {
            assert.sameValue(radix, 16);
            regExpRawOrder.push('hex');
            return '41';
        }};
    },
    toString: function() { regExpRawOrder.push('symbol'); return 'A'; }
};
var regExpRawSymbols = [regExpRawSymbol];
testPropertyEscapes({test: function(input) {
    if (input === regExpRawSymbols) return false;
    assert.sameValue(input, regExpRawSymbol);
    regExpRawOrder.push('test');
    return true;
}}, regExpRawSymbols, {
    toString: function() { regExpRawOrder.push('expression'); return 'raw'; }
});
assert.compareArray(regExpRawOrder, ['codePointAt', 'hex', 'test', 'expression', 'symbol']);
assert.throws(TypeError, function() {
    testPropertyEscapes({test: function() { return false; }}, [''], 'empty symbol');
});
var regExpHelperClosed = 0;
var regExpHelperIterable = {
    [Symbol.iterator]: function() {
        return {
            next: function() { return {value: '!', done: false}; },
            return: function() {
                regExpHelperClosed++;
                throw new Test262Error('Iterator close must preserve the original assertion');
            }
        };
    }
};
var regExpHelperFailure;
try {
    testPropertyEscapes({test: function() { return false; }}, regExpHelperIterable, 'failure');
} catch (error) {
    regExpHelperFailure = error;
}
assert(regExpHelperFailure.message.includes('should match U+000021'));
assert.sameValue(regExpHelperClosed, 1);

var value = {};
Object.defineProperty(value, 'answer', {
    value: 42,
    writable: false,
    enumerable: true,
    configurable: false
});

verifyProperty(value, 'answer', {
    value: 42,
    writable: false,
    enumerable: true,
    configurable: false
});
verifyNotWritable(value, 'answer');
verifyEnumerable(value, 'answer');
verifyNotConfigurable(value, 'answer');
verifyEqualTo({ answer: 42 }, 'answer', 42);

assert.sameValue(decimalToHexString(255), '00FF');
assert.sameValue(decimalToPercentHexString(255), '%FF');
assert.sameValue($MAX_ITERATIONS, 100000);
assert.sameValue(NaNs.length, 9);
assert(NaNs.every(Number.isNaN));
assert(checkSequence([1, 2, 3]));
assert.sameValue(byteConversionValues.values.length, 56);
assert.sameValue(byteConversionValues.values[0], 127);
assert.sameValue(byteConversionValues.values[20], undefined);
assert.sameValue(byteConversionValues.values[38], Infinity);
assert.sameValue(byteConversionValues.expected.Int8[1], -128);
assert.sameValue(byteConversionValues.expected.Uint8Clamped[17], 1);
assert.sameValue(byteConversionValues.expected.Uint16[8], 65535);
assert.sameValue(byteConversionValues.expected.Int32[10], -1);
assert.sameValue(byteConversionValues.expected.Uint32[21], 4294967295);
assert(Number.isNaN(byteConversionValues.expected.Float16[20]));
assert.sameValue(byteConversionValues.expected.Float16[41], 2048);
assert.sameValue(byteConversionValues.expected.Float16[42], 2052);
assert.sameValue(byteConversionValues.expected.Float16[51], 65504);
assert.sameValue(byteConversionValues.expected.Float16[52], Infinity);
assert(Number.isNaN(byteConversionValues.expected.Float32[20]));
assert(Number.isNaN(byteConversionValues.expected.Float64[20]));

var typedArrayRuns = 0;
testWithTypedArrayConstructors(function(TA, makeCtorArg) {
    var typed = new TA(makeCtorArg([1, 2]));
    assert.compareArray(typed, [1, 2]);
    typedArrayRuns++;
}, null, ['passthrough']);
assert.sameValue(typedArrayRuns, 9);

assert(isFloatTypedArrayConstructor(Float32Array));
assert(isFloatTypedArrayConstructor(Float64Array));
assert.sameValue(floatTypedArrayConstructorPrecision(Float32Array), 'single');
assert.sameValue(floatTypedArrayConstructorPrecision(Float64Array), 'double');
var typedArrayConversionRuns = 0;
testTypedArrayConversions(byteConversionValues, function(TA, value, expected, initial) {
    assert(TA);
    assert(initial === 0 || initial === 1);
    typedArrayConversionRuns++;
});
assert.sameValue(typedArrayConversionRuns, 504);

var rab = CreateRabForTest(Uint8Array);
assert.compareArray(ToNumbers(new Uint8Array(rab)), [0, 2, 4, 6]);
var collected = [];
assert.sameValue(CollectValuesAndResize(3, collected, rab, 1, 2), true);
assert.compareArray(collected, [3]);
assert.sameValue(rab.byteLength, 2);

var outOfBoundsIndexRuns = 0;
testWithAtomicsOutOfBoundsIndices(function(indexGenerator) {
    indexGenerator({ length: 4 });
    outOfBoundsIndexRuns++;
});
assert.sameValue(outOfBoundsIndexRuns, 7);

var nonViewRuns = 0;
testWithAtomicsNonViewValues(function() {
    nonViewRuns++;
});
assert.sameValue(nonViewRuns, 24);

assert.throws(Test262Error, function() {
    $ERROR('native error');
}, '$ERROR should throw Test262Error');

var constructed = new Test262Error('constructed error');
assert.sameValue(constructed.name, 'Test262Error');
assert.sameValue(constructed.message, 'constructed error');

var detachableBuffer = new ArrayBuffer(4);
var detachableView = new Uint8Array(detachableBuffer);
$DETACHBUFFER(detachableBuffer);
assert.sameValue(detachableBuffer.detached, true);
assert.sameValue(detachableBuffer.byteLength, 0);
assert.sameValue(detachableView.length, 0);
assert.sameValue(detachableView[0], undefined);

var detachedDataViewBuffer = new ArrayBuffer(4);
var detachedDataView = new DataView(detachedDataViewBuffer, 0, 4);
$DETACHBUFFER(detachedDataViewBuffer);
assert.throws(RangeError, function() {
    detachedDataView.getInt8(Infinity);
});

var coercionDataViewBuffer = new ArrayBuffer(4);
var coercionDataView = new DataView(coercionDataViewBuffer, 0, 4);
assert.throws(TypeError, function() {
    coercionDataView.getInt8({
        valueOf: function() {
            $DETACHBUFFER(coercionDataViewBuffer);
            return 0;
        }
    });
});

var constructorBuffer = new ArrayBuffer(4);
assert.throws(TypeError, function() {
    new Uint8Array(constructorBuffer, 0, {
        valueOf: function() {
            $DETACHBUFFER(constructorBuffer);
            return 1;
        }
    });
});

var allowedProxyTrapCalls = 0;
var allowedProxyTraps = allowProxyTraps({
    get: function() {
        allowedProxyTrapCalls++;
        return 'allowed';
    }
});
assert.sameValue(allowedProxyTraps.get(), 'allowed');
assert.sameValue(allowedProxyTrapCalls, 1);
assert.throws(Test262Error, function() {
    allowedProxyTraps.set();
});

assertNativeFunction(Function.prototype.bind);
assertToStringOrNativeFunction(Function.prototype.bind, 'function bind() { [native code] }');
assert.throws(SyntaxError, function() {
    validateNativeFunctionSource('function broken() {}');
});

assert.sameValue(WellKnownIntrinsicObjects.length, 1);
assert.sameValue(WellKnownIntrinsicObjects[0].name, '%ThrowTypeError%');
assert.sameValue(WellKnownIntrinsicObjects[0].value, undefined);

var temporalCalls = [];
var temporalTarget = {};
TemporalHelpers.observeProperty(temporalCalls, temporalTarget, Symbol.iterator, undefined, "items");
assert.sameValue(temporalTarget[Symbol.iterator], undefined);
var temporalBag = TemporalHelpers.propertyBagObserver(temporalCalls, { length: 2 }, "items");
assert.sameValue(+temporalBag.length, 2);
assert.compareArray(temporalCalls, [
    "get items[Symbol.iterator]",
    "get items.length",
    "get items.length.valueOf",
    "call items.length.valueOf"
]);

// The selected PlainTime ports require every field assertion and every option
// callback, not a presence-only TemporalHelpers shim.
[
    ["assertPlainTime", 7],
    ["assertPlainTimesEqual", 2],
    ["checkStringOptionWrongType", 4],
    ["checkRoundingIncrementOptionWrongType", 3],
    ["checkPluralUnitsAccepted", 2]
].forEach(function(entry) {
    assert.sameValue(TemporalHelpers[entry[0]].name, entry[0]);
    assert.sameValue(TemporalHelpers[entry[0]].length, entry[1]);
});
assert.sameValue(TemporalHelpers.ISO.plainTimeStringsAmbiguous.name, "plainTimeStringsAmbiguous");
assert.sameValue(TemporalHelpers.ISO.plainTimeStringsAmbiguous.length, 0);
assert.sameValue(TemporalHelpers.ISO.plainTimeStringsUnambiguous.name, "plainTimeStringsUnambiguous");
assert.sameValue(TemporalHelpers.ISO.plainTimeStringsUnambiguous.length, 0);

var nativeTemporalTime = new Temporal.PlainTime(12, 34, 56, 123, 456, 789);
var nativeTemporalFields = [12, 34, 56, 123, 456, 789];
TemporalHelpers.assertPlainTime(nativeTemporalTime, ...nativeTemporalFields);
var nativeTemporalLookalike = {
    hour: 12, minute: 34, second: 56, millisecond: 123, microsecond: 456, nanosecond: 789
};
assert.throws(assert.AssertionError, function() {
    TemporalHelpers.assertPlainTime(nativeTemporalLookalike, ...nativeTemporalFields);
});
assert.throws(assert.AssertionError, function() {
    TemporalHelpers.assertPlainTime({}, ...nativeTemporalFields);
});
for (var temporalFieldIndex = 0; temporalFieldIndex < 6; temporalFieldIndex++) {
    var wrongTemporalFields = nativeTemporalFields.slice();
    wrongTemporalFields[temporalFieldIndex]++;
    assert.throws(assert.AssertionError, function() {
        TemporalHelpers.assertPlainTime(nativeTemporalTime, ...wrongTemporalFields);
    });
}
assert.throws(assert.AssertionError, function() {
    TemporalHelpers.assertPlainTime(new Temporal.PlainTime(), -0, 0, 0, 0, 0, 0);
});
var temporalFieldReadOrder = [];
var temporalObservedTime = new Temporal.PlainTime(...nativeTemporalFields);
["hour", "minute", "second", "millisecond", "microsecond", "nanosecond"].forEach(function(field, index) {
    Object.defineProperty(temporalObservedTime, field, {
        get: function() {
            temporalFieldReadOrder.push(field);
            return nativeTemporalFields[index];
        }
    });
});
TemporalHelpers.assertPlainTime(temporalObservedTime, ...nativeTemporalFields);
assert.compareArray(temporalFieldReadOrder, [
    "hour", "minute", "second", "millisecond", "microsecond", "nanosecond"
]);
TemporalHelpers.assertPlainTimesEqual(nativeTemporalTime, new Temporal.PlainTime(...nativeTemporalFields));
assert.throws(assert.AssertionError, function() {
    TemporalHelpers.assertPlainTimesEqual(nativeTemporalTime, nativeTemporalLookalike);
});
var nativeTemporalEqualsLookalike = {
    equals: function() { return true; }
};
assert.throws(assert.AssertionError, function() {
    TemporalHelpers.assertPlainTimesEqual(nativeTemporalEqualsLookalike, nativeTemporalTime);
});
assert.throws(assert.AssertionError, function() {
    TemporalHelpers.assertPlainTimesEqual(nativeTemporalTime, new Temporal.PlainTime(12, 34, 56, 123, 456, 788));
});
assert.throws(assert.AssertionError, function() {
    TemporalHelpers.assertPlainTimesEqual({}, nativeTemporalTime);
});
assert.throws(assert.AssertionError, function() {
    TemporalHelpers.assertPlainTimesEqual(nativeTemporalTime, {});
});

var expectedAmbiguousTimes = [
    "2021-12", "2021-12[-12:00]", "1214", "0229", "1130", "12-14",
    "12-14[-14:00]", "202112", "202112[UTC]",
    "2021-12[u-ca=iso8601]", "2021-12[-12:00][u-ca=iso8601]",
    "1214[u-ca=iso8601]", "0229[u-ca=iso8601]", "1130[u-ca=iso8601]",
    "12-14[u-ca=iso8601]", "12-14[-14:00][u-ca=iso8601]",
    "202112[u-ca=iso8601]", "202112[UTC][u-ca=iso8601]"
];
var expectedUnambiguousTimes = [
    "2021-13", "202113", "2021-13[-13:00]", "202113[-13:00]",
    "0000-00", "000000", "0000-00[UTC]", "000000[UTC]",
    "1314", "13-14", "1232", "0230", "0631", "0000", "00-00"
];
assert.compareArray(TemporalHelpers.ISO.plainTimeStringsAmbiguous(), expectedAmbiguousTimes);
assert.compareArray(TemporalHelpers.ISO.plainTimeStringsUnambiguous(), expectedUnambiguousTimes);
var mutableAmbiguousTimes = TemporalHelpers.ISO.plainTimeStringsAmbiguous();
var mutableUnambiguousTimes = TemporalHelpers.ISO.plainTimeStringsUnambiguous();
mutableAmbiguousTimes[0] = "mutated";
mutableUnambiguousTimes[0] = "mutated";
assert.compareArray(TemporalHelpers.ISO.plainTimeStringsAmbiguous(), expectedAmbiguousTimes);
assert.compareArray(TemporalHelpers.ISO.plainTimeStringsUnambiguous(), expectedUnambiguousTimes);

function temporalOptionInputLabel(value) {
    if (value === null) return "null";
    if (value === true) return "true";
    if (value === false) return "false";
    if (typeof value === "symbol") return "symbol";
    if (typeof value === "number") return "number";
    if (typeof value === "bigint") return "bigint";
    return Object.prototype.hasOwnProperty.call(value, "valueOf") ? "observer" : "plain object";
}
var temporalStringOptionCalls = [];
function temporalCheckOverflow(value) {
    temporalStringOptionCalls.push(temporalOptionInputLabel(value));
    return Temporal.PlainTime.from({ hour: 24, minute: 35 }, { overflow: value });
}
TemporalHelpers.checkStringOptionWrongType("overflow", "constrain", temporalCheckOverflow, function(result, description) {
    temporalStringOptionCalls.push("assertion");
    assert.sameValue(description, "object with toString");
    TemporalHelpers.assertPlainTime(result, 23, 35, 0, 0, 0, 0);
});
assert.compareArray(temporalStringOptionCalls, [
    "null", "true", "false", "symbol", "number", "bigint", "plain object", "observer", "assertion"
]);
// Every invalid partition must actually assert the exception, and wrong exception
// classes must not be treated as expected errors.
for (var missingStringError = 0; missingStringError < 7; missingStringError++) {
    var stringErrorIndex = 0;
    assert.throws(assert.AssertionError, function() {
        TemporalHelpers.checkStringOptionWrongType("overflow", "constrain", function(value) {
            if (stringErrorIndex++ === missingStringError) return nativeTemporalTime;
            return temporalCheckOverflow(value);
        }, function() { throw new Test262Error("unexpected success callback"); });
    });
}
assert.throws(assert.AssertionError, function() {
    TemporalHelpers.checkStringOptionWrongType("overflow", "constrain", function() {
        throw new TypeError("wrong error for null");
    }, function() {});
});
assert.throws(Test262Error, function() {
    TemporalHelpers.checkStringOptionWrongType("overflow", "constrain", temporalCheckOverflow, function() {
        throw new Test262Error("assertion callback failure");
    });
});

var temporalIncrementCalls = [];
var temporalIncrementTime = new Temporal.PlainTime(13, 46, 22, 600);
function temporalCheckIncrement(value) {
    temporalIncrementCalls.push(temporalOptionInputLabel(value));
    return temporalIncrementTime.round({ smallestUnit: "second", roundingIncrement: value });
}
TemporalHelpers.checkRoundingIncrementOptionWrongType(temporalCheckIncrement, function(result, description) {
    temporalIncrementCalls.push("true assertion");
    assert.sameValue(description, "true");
    TemporalHelpers.assertPlainTime(result, 13, 46, 23, 0, 0, 0);
}, function(result, description) {
    temporalIncrementCalls.push("object assertion");
    assert.sameValue(description, "object with valueOf");
    TemporalHelpers.assertPlainTime(result, 13, 46, 22, 0, 0, 0);
});
assert.compareArray(temporalIncrementCalls, [
    "null", "true", "true assertion", "false", "symbol", "bigint", "plain object", "observer", "object assertion"
]);
// The pinned halfEven examples exercise an odd microsecond tie. Pin the even
// tie too, with all six native-helper fields and an independently exact string.
var temporalEvenTie = new Temporal.PlainTime(13, 46, 23, 123, 986, 500);
TemporalHelpers.assertPlainTime(
    temporalEvenTie.round({ smallestUnit: "microsecond", roundingMode: "halfEven" }),
    13, 46, 23, 123, 986, 0);
TemporalHelpers.assertPlainTime(
    temporalEvenTie.round({ smallestUnit: "microsecond", roundingMode: "halfExpand" }),
    13, 46, 23, 123, 987, 0);
assert.sameValue(
    temporalEvenTie.toString({ smallestUnit: "microsecond", roundingMode: "halfEven" }),
    "13:46:23.123986");
var incrementInvalidLabels = ["null", "false", "symbol", "bigint", "plain object"];
incrementInvalidLabels.forEach(function(missingLabel) {
    assert.throws(assert.AssertionError, function() {
        TemporalHelpers.checkRoundingIncrementOptionWrongType(function(value) {
            if (temporalOptionInputLabel(value) === missingLabel) return nativeTemporalTime;
            return temporalCheckIncrement(value);
        }, function() {}, function() {});
    });
});
assert.throws(Test262Error, function() {
    TemporalHelpers.checkRoundingIncrementOptionWrongType(temporalCheckIncrement, function() {
        throw new Test262Error("true assertion failure");
    }, function() {});
});
assert.throws(Test262Error, function() {
    TemporalHelpers.checkRoundingIncrementOptionWrongType(temporalCheckIncrement, function() {}, function() {
        throw new Test262Error("object assertion failure");
    });
});
// Bypassing the primitive observer must fail trace validation even when the
// result and external assertion callback otherwise succeed.
var originalTemporalPrimitiveObserver = TemporalHelpers.toPrimitiveObserver;
try {
    TemporalHelpers.toPrimitiveObserver = function(calls, value) { return value; };
    assert.throws(assert.AssertionError, function() {
        TemporalHelpers.checkStringOptionWrongType("overflow", "constrain", temporalCheckOverflow, function() {});
    });
    assert.throws(assert.AssertionError, function() {
        TemporalHelpers.checkRoundingIncrementOptionWrongType(temporalCheckIncrement, function() {}, function() {});
    });
} finally {
    TemporalHelpers.toPrimitiveObserver = originalTemporalPrimitiveObserver;
}

var temporalPluralCalls = [];
TemporalHelpers.checkPluralUnitsAccepted(function(unit) {
    temporalPluralCalls.push(unit);
    return temporalIncrementTime.round({ smallestUnit: unit });
}, ["hour", "minute", "second", "millisecond", "microsecond", "nanosecond"]);
assert.compareArray(temporalPluralCalls, [
    "hour", "hours", "minute", "minutes", "second", "seconds",
    "millisecond", "milliseconds", "microsecond", "microseconds", "nanosecond", "nanoseconds"
]);
var temporalPluralStringCalls = [];
TemporalHelpers.checkPluralUnitsAccepted(function(unit) {
    temporalPluralStringCalls.push(unit);
    return temporalIncrementTime.toString({ smallestUnit: unit });
}, ["minute", "second", "millisecond", "microsecond", "nanosecond"]);
assert.compareArray(temporalPluralStringCalls, [
    "minute", "minutes", "second", "seconds", "millisecond", "milliseconds",
    "microsecond", "microseconds", "nanosecond", "nanoseconds"
]);
assert.throws(assert.AssertionError, function() {
    TemporalHelpers.checkPluralUnitsAccepted(function(unit) {
        return new Temporal.PlainTime(unit === "hour" ? 1 : 2);
    }, ["hour"]);
});
assert.throws(assert.AssertionError, function() {
    TemporalHelpers.checkPluralUnitsAccepted(function(unit) { return unit; }, ["second"]);
});
assert.throws(TypeError, function() {
    TemporalHelpers.checkPluralUnitsAccepted(function() { return {}; }, ["second"]);
});

assert.sameValue(date_1899_end, -2208988800001);
assert.sameValue(date_1900_start, -2208988800000);
assert.sameValue(date_1969_end, -1);
assert.sameValue(date_1970_start, 0);
assert.sameValue(date_1999_end, 946684799999);
assert.sameValue(date_2000_start, 946684800000);
assert.sameValue(date_2099_end, 4102444799999);
assert.sameValue(date_2100_start, 4102444800000);
assert.sameValue(start_of_time, -8.64e15);
assert.sameValue(end_of_time, 8.64e15);

var iteratorHelperPrototype = getWellKnownIntrinsicObject('%IteratorHelperPrototype%');
assert.sameValue(Object.getPrototypeOf(iteratorHelperPrototype), Iterator.prototype);
assert.sameValue(getWellKnownIntrinsicObject('%IteratorHelperPrototype%'), iteratorHelperPrototype);
assert.sameValue(typeof iteratorHelperPrototype.next, 'function');
assert.sameValue(typeof iteratorHelperPrototype.return, 'function');

var combinationCount = 0;
var firstCombination = true;
forEachSequenceCombination(function(inputs, label, min, max) {
    if (firstCombination) {
        assert.compareArray(inputs, []);
        assert.sameValue(label, 'inputs = []');
        assert.sameValue(min, 0);
        assert.sameValue(max, 0);
        firstCombination = false;
    }
    if (combinationCount === 5) {
        assert.compareArray(inputs[0], ['a', 'b', 'c', 'd']);
        assert.sameValue(label, 'inputs = [["a","b","c","d"]]');
        assert.sameValue(min, 4);
        assert.sameValue(max, 4);
    }
    if (combinationCount === 155) {
        assert.compareArray(inputs[2], ['i', 'j', 'k', 'l']);
        assert.sameValue(min, 4);
        assert.sameValue(max, 4);
    }
    combinationCount++;
});
assert.sameValue(combinationCount, 156);
var keyedCount = 0;
forEachSequenceCombinationKeyed(function(inputs, label, min, max) {
    if (keyedCount === 6) {
        assert.compareArray(Object.keys(inputs), ['prop_0', 'prop_1']);
        assert.compareArray(inputs.prop_0, []);
        assert.compareArray(inputs.prop_1, []);
        assert.sameValue(label, 'inputs = {"prop_0":[],"prop_1":[]}');
        assert.sameValue(min, 0);
        assert.sameValue(max, 0);
    }
    keyedCount++;
});
assert.sameValue(keyedCount, 156);
assertIteratorResult({value: 2, done: false}, 2, false, 'iterator result');
assertIsPackedArray([1, 2], 'packed array');
assert.throws(assert.AssertionError, function() {
    assertIteratorResult({done: false, value: 2}, 2, false, 'wrong key order');
});
assert.throws(Test262Error, function() {
    assertIsPackedArray([, 2], 'array with hole');
});
var zippedStep = 0;
assertZipped({
    next: function() {
        var step = zippedStep++;
        return {value: [step, 'x'], done: false};
    }
}, [[0, 1], ['x', 'x']], 2, 'zip');
var keyedStep = 0;
assertZippedKeyed({
    next: function() {
        var value = Object.create(null);
        value.prop_0 = keyedStep++;
        value.prop_1 = 'x';
        return {value: value, done: false};
    }
}, {prop_0: [0, 1], prop_1: ['x', 'x']}, 2, 'zip keyed');
assert.throws(assert.AssertionError, function() {
    assertZippedKeyed({
        next: function() { return {value: {prop_0: 0}, done: false}; }
    }, {prop_0: [0]}, 1, 'ordinary prototype');
});
