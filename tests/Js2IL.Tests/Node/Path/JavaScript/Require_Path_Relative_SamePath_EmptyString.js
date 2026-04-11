"use strict";

const path = require('path');
const same = path.resolve(__dirname, 'a', 'b');
console.log(path.relative(same, same));
