"use strict";\r\n\r\nconsole.log("a");

try {
  throw 123;
} catch (e) {
  console.log(e);
} finally {
  console.log("c");
}
