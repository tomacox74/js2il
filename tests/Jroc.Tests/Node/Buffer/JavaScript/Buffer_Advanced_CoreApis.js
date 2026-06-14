"use strict";

var base = Buffer.from([1, 2, 3, 4, 5]);
var sliced = base.slice(1, 4);
sliced[0] = 99;
console.log(base[1]);

var sub = base.subarray(2, 5);
sub[1] = 77;
console.log(base[3]);

console.log(base.equals(Buffer.from([1, 99, 3, 77, 5])));
console.log(base.indexOf(77));
console.log(base.lastIndexOf(Buffer.from([77, 5])));

var filled = Buffer.alloc(6);
filled.fill("ab");
console.log(filled.toString());
console.log(filled.indexOf("ba"));
console.log(filled.lastIndexOf("ab"));
console.log(filled.includes("zz"));

var numeric = Buffer.alloc(16);
numeric.writeFloatLE(3.5, 0);
numeric.writeFloatBE(3.5, 4);
numeric.writeDoubleLE(10.25, 8);
console.log(Math.round(numeric.readFloatLE(0) * 100) / 100);
console.log(Math.round(numeric.readFloatBE(4) * 100) / 100);
console.log(Math.round(numeric.readDoubleLE(8) * 100) / 100);

numeric.writeDoubleBE(-2.5, 0);
console.log(Math.round(numeric.readDoubleBE(0) * 10) / 10);

var unsafe = Buffer.allocUnsafe(4);
unsafe[0] = 1;
unsafe[1] = 2;
unsafe[2] = 3;
unsafe[3] = 4;
console.log(unsafe.toString("hex"));
