// Exercise newly-implemented Array.prototype methods (non-iterator APIs)

var arr = [1, 2, 3, 2];
console.log(arr.indexOf(2));
console.log(arr.lastIndexOf(2));
console.log(arr.findIndex(function (x) { return x === 2; }));
console.log(arr.findLast(function (x) { return x === 2; }));
console.log(arr.findLastIndex(function (x) { return x === 2; }));

var a = [1, 2, 3];
a.forEach(function (x) { console.log(x); });

var b = [1, 2, 3, 4];
var evens = b.filter(function (x) { return x % 2 === 0; });
console.log(evens.join());
console.log(b.every(function (x) { return x > 0; }));
console.log(b.reduce(function (acc, x) { return acc + x; }, 0));
console.log(b.reduceRight(function (acc, x) { return acc - x; }));

var d = [1, 2, 3];
console.log(d.shift());
console.log(d.join());
console.log(d.unshift(9, 8));
console.log(d.join());

var e = [1, 2, 3];
console.log(e.reverse().join());

var f = [1, 2].concat([3, 4], 5);
console.log(f.join());

var g = [1, [2, [3]]];
console.log(g.flat(2).join());

var h = [1, 2, 3];
console.log(h.flatMap(function (x) { return [x, x]; }).join());

var i = [1, 2, 3];
console.log(i.at(-1));
console.log(i.at(0));

var j = [0, 0, 0, 0];
j.fill(7, 1, 3);
console.log(j.join());

var k = [1, 2, 3, 4, 5];
k.copyWithin(0, 3);
console.log(k.join());

var l = [1, 2, 3];
console.log(l.toReversed().join());
console.log(l.join());
console.log(l.toSorted(function (aa, bb) { return bb - aa; }).join());

var m = [1, 2, 3, 4];
console.log(m.toSpliced(1, 2, 9, 9).join());
console.log(m.join());
console.log(m.with(1, 8).join());
