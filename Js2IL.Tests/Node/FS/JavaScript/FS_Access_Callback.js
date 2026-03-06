"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const unique = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const file = path.join(os.tmpdir(), `js2il-access-callback-${unique}.txt`);

fs.writeFileSync(file, 'x', 'utf8');

fs.access(file, (err) => {
    console.log('ExistsErrIsNull:', err == null);
    fs.rmSync(file, { force: true });

    const missing = path.join(os.tmpdir(), `js2il-access-missing-${unique}.txt`);
    fs.rmSync(missing, { force: true });

    fs.access(missing, (err2) => {
        console.log('MissingHasError:', !!err2);
        console.log('MissingHasENOENT:', !!(err2 && err2.message && err2.message.indexOf('ENOENT:') >= 0));
    });
});
