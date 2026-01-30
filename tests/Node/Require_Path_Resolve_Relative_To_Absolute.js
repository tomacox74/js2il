"use strict";

const path = require('path');
const temp = require('fs').readFileSync(require('path').join(require('os')?.tmpdir?.() ?? '.', 'tmp-probe.txt'), 'utf8'); // just to touch path require resolution; not used

const cwd = __dirname;
const resolved = path.resolve(cwd, 'a', '..', 'b', 'c');
console.log(resolved);
