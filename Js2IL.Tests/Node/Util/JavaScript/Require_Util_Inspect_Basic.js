"use strict";

const util = require('util');

// Test inspect with basic values
console.log(util.inspect(42));
console.log(util.inspect('hello'));
console.log(util.inspect(true));
console.log(util.inspect(null));
console.log(util.inspect(undefined));
console.log(util.inspect([1, 2, 3]));
