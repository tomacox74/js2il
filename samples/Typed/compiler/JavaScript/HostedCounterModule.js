"use strict";

class Counter {
  constructor(start) {
    this.value = start;
  }

  add(delta) {
    this.value += delta;
    return this.value;
  }

  getValue() {
    return this.value;
  }
}

function add(x, y) {
  return x + y;
}

// Async export demonstration:
// - In JS, an `async` function returns a Promise.
// - The hosting layer can surface Promises as Task/Task<T> (or you can bridge manually).
async function addAsync(x, y) {
  // Use a call expression so the contract generator conservatively types the return as `object`.
  // The actual resolved value is still a JavaScript number.
  return Number(x + y);
}

function createCounter(start) {
  return new Counter(start);
}

const version = "1.2.3";

module.exports = {
  version,
  add,
  addAsync,
  Counter,
  createCounter,
};
