"use strict";

const path = require('path');

console.log(path.toNamespacedPath('/tmp/a/b') === '/tmp/a/b');
console.log(path.toNamespacedPath('relative/path') === 'relative/path');
