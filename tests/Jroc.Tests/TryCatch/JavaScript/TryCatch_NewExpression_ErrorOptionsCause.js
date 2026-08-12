const cause = new Error("root");
const withCause = new Error("outer", { cause });
const withoutCause = new Error("plain", null);

console.log(withCause.message);
console.log(withCause.cause.message);
console.log(withoutCause.message);
