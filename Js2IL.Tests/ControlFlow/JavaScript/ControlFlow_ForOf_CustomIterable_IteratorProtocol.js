"use strict";\r\n\r\nlet closed = 0;

const iterable = {
  [Symbol.iterator]: function () {
    let i = 0;
    return {
      next: function () {
        i++;
        if (i === 1) return { value: "a", done: false };
        if (i === 2) return { value: "b", done: false };
        return { value: undefined, done: true };
      },
      return: function () {
        closed++;
        return { value: "ret", done: true };
      },
    };
  },
};

for (const x of iterable) {
  console.log("break", x);
  break;
}
console.log("closed", closed);

closed = 0;
for (const x of iterable) {
  console.log("normal", x);
}
console.log("closed", closed);

closed = 0;
try {
  for (const x of iterable) {
    console.log("throw", x);
    throw new Error("boom");
  }
} catch (e) {
  console.log("caught", e.name);
}
console.log("closed", closed);

let closed2 = 0;
const iterableThrow = {
  [Symbol.iterator]: function () {
    let i = 0;
    return {
      next: function () {
        i++;
        if (i === 1) return { value: "x", done: false };
        throw new Error("nextboom");
      },
      return: function () {
        closed2++;
        return { value: undefined, done: true };
      },
    };
  },
};

try {
  for (const x of iterableThrow) {
    console.log("iter", x);
  }
} catch (e) {
  console.log("caught", e.name);
}
console.log("closed2", closed2);
