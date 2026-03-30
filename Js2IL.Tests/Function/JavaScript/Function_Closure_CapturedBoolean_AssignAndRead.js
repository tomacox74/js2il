"use strict";

function outer() {
  let flag = false;

  function setTrue() {
    flag = true;
  }

  console.log(flag);
  setTrue();
  console.log(flag);
}

outer();
