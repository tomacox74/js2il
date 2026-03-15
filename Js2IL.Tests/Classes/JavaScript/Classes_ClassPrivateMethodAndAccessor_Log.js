"use strict";

class Counter {
  #value = 2;

  #double() {
    return this.#value * 2;
  }

  get #secret() {
    return this.#double() + 1;
  }

  set #secret(value) {
    this.#value = value;
  }

  log() {
    console.log(this.#secret);
    this.#secret = 4;
    console.log(this.#double());
  }
}

const counter = new Counter();
counter.log();
console.log(Object.getOwnPropertyNames(counter).includes("#double"));
console.log(Object.getOwnPropertyNames(counter).includes("#secret"));
