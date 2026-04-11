"use strict";

const util = require('util');

console.log(util.types.isMap(new Map()));
console.log(util.types.isSet(new Set()));
console.log(util.types.isProxy(new Proxy({}, {})));
console.log(util.types.isTypedArray(new Int32Array(3)));
console.log(util.types.isTypedArray(Buffer.from([1, 2, 3])));
console.log(util.types.isRegExp(/abc/));
