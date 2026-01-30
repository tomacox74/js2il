"use strict";\r\n\r\nconst fs = require('fs');
const s = fs.statSync('__no_such_file__');
console.log(s.size);
