"use strict";

try {
  console.log(a); // TDZ access before declaration
  let a = 3;
  console.log('NO_TDZ');
} catch (e) {
  console.log('TDZ_ERROR');
}
