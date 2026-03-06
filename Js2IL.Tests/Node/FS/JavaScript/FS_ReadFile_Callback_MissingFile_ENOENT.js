"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const unique = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const missing = path.join(os.tmpdir(), `js2il-does-not-exist-${unique}.txt`);
fs.rmSync(missing, { force: true });

fs.readFile(missing, 'utf8', (err, content) => {
    console.log('HasError:', !!err);
    console.log('MessageHasENOENT:', !!(err && err.message && err.message.indexOf('ENOENT:') >= 0));
    console.log('ContentIsUndefined:', content === undefined);
});
