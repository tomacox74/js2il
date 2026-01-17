// Callback-based Array operations

var a = [1, 2, 3];
a.forEach(function (x) { console.log(x); });

var b = [1, 2, 3, 4];
var evens = b.filter(function (x) { return x % 2 === 0; });
console.log(evens.join());
console.log(b.every(function (x) { return x > 0; }));
console.log(b.reduce(function (acc, x) { return acc + x; }, 0));
console.log(b.reduceRight(function (acc, x) { return acc - x; }));

console.log([1, 2, 3].flatMap(function (x) { return [x, x]; }).join());
