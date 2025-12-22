let count = 0;

const id = setInterval(() => {
    count++;
    console.log("tick " + count);

    if (count === 3) {
        clearInterval(id);
        console.log("cleared");
    }
}, 50);  // 50ms interval for stable CI testing

console.log("setInterval scheduled");
