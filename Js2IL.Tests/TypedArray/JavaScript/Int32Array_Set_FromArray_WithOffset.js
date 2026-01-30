"use strict";

const a = new Int32Array(5);
const src = [10.9, -3.4, 2];
a.set(src, 1);
for (let i = 0; i < a.length; i++) console.log(a[i]);
