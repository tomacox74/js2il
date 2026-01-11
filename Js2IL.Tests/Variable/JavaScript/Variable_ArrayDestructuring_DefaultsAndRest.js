// Array destructuring with defaults and rest
const arr = [10];
const [a = 1, b = 2, ...rest] = arr;

console.log('a=', a);
console.log('b=', b);
console.log('rest.length=', rest.length);
console.log('rest0=', rest[0]);
