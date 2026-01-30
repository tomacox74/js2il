"use strict";

const x = 5;
try {
  x = 10; // should trigger runtime error (not yet implemented)
  console.log('NO_ERROR');
} catch (e) {
  console.log('CONST_REASSIGN_ERROR');
}
