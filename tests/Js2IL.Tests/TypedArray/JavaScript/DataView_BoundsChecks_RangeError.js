"use strict";

const buffer = new ArrayBuffer(4);
const view = new DataView(buffer, 1, 2);

try {
    view.getUint32(0);
} catch (e) {
    console.log(e.name);
}

try {
    new DataView(buffer, 5);
} catch (e) {
    console.log(e.name);
}
