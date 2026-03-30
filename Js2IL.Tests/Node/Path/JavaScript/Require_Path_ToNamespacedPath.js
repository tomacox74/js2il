"use strict";

const path = require('path');
const sep = path.sep;
const absolutePath = sep + ['tmp', 'a', 'b'].join(sep);
const relativePath = ['relative', 'path'].join(sep);

console.log(path.toNamespacedPath(absolutePath) === absolutePath);
console.log(path.toNamespacedPath(relativePath) === relativePath);
