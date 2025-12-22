let count = 0;

const id = setInterval(() => {
    count++;
    console.log("tick " + count);

    if (count === 3) {
        clearInterval(id);
        console.log("cleared");
    }
}, 10);  // 10ms interval for fast testing

console.log("setInterval scheduled");
