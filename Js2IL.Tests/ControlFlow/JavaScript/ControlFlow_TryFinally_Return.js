function f() {
  try {
    console.log("a");
    return;
  } finally {
    console.log("b");
  }
}

f();
