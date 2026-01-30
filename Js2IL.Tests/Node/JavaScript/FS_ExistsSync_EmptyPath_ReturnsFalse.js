"use strict";

const fs = require('fs');
// Empty path should not exist
console.log(fs.existsSync(''));
