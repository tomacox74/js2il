"use strict";

class Greeter {
  #secret = "TopSecret";
  logSecret() {
    console.log(this.#secret);
  }
}

const g = new Greeter();
g.logSecret();
