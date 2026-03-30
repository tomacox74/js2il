"use strict";

// Domino's WindowTimers polyfill pattern relies on host timer functions being usable
// as first-class values (assignable to properties).

const window = {};
window.setTimeout = setTimeout;
window.clearTimeout = clearTimeout;
window.setInterval = setInterval;
window.clearInterval = clearInterval;

console.log(typeof window.setTimeout);
console.log(typeof window.clearTimeout);
console.log(typeof window.setInterval);
console.log(typeof window.clearInterval);

// Ensure calling via the assigned function value works when the underlying intrinsic expects
// a trailing params array (third argument for setTimeout/setInterval).
const id = window.setTimeout(function () { }, 100000);
window.clearTimeout(id);
console.log("ok");
