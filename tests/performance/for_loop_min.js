"use strict";
class C { run() { let i = 0; const q = 10; while (i < q) { i = i + 1; } return i; } }
console.log(new C().run());
