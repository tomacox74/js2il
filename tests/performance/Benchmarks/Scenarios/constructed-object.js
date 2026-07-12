"use strict";

function Point(x, y) {
    this.x = x;
    this.y = y;
}

let checksum = 0;
const points = [];
for (let i = 0; i < 1000; i++) {
    const point = new Point(i, i + 1);
    point.x += 1;
    point.y += 2;
    points.push(point);
}

for (let iteration = 0; iteration < 100; iteration++) {
    for (let i = 0; i < points.length; i++) {
        const point = points[i];
        checksum += point.x + point.y;
    }
}

if (checksum !== 100300000) {
    throw new Error("Unexpected constructed-object checksum: " + checksum);
}
