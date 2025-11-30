class Point {
    constructor({ x, y }) {
        this.x = x;
        this.y = y;
    }
    
    display() {
        console.log("Point: (" + this.x + ", " + this.y + ")");
    }
}

const p1 = new Point({ x: 10, y: 20 });
p1.display();
