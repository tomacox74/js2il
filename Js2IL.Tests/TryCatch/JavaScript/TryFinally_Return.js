"use strict";\r\n\r\nfunction f() {
  try {
    console.log("a");
    return;
  } finally {
    console.log("b");
  }
}

f();
