function GraphNode(pos) {
    this.pos = pos;
}

function readPos(obj) {
    return obj.pos;
}

var node = new GraphNode("typed");
console.log(readPos(node));
console.log(readPos({ pos: "fallback" }));
