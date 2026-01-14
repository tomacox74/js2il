console.log("Before calling async function");

function run() {
    console.log("Inside async function (before await)");

    return new Promise((resolve) => {
        setTimeout(() => {
            console.log("Timer fired");
            resolve();
        }, 0);
    }).then(() => {
        console.log("Inside async function (after await)");
    });
}

run().then(() => {
    console.log("After calling async function");
});
