// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: The isNaN property has not prototype property
esid: sec-isnan-number
description: Checking isNaN.prototype
---*/
console.log(Object.is(isNaN.prototype, undefined));
