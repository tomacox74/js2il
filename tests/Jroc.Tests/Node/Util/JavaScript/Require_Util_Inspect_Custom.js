"use strict";

const util = require('util');

const custom = Symbol.for('nodejs.util.inspect.custom');
console.log(util.inspect.custom === custom);

const obj = { a: 1 };
obj[custom] = function (depth, options, inspect) {
    return `custom:${depth}:${typeof options}:${typeof inspect}:${this.a}`;
};

console.log(util.inspect(obj, { depth: 2 }));
