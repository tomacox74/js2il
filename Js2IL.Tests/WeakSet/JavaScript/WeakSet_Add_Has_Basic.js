"use strict";

var ws = new WeakSet();
var obj = {};
ws.add(obj);
console.log(ws.has(obj));
var obj2 = {};
console.log(ws.has(obj2));
