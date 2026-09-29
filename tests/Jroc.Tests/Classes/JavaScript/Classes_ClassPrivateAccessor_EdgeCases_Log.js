"use strict";

class AccessorEdges {
  get #readOnly() {
    return 1;
  }

  set #writeOnly(value) {
    this.saved = value;
  }

  log() {
    try {
      console.log(String(this.#writeOnly));
    } catch (error) {
      console.log(error.name);
      console.log(String(error.message).includes("without a getter"));
    }

    try {
      this.#readOnly = 2;
    } catch (error) {
      console.log(error.name);
      console.log(String(error.message).includes("without a setter"));
    }

    this.#writeOnly = 7;
    console.log(this.saved);
    console.log(this.#readOnly);
  }
}

new AccessorEdges().log();
