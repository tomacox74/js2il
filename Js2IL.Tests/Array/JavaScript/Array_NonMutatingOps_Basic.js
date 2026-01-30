"use strict";\r\n\r\n// Non-mutating/copying Array operations

var f = [1, 2].concat([3, 4], 5);
console.log(f.join());

var g = [1, [2, [3]]];
console.log(g.flat(2).join());

var l = [1, 2, 3];
console.log(l.toReversed().join());
console.log(l.join());
console.log(l.toSorted(function (aa, bb) { return bb - aa; }).join());

var m = [1, 2, 3, 4];
console.log(m.toSpliced(1, 2, 9, 9).join());
console.log(m.join());
console.log(m.with(1, 8).join());
