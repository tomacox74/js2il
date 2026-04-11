"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const source = path.join(os.tmpdir(), `js2il-fs-callback-append-${uniqueSuffix}.txt`);
const renamed = path.join(os.tmpdir(), `js2il-fs-callback-append-${uniqueSuffix}-renamed.txt`);

fs.rmSync(source, { force: true });
fs.rmSync(renamed, { force: true });
fs.writeFileSync(source, 'a', 'utf8');

fs.appendFile(source, 'b', 'utf8', (appendErr) => {
    if (appendErr) {
        console.log('AppendError:', appendErr.message);
        return;
    }

    fs.rename(source, renamed, (renameErr) => {
        if (renameErr) {
            console.log('RenameError:', renameErr.message);
            return;
        }

        const text = fs.readFileSync(renamed, 'utf8');
        fs.unlink(renamed, (unlinkErr) => {
            if (unlinkErr) {
                console.log('UnlinkError:', unlinkErr.message);
                return;
            }

            console.log('RenamedText:', text);
            console.log('SourceExists:', fs.existsSync(source));
            console.log('RenamedExists:', fs.existsSync(renamed));
        });
    });
});
