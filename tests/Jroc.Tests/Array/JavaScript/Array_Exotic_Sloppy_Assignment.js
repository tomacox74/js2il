const locked = [1];
Object.defineProperty(locked, "length", { writable: false });
locked.length = 3;
locked[1] = 2;
console.log(locked.length, 1 in locked);

const frozen = [4];
Object.freeze(frozen);
frozen[0] = 9;
console.log(frozen[0]);

const blocked = [0, 1, 2];
Object.defineProperty(blocked, "1", { configurable: false });
blocked.length = 0;
console.log(blocked.length, 1 in blocked, 2 in blocked);

let coercions = 0;
const rhs = {
    valueOf: function () {
        coercions++;
        return 0;
    }
};
const noCoercion = [1];
Object.defineProperty(noCoercion, "length", { writable: false });
noCoercion.length = rhs;
console.log(coercions, noCoercion.length);
