// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The $ replacements are done left-to-right, and, once such are placement is performed, the new
    replacement text is not subject to further replacements
es5id: 15.5.4.11_A2_T1
description: >
    Don`t use $ in replaceValue, searchValue is regular expression
    /sh/g
---*/

var value = 'She sells seashells by the seashore.';
var re = /sh/g;
console.log(value.replace(re, 'sch') === 'She sells seaschells by the seaschore.');
