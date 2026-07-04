var sab = new SharedArrayBuffer(4);
var view = new Int32Array(sab);

console.log(typeof SharedArrayBuffer);
console.log(typeof Atomics);
console.log(sab.byteLength);
console.log(view.buffer === sab);
console.log(view.length);
console.log(Atomics.wait(view, 0, 1, 0));
console.log(Atomics.wait(view, 0, 0, 1));
