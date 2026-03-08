"use strict";

var literal = /abc/y;
console.log(literal.sticky);
console.log(literal.global);
console.log(literal.flags);

var ctor = RegExp("abc", "gy");
console.log(ctor.sticky);
console.log(ctor.global);
console.log(ctor.flags);
console.log(ctor.toString());
