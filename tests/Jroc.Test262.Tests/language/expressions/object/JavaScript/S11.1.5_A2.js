// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Evaluate the production ObjectLiteral: { PropertyName :
    AssignmentExpression }
es5id: 11.1.5_A2
description: Creating property "prop" of various types(boolean, number and etc.)
---*/

var x = true;
var object = {prop : x};
assert.sameValue(object.prop, x);

x = new Boolean(true);
object = {prop : x};
assert.sameValue(object.prop, x);

x = 1;
object = {prop : x};
assert.sameValue(object.prop, x);

x = new Number(1);
object = {prop : x};
assert.sameValue(object.prop, x);

x = "1";
object = {prop : x};
assert.sameValue(object.prop, x);

x = new String(1);
object = {prop : x};
assert.sameValue(object.prop, x);

x = undefined;
object = {prop : x};
assert.sameValue(object.prop, x);

x = null;
object = {prop : x};
assert.sameValue(object.prop, x);

x = {};
object = {prop : x};
assert.sameValue(object.prop, x);

x = [1,2];
object = {prop : x};
assert.sameValue(object.prop, x);

x = function() {};
object = {prop : x};
assert.sameValue(object.prop, x);

x = this;
object = {prop : x};
assert.sameValue(object.prop, x);
