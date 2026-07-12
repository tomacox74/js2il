"use strict";

const points = [];
for (let i = 0; i < 1000; i++) {
    points.push({ x: i, y: i + 1 });
}

let checksum = 0;
for (let iteration = 0; iteration < 100; iteration++) {
    for (let i = 0; i < points.length; i++) {
        const point = points[i];
        checksum += point.x + point.y;
    }
}

if (checksum !== 100000000) {
    throw new Error("Unexpected plain-object checksum: " + checksum);
}
