"use strict";\r\n\r\nconst fs = require('fs');
const list = fs.readdirSync('__no_such_dir__');
console.log(list.length);
