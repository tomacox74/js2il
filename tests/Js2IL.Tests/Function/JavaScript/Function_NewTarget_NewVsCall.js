"use strict";

function C() {
    console.log(new.target === C);
    console.log(new.target == null ? "undefined" : "defined");
}

new C();
C();
