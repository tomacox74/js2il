"use strict";

function Outer() {
    const arrow = () => {
        console.log(new.target === Outer);
    };
    arrow();
}

new Outer();
Outer();
