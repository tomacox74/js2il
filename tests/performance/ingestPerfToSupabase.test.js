const assert = require('node:assert/strict');
const test = require('node:test');
const { normalizeRuntime } = require('./ingestPerfToSupabase');

test('preserves execution-phase runtime identities', () => {
    assert.equal(normalizeRuntime('jint-execute'), 'jint-execute');
    assert.equal(normalizeRuntime('yantrajs-execute'), 'yantrajs-execute');
});

test('keeps existing Jint phase distinctions', () => {
    assert.equal(normalizeRuntime('Jint prepare'), 'jint-prepare');
    assert.equal(normalizeRuntime('Jint execute (prepared)'), 'jint-execute-prepared');
});
