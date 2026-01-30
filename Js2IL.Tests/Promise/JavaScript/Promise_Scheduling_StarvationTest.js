"use strict";\r\n\r\n// the maximum number of microtasks that can execute sequentially is 1024 before the next macrotask is executed
// we test this by enquening 2048 promises and 1 setTimer
console.log("Creating 2048 Promises")
for (let i = 0; i < 2048; i++) {
    Promise.resolve(i).then((id) => { 
        if (id === 512)
        {
            console.log("[then] scheduling a timer at promise number:", id);
            setTimeout(() => {
                console.log("setTimer executed");
            }, 0);
        }

        // logging output from all the promises would make the test slow
        if (id === 1023 || id === 2047) {
            console.log("executed promise number:", id);
        }
    });
}
