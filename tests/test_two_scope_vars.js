// Test reading two scope variables

class Test {
    test() {
        let a = 5;
        let b = 10;
        console.log("a =", a);
        console.log("b =", b);
        const sum = a | b;
        console.log("a | b =", sum, "(expected: 15)");
    }
}

new Test().test();
