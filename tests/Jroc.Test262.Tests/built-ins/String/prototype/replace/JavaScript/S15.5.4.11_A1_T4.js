// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: String.prototype.replace (searchValue, replaceValue)
es5id: 15.5.4.11_A1_T4
description: >
    Call replace (searchValue, replaceValue) function with null and
    function(a1,a2,a3){return a2+"";} arguments of function object
---*/

var result = (function() {
  return "gnulluna";
}()).replace(null, function(a1, a2, a3) {
  return a2 + "";
});

console.log(result === "g1una");
