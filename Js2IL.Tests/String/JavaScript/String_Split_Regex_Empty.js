"use strict";

var parts = "abc".split(/(?:)/);
console.log(parts.length);
console.log(parts[0]);
console.log(parts[1]);
console.log(parts[2]);

var limited = "abc".split(/(?:)/, 2);
console.log(limited.length);
console.log(limited[0]);
console.log(limited[1]);

var empty = "".split(/(?:)/);
console.log(empty.length);

var noUnicode = "😀a".split(/(?:)/);
console.log(noUnicode.length);
console.log(noUnicode[0].charCodeAt(0));
console.log(noUnicode[1].charCodeAt(0));
console.log(noUnicode[2]);

var unicode = "😀a".split(/(?:)/u);
console.log(unicode.length);
console.log(unicode[0]);
console.log(unicode[1]);
