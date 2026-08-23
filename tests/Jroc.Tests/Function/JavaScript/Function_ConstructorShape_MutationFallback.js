function GraphNode(pos) {
    this.pos = pos;
}

function readPos(obj) {
    return obj.pos;
}

var node = new GraphNode("initial");
console.log(readPos(node));

node.pos = "plain-write";
console.log(readPos(node));

delete node.pos;
GraphNode.prototype.pos = "prototype";
console.log(readPos(node));

var getterCalls = 0;
Object.defineProperty(node, "pos", {
    configurable: true,
    get: function () {
        getterCalls++;
        return "accessor";
    }
});
console.log(readPos(node));
node.pos;
console.log(getterCalls);

Object.defineProperty(node, "pos", {
    configurable: true,
    enumerable: true,
    writable: true,
    value: "descriptor-data"
});
console.log(readPos(node));

Object.setPrototypeOf(node, { pos: "other-prototype" });
console.log(readPos(node));

node.extra = 1;
console.log(readPos(node), Object.keys(node).join(","));
