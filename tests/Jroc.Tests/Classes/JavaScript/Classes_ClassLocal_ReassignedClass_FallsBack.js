class Box {
    read() {
        return 1;
    }
}

function test() {
    let value = new Box();
    value = null;

    try {
        value.read();
    }
    catch {
        console.log("caught");
    }
}

test();
