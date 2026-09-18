const math = globalThis["Math"];

console.log(math["max"](1, 2, 3));
console.log(math["max"](...[-4, 5, 2]));
console.log(math["min"](1, 2, 3));
console.log(math["hypot"](3, 4));
console.log(math["max"]());

const assigned = globalThis["Object"]["assign"]({}, { first: 1 }, { second: 2 });
console.log(assigned.first);
console.log(assigned.second);
