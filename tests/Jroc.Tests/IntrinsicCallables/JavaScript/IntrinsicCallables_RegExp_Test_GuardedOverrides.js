"use strict";

const ownOverride = /a/;
console.log(ownOverride.test("a"));
ownOverride.test = () => "own";
console.log(ownOverride.test("a"));

RegExp.prototype.test = () => "prototype";
console.log(/a/.test("a"));
