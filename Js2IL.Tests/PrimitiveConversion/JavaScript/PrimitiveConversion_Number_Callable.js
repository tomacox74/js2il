"use strict";

console.log('Number():' + Number());
console.log('Number("1.5"):' + Number("1.5"));
console.log('Number(true):' + Number(true));
console.log('Number(false):' + Number(false));
console.log('Number(null):' + Number(null));

var n = Number(undefined);
console.log('Number(undefined):' + (Number.isNaN(n) ? 'NaN' : n));
