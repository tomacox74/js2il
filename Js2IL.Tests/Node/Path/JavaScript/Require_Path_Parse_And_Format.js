"use strict";

const path = require('path');

const parsed = path.parse('/home/user/dir/file.txt');
console.log(parsed.root);
console.log(parsed.dir);
console.log(parsed.base);
console.log(parsed.ext);
console.log(parsed.name);

console.log(path.format(parsed));
console.log(path.format({ dir: '/tmp/demo', name: 'archive', ext: '.tar' }));
console.log(path.format({ root: '/', base: 'index.js' }));
