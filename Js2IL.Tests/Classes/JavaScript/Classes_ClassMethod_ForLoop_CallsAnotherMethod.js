"use strict";\r\n\r\nclass Accumulator {
  constructor(start) {
    this.total = start;
  }
  add(n) {
    this.total += n;
  }
  addRange(count) {
    for (let i = 1; i <= count; i++) {
      this.add(i);
    }
  }
  log() {
    console.log(this.total);
  }
}

const acc = new Accumulator(0);
acc.addRange(5); // adds 1+2+3+4+5 = 15
acc.log();
