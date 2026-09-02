const originalObject = Object;

(function () {
    with ({
        Object: function WithObject() {
            this.withValue = 45;
        }
    }) {
        console.log(new Object().withValue);
    }
})();

(function () {
    function Object() {
        this.local = 43;
    }

    console.log(new Object().local);
})();

globalThis.Object = function Replacement() {
    this.replaced = 44;
};

const replaced = new Object();
console.log(replaced.replaced);

globalThis.Object = originalObject;
console.log(Object.getPrototypeOf(new Object()) === Object.prototype);
