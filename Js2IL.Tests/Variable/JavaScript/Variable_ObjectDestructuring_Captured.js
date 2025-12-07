// Test object destructuring where variables are captured by nested function
const obj = { a: 5, b: 10 };
const { a, b } = obj;

// Nested function captures 'a' and 'b' - they must be stored as fields, not locals
function compute() {
    return a * b;
}

console.log('a=', a);
console.log('b=', b);
console.log('compute()=', compute());
