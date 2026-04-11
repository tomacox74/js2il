"use strict";

class Counter {
  constructor(start) {
    this.value = start;
  }

  async add(delta) {
    await Promise.resolve(0);
    this.value = this.value + delta;
    return this.value;
  }

  async getValue() {
    await Promise.resolve(0);
    return this.value;
  }
}

module.exports = {
  Counter,
};
