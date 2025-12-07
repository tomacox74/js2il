// Test object destructuring with default values
const obj = { provided: 42 };
const { provided, missing = 'default', count = 100 } = obj;

console.log('provided=', provided);
console.log('missing=', missing);
console.log('count=', count);
