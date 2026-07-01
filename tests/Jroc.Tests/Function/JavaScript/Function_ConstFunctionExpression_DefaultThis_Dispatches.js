globalThis.marker = "global";

function Outer() {
    this.marker = "outer";

    const f = function (x = this.marker) {
        return x;
    };

    console.log(f());
}

new Outer();
