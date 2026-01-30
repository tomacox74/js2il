"use strict";\r\n\r\nfunction logError(fn) {
  try {
    fn();
  } catch (e) {
    console.log(e.name + ': ' + e.message);
  }
}

let x = null;
logError(() => {
  const { a } = x;
  a;
});

x = undefined;
logError(() => {
  const { a } = x;
  a;
});

let y = null;
logError(() => {
  const [b] = y;
  b;
});

y = undefined;
logError(() => {
  const [b] = y;
  b;
});
