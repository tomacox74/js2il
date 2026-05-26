// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Evaluate the production ObjectLiteral: { StringLiteral :
    AssignmentExpression}
es5id: 11.1.5_A1.3
description: >
    Checking various properteis and contents of the object defined
    with "var object = {"x" : true}"
---*/

function assert(value) {
  console.log(!!value);
}

assert.sameValue = function(actual, expected) {
  console.log(Object.is(actual, expected));
};

var object = {"x" : true};

assert.sameValue(typeof object, "object");
assert(object instanceof Object);
assert.sameValue(object.toString, Object.prototype.toString);
assert.sameValue(object["x"], true);
assert.sameValue(object.x, true);
