// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Boolean() returns false
esid: sec-terms-and-definitions-boolean-value
description: Call Boolean() and check result
---*/
console.log(Object.is(typeof Boolean(), "boolean"));
console.log(Object.is(Boolean(), false));
