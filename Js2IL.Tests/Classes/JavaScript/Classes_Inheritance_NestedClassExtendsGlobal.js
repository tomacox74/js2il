"use strict";\r\n\r\nclass GlobalBase {
    m() {
        return 5;
    }
}

function makeAndRun() {
    class NestedDerived extends GlobalBase {
        n() {
            return super.m() + 1;
        }
    }

    console.log(new NestedDerived().n());
}

makeAndRun();
