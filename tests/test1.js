var x = 1 + 2;
console.log('x is ', x);


class A {
    static a1() {
        if (someOtherCondtion) {
            B.b1();
        }
    }
}

class B {
    static b1() {
        if (someCondtion) {
            A.a1();
        }
    }
}