// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Evaluate the production ObjectLiteral: { NumericLiteral :
    AssignmentExpression}
es5id: 11.1.5_A1.2
description: >
    Checking various properteis and contents of the object defined
    with "var object = {1 : true}"
---*/

var object = {1 : true};

assert.sameValue(typeof object, "object");
assert(object instanceof Object);
assert.sameValue(object.toString, Object.prototype.toString);
assert.sameValue(object[1], true);
assert.sameValue(object["1"], true);
