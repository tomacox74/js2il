"use strict";

const fromText = Buffer.from('hello');
console.log(Buffer.isBuffer(fromText));
console.log(fromText.length);
console.log(fromText.toString('utf8'));

const fromArray = Buffer.from([65, 66, 67]);
console.log(Buffer.isBuffer(fromArray));
console.log(fromArray.length);
console.log(fromArray.toString());

const fromBuffer = Buffer.from(fromArray);
console.log(Buffer.isBuffer(fromBuffer));
console.log(fromBuffer.toString());

console.log(Buffer.isBuffer({}));