"use strict";

// Object.fromEntries ( iterable )

var entries = [['a', 1], ['b', 2], ['c', 3]];

var obj = Object.fromEntries(entries);

console.log('a=' + obj.a);
console.log('b=' + obj.b);
console.log('c=' + obj.c);
