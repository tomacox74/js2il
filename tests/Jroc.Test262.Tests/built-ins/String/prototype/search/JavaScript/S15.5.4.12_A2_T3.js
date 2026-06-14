// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: String.prototype.search (regexp) returns ...
es5id: 15.5.4.12_A2_T3
description: Checking disabling of case sensitive of search, argument is RegExp
---*/

var value = new String("test string");
console.log(value.search(/String/i) === 5);
