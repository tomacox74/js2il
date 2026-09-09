async function main() {
    function* generator() { yield 1; }
    async function* asyncGenerator() { yield 1; }
    for (const borrowed of [false, true]) {
        const iterators = [
            [1][Symbol.iterator](),
            "x"[Symbol.iterator](),
            new Set([1])[Symbol.iterator](),
            new Map([[1, 2]])[Symbol.iterator](),
            generator(),
            asyncGenerator()
        ];
        for (let index = 0; index < iterators.length; index++) {
            let calls = 0;
            const iterator = iterators[index];
            iterator.next = function () {
                calls++;
                return { done: true, value: 99 };
            };
            const result = borrowed
                ? await Array.fromAsync.call(Array, iterator)
                : await Array.fromAsync(iterator);
            console.log(borrowed, index, calls, result.length);
        }
    }
}
main().catch(function (error) { console.log("unexpected", error); });
