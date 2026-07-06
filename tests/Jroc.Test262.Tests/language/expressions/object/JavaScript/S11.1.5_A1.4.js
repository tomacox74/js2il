// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Evaluate the production ObjectLiteral: { Identifier :
    AssignmentExpression}
es5id: 11.1.5_A1.4
description: >
    Checking various properteis and contents of the object defined
    with "var object = {prop : true}"
---*/

var object = {prop : true};

assert.sameValue(typeof object, "object");
assert(object instanceof Object);
assert.sameValue(object.toString, Object.prototype.toString);
assert.sameValue(object["prop"], true);
assert.sameValue(object.prop, true);
