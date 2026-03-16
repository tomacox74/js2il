"use strict";

const Counter = class {
  get #value() {
    return 1;
  }

  log() {
    console.log(this.#value);
  }
};

new Counter().log();
