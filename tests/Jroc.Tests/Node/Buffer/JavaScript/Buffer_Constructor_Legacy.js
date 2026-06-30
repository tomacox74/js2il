const fromString = new Buffer("hello");
const fromArray = new Buffer([65, 66, 67]);
const fromSize = new Buffer(3);

console.log(fromString.toString("utf8"));
console.log(fromArray.toString("utf8"));
console.log(fromSize.length);
