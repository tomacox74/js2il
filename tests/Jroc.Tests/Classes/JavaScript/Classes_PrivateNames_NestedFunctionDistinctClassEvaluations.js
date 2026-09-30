"use strict";

function makeClass() {
  return class {
    #field = 42;

    get #getter() {
      return this.#field + 1;
    }

    readers() {
      function readField(value) {
        return value.#field;
      }

      function readGetter(value) {
        return value.#getter;
      }

      return [readField, readGetter];
    }
  };
}

const A = makeClass();
const B = makeClass();
const a = new A();
const b = new B();
const [fieldA, getterA] = a.readers();
const [fieldB, getterB] = b.readers();

console.log(fieldA(a), getterA(a), fieldB(b), getterB(b));

for (const [reader, value] of [[fieldA, b], [getterA, b], [fieldB, a], [getterB, a]]) {
  try {
    reader(value);
    console.log("unexpected");
  } catch (error) {
    console.log(error.name);
  }
}

Object.setPrototypeOf(b, A.prototype);
try {
  fieldA(b);
  console.log("unexpected");
} catch (error) {
  console.log(error.name);
}
