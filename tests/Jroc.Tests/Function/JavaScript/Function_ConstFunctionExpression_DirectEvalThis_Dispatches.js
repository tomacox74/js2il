function Outer() {
    this.marker = "outer";

    const f = function () {
        return eval("this").marker === "outer" ? "outer" : "not outer";
    };

    console.log(f());
}

new Outer();
