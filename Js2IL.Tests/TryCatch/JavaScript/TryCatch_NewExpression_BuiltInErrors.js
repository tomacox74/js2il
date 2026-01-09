// PL3.3a: NewExpression for built-in Error types
try {
    throw new TypeError(123);
} catch (e) {
    console.log(e);
}

const e2 = new Error("boom");
console.log(e2);
