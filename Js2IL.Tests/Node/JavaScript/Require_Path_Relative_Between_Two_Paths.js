const path = require('path');
const from = path.resolve(__dirname, 'a', 'b');
const to = path.resolve(__dirname, 'a', 'c', 'd');
console.log(path.relative(from, to));
