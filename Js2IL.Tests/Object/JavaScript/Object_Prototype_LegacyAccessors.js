"use strict";

var o = { _v: 1 };
Object.prototype.__defineGetter__.call(o, "x", function () { return this._v; });
Object.prototype.__defineSetter__.call(o, "x", function (v) { this._v = v; });

console.log(typeof Object.prototype.__lookupGetter__.call(o, "x") === "function");
console.log(typeof Object.prototype.__lookupSetter__.call(o, "x") === "function");

o.x = 7;
console.log(o.x);
