"use strict";

async function* values() {
  try {
    yield "before";
    await Promise.reject(new Error("await boom"));
    yield "unreachable";
  } catch (error) {
    console.log("catch=" + error.message);
    yield "caught";
  } finally {
    console.log("finally");
  }

  yield "after";
}

(async () => {
  for await (const value of values()) {
    console.log("value=" + value);
  }
})();
