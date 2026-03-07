"use strict";

const buffer = new ArrayBuffer(4);

try {
    const invalidOffsetView = new DataView(buffer, 5);
    console.log(invalidOffsetView.byteLength);
} catch (e) {
    console.log(e.name);
    console.log(e.message);
}

try {
    const invalidLengthView = new DataView(buffer, 0, 5);
    console.log(invalidLengthView.byteLength);
} catch (e) {
    console.log(e.name);
    console.log(e.message);
}
