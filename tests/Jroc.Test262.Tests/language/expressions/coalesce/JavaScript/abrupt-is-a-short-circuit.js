// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x;
function poison() {
    throw new Test262Error('poison handled');
}

function morePoison() {
    throw 'poison!!!!';
}

x = undefined;
assert.throws(Test262Error, function() {
    undefined ?? poison() ?? morePoison();
}, 'undefined ?? poison() ?? morePoison();');

x = undefined;
assert.throws(Test262Error, function() {
    null ?? poison() ?? morePoison();
}, 'null ?? poison() ?? morePoison();');

assert.throws(Test262Error, function() {
    poison() ?? morePoison();
}, 'poison() ?? morePoison();');
