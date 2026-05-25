// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The production CharacterEscape :: t evaluates by returning
    the character 	
es5id: 15.10.2.10_A1.1_T1
description: Use 	 in RegExp and 	 in tested string
---*/

var first = /	/.exec("	");
console.log(first !== null && first[0] === "	");

var second = /		/.exec("a		b");
console.log(second !== null && second[0] === "		");
