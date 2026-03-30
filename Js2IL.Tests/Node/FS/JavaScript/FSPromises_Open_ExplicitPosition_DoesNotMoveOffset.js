"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

(async () => {
    const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
    const file = path.join(os.tmpdir(), `js2il-fs-open-position-${uniqueSuffix}.txt`);

    try {
        fs.rmSync(file, { force: true });
        fs.writeFileSync(file, 'hello', 'utf8');

        const handle = await fs.promises.open(file, 'r+');
        const explicitReadBuffer = Buffer.alloc(1);
        const implicitReadBuffer = Buffer.alloc(1);

        const explicitRead = await handle.read(explicitReadBuffer, 0, explicitReadBuffer.length, 2);
        const implicitRead = await handle.read(implicitReadBuffer, 0, implicitReadBuffer.length, null);
        const explicitWrite = await handle.write('X', 4, 'utf8');
        const implicitWrite = await handle.write('Y', null, 'utf8');
        await handle.close();

        console.log('ExplicitRead:', explicitReadBuffer.toString('utf8'), explicitRead.bytesRead);
        console.log('ImplicitReadAfterExplicit:', implicitReadBuffer.toString('utf8'), implicitRead.bytesRead);
        console.log('ExplicitWriteBytes:', explicitWrite.bytesWritten);
        console.log('ImplicitWriteBytes:', implicitWrite.bytesWritten);
        console.log('FinalText:', fs.readFileSync(file, 'utf8'));
    } catch (err) {
        console.log('Error:', err && err.message);
    } finally {
        fs.rmSync(file, { force: true });
    }
})();
