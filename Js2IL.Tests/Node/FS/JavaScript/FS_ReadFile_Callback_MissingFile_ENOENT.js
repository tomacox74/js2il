"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const missing = path.join(os.tmpdir(), 'js2il-does-not-exist.txt');
try { fs.unlinkSync(missing); } catch { }

fs.readFile(missing, 'utf8', (err, content) => {
    console.log('HasError:', !!err);
    console.log('Message:', err ? err.message : '');
    console.log('ContentIsUndefined:', content === undefined);
});
