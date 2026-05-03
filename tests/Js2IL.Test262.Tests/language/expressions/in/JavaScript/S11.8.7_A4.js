// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator "in" calls ToString(ShiftExpression)
es5id: 11.8.7_A4
description: Checking ToString coversion;
---*/

var object = {};
object["true"] = 1;
console.log(Object.is(true in object, "true" in object));

var object = {};
object.Infinity = 1;
console.log(Object.is(Infinity in object, "Infinity" in object));

var object = {};
object.undefined = 1;
console.log(Object.is(undefined in object, "undefined" in object));

var object = {};
object["null"] = 1;
console.log(Object.is(null in object, "null" in object));
