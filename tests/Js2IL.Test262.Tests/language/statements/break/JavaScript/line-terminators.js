// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Since LineTerminator between "break" and Identifier is not allowed,
    "break" is evaluated without label
es5id: 12.8_A2
description: >
    Checking by using eval, inserting LineTerminator between break and
    Identifier
---*/

function assert(value, message) {
  console.log(!!value);
}
assert.sameValue = function(actual, expected, message) {
  console.log(Object.is(actual, expected));
};
assert.notSameValue = function(actual, unexpected, message) {
  console.log(!Object.is(actual, unexpected));
};
assert.throws = function(expectedErrorConstructor, func, message) {
  try {
    func();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedErrorConstructor);
  }
};

var afterBreak1, afterBreak2, afterBreak3, afterBreak4;

FOR1 : for(var i=1;i<2;i++){
  LABEL1 : do {
    break
afterBreak1;
  } while(0);
}

assert.sameValue(i, 2, '#1: Since LineTerminator(U-000A) between break and Identifier not allowed break evaluates without label');

FOR2 : for(var i=1;i<2;i++){
  LABEL2 : do {
    breakafterBreak2;
  } while(0);
}

assert.sameValue(i, 2, '#2: Since LineTerminator(U-000D) between break and Identifier not allowed break evaluates without label');

FOR3 : for(var i=1;i<2;i++){
  LABEL3 : do {
    break afterBreak3;
  } while(0);
}

assert.sameValue(i, 2, '#3: Since LineTerminator(U-2028) between break and Identifier not allowed break evaluates without label');

FOR4 : for(var i=1;i<2;i++){
  LABEL4 : do {
    break afterBreak4;
  } while(0);
}

assert.sameValue(i, 2, '#4: Since LineTerminator(U-2029) between break and Identifier not allowed break evaluates without label');

console.log(true);

