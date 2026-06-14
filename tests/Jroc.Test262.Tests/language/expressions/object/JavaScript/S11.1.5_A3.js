// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: "Evaluate the production ObjectLiteral: { PropertyNameAndValueList }"
es5id: 11.1.5_A3
description: >
    Creating the object defined with "var object = {0 : 1, "1" : "x",
    o : {}}"
---*/

function assert(value) {
  console.log(!!value);
}

assert.sameValue = function(actual, expected) {
  console.log(Object.is(actual, expected));
};

var object = {0 : 1, "1" : "x", o : {}};

assert.sameValue(object[0], 1);
assert.sameValue(object["1"], "x");
assert.sameValue(typeof object.o, "object");
