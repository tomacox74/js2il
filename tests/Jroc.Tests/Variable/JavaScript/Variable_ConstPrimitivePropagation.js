"use strict";

const NUMBER = 32;
const TEXT = "constant";
const FLAG = true;
const NULL_VALUE = null;

class Reader {
  read() {
    console.log(NUMBER / 2);
    console.log(TEXT);
    console.log(FLAG);
    console.log(NULL_VALUE);
  }

  shadow() {
    let NUMBER = 7;
    console.log(NUMBER);
  }
}

const reader = new Reader();
reader.read();
reader.shadow();

let mutable = 1;
mutable = 2;
console.log(mutable);

function readLate() {
  return LATE;
}

try {
  readLate();
  console.log("NO_TDZ");
} catch (error) {
  console.log("TDZ");
}

const LATE = 99;
console.log(readLate());

const WRITTEN = 3;

class WrittenReader {
  read() {
    return WRITTEN;
  }
}

try {
  WRITTEN = 4;
  console.log("NO_CONST_ERROR");
} catch (error) {
  console.log(error.name);
}

console.log(new WrittenReader().read());

const WRITTEN_PATTERN = 6;

class PatternReader {
  read() {
    return WRITTEN_PATTERN;
  }
}

try {
  [WRITTEN_PATTERN] = [7];
} catch (error) {
  console.log(error.name);
}

console.log(new PatternReader().read());

const WRITTEN_LOOP = 8;

class LoopReader {
  read() {
    return WRITTEN_LOOP;
  }
}

try {
  for (WRITTEN_LOOP of [9]) {
  }
} catch (error) {
  console.log(error.name);
}

console.log(new LoopReader().read());

const [DESTRUCTURED] = "ab";

class DestructuredReader {
  read() {
    return DESTRUCTURED;
  }
}

console.log(new DestructuredReader().read());

function sameCallableTdz() {
  try {
    console.log(SAME_CALLABLE_TDZ);
  } catch (error) {
    console.log("SAME_TDZ");
  }

  const SAME_CALLABLE_TDZ = 11;
  return () => SAME_CALLABLE_TDZ;
}

console.log(sameCallableTdz()());

let skippedReader;
switch (1) {
  case 0:
    const SKIPPED = 5;
  case 1:
    skippedReader = () => SKIPPED;
    break;
}
