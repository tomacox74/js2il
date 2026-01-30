"use strict";

// call on expression, not an identifier
console.log(('abc').replace('b','x'));
console.log(('abc').replace(/b/g,'x'));
