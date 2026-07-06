// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The PropertyName is undefined, ToString(BooleanLiteral),
    ToString(nullLiteral)
es5id: 11.1.5_A4.3
description: "Creating properties with following names: undefined, 'true', 'null'"
---*/

var object = {undefined : true};
assert(object.undefined === true);
assert(object["undefined"] === true);

object = {"true" : true};
assert(object["true"] === true);

object = {"null" : true};
assert(object["null"] === true);
