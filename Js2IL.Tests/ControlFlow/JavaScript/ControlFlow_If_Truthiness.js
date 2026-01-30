"use strict";

const values = [0, 1, "", "hi", null, undefined, [], [1], {}, {a:1}];
for (let i = 0; i < values.length; i++) {
  if (values[i]) {
    console.log("T");
  } else {
    console.log("F");
  }
}
