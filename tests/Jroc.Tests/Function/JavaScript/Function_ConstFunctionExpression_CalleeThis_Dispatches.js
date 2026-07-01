globalThis.marker = "global";

function readThis() {
    return this.marker;
}

function Outer() {
    this.marker = "outer";

    const f = function () {
        return readThis();
    };

    console.log(f());
}

new Outer();
