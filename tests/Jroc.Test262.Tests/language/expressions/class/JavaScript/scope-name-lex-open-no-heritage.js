// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var probeBefore = function() { return C; };
var C = 'outside';

var cls = class C {
  probe() {
    return C;
  }
  modify() {
    C = null;
  }
};

assert.sameValue(probeBefore(), 'outside');
assert.sameValue(cls.prototype.probe(), cls, 'inner binding value');
assert.throws(
  TypeError, cls.prototype.modify, 'inner binding rejects modification'
);
assert.sameValue(cls.prototype.probe(), cls, 'inner binding is immutable');
