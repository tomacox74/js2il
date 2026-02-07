"use strict";

// RegExp.prototype getter properties: global / ignoreCase / multiline / source

var r1 = /ab/gim;
console.log('r1.source=' + r1.source);
console.log('r1.global=' + r1.global);
console.log('r1.ignoreCase=' + r1.ignoreCase);
console.log('r1.multiline=' + r1.multiline);

var r2 = /cd/;
console.log('r2.source=' + r2.source);
console.log('r2.global=' + r2.global);
console.log('r2.ignoreCase=' + r2.ignoreCase);
console.log('r2.multiline=' + r2.multiline);
