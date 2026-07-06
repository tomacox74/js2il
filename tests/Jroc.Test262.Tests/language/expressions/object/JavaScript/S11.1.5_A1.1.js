// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: "Evaluate the production ObjectLiteral: { }"
es5id: 11.1.5_A1.1
description: >
    Checking various properteis of the object defined with "var object
    = {}"
---*/

var object = {};

assert.sameValue(typeof object, "object");
assert(object instanceof Object);
assert.sameValue(object.toString, Object.prototype.toString);
assert.sameValue(object.toString(), "[object Object]");
