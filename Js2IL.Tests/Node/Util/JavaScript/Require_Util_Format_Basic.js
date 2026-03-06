"use strict";

const util = require('util');

console.log(util.format('%s', 'foo'));
console.log(util.format('%d', 42));
console.log(util.format('%% %s', 'x'));
console.log(util.format('%j', { a: 1 }));

const circ = {};
circ.self = circ;
console.log(util.format('%j', circ));

console.log(util.format('a', 1, { b: 2 }));
console.log(util.format({ a: 1 }, { b: 2 }));
console.log(util.format('x%y', 1));
