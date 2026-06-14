// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: String.prototype.search (regexp) ignores global properties of regexp
es5id: 15.5.4.12_A3_T2
description: >
    Checking results of search regexp with and without global
    properties. Unicode symbols used
---*/

var value = new String("power of the power of the power of the power of the power of the power of the great sword");
console.log(value.search(/of/) === value.search(/of/g));
