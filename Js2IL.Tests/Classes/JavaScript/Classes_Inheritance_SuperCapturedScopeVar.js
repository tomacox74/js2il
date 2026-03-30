"use strict";

function outer() {
    const captured = 40;

    class Base {
        m() {
            return captured + 2;
        }
    }

    class Derived extends Base {
        n() {
            return super.m();
        }
    }

    console.log(new Derived().n());
}

outer();
