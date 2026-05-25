// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: String.prototype.replace (searchValue, replaceValue)
es5id: 15.5.4.11_A1_T1
description: Arguments are true and 1, and instance is object
---*/

var instance = new Object(true);
instance.replace = String.prototype.replace;
console.log(instance.replace(true, 1) === "1");
