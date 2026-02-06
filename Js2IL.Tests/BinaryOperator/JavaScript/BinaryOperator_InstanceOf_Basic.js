"use strict";

function A() {}
A.prototype = {};

function B() {}
B.prototype = new A();

var b = new B();
console.log(b instanceof B);
console.log(b instanceof A);

function Node() {}
Node.prototype = {};

var n = new Node();
console.log(n instanceof Node);
console.log(({}) instanceof Node);
console.log((5) instanceof Node);
