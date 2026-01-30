"use strict";\r\n\r\nclass Counter {
  run(n) {
    while (n < 3) {
      n++;
    }
  }
}

const c = new Counter();
c.run(0);

console.log("Classes_ClassMethod_While_Increment_Param_Postfix");
