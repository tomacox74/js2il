"use strict";

class Counter {
  run(n) {
    let i = 0;
    while (i < n) {
      ++i;
    }
  }
}

const c = new Counter();
c.run(3);

console.log("Classes_ClassMethod_While_Increment_Prefix");
