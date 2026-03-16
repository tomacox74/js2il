"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

(async () => {
    const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
    const root = path.join(os.tmpdir(), `js2il-fs-rename-existing-dir-${uniqueSuffix}`);
    const source = path.join(root, 'source');
    const destination = path.join(root, 'destination');
    const sentinel = path.join(destination, 'keep.txt');

    try {
        fs.rmSync(root, { recursive: true, force: true });
        fs.mkdirSync(source, { recursive: true });
        fs.mkdirSync(destination, { recursive: true });
        fs.writeFileSync(sentinel, 'keep', 'utf8');

        try {
            await fs.promises.rename(source, destination);
            console.log('RenameResult:', 'success');
        } catch (err) {
            console.log('RenameStartsWithEPERM:', /^EPERM: /.test(err && err.message));
        }

        console.log('SourceExists:', fs.existsSync(source));
        console.log('DestinationExists:', fs.existsSync(destination));
        console.log('SentinelExists:', fs.existsSync(sentinel));
    } finally {
        fs.rmSync(root, { recursive: true, force: true });
    }
})();
