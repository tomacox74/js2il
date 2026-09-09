async function main() {
    for (const borrowed of [false, true]) {
        const events = [];
        function mapper(value) {
            events.push("map");
            return value;
        }
        const pending = borrowed
            ? Array.fromAsync.call(Array, [1], mapper)
            : Array.fromAsync([1], mapper);
        Promise.resolve().then(function () { events.push("tick"); });
        await pending;
        console.log(borrowed, events.join(","));
    }
    const emptyEvents = [];
    function Result() {
        Object.defineProperty(this, "length", {
            set(value) { emptyEvents.push("length " + value); }
        });
    }
    const pendingEmpty = Array.fromAsync.call(Result, []);
    emptyEvents.push("returned");
    await pendingEmpty;
    console.log("empty", emptyEvents.join(","));
}
main().catch(function (error) { console.log("unexpected", error); });
