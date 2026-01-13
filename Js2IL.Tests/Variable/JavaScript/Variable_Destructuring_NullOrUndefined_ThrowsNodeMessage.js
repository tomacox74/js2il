function logError(fn) {
  try {
    fn();
  } catch (e) {
    console.log(e.name + ': ' + e.message);
  }
}

let x = null;
logError(() => {
  const { a } = x;
});

x = undefined;
logError(() => {
  const { a } = x;
});

let y = null;
logError(() => {
  const [b] = y;
});

y = undefined;
logError(() => {
  const [b] = y;
});
