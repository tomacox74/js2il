'use strict';

const fs = require('node:fs');

function check(actual, expected) {
  if (JSON.stringify(actual) !== JSON.stringify(expected)) {
    throw new Error('Unexpected observable: ' + JSON.stringify(actual));
  }
  return actual;
}

function error(action) {
  try {
    action();
  } catch (failure) {
    return [failure.name, failure.code === undefined ? null : failure.code];
  }
  throw new Error('Expected an exception.');
}

function finish(value) {
  fs.writeFileSync('result.json', JSON.stringify(value));
}

module.exports = { check, error, finish };
