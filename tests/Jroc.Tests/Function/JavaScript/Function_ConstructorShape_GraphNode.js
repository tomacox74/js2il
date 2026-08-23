function GraphNode(x, y, isWall) {
    this.x = x;
    this.y = y;
    this._isWall = isWall;
    this.pos = { x: x, y: y };
    this.debug = "";
}

GraphNode.prototype.describe = function () {
    return this.x + "," + this.y;
};

var node = new GraphNode(3, 4, false);
console.log(node.describe());
console.log(node.pos.x, node.pos.y, node._isWall, node.debug);
console.log(node instanceof GraphNode);
console.log(Object.getPrototypeOf(node) === GraphNode.prototype);
console.log(Object.keys(node).join(","));

Object.freeze(node);
GraphNode.call(node, 9, 10, true);
console.log(node.x, node.y, node._isWall);

var ordinaryReceiver = {};
GraphNode.call(ordinaryReceiver, 7, 8, true);
console.log(ordinaryReceiver.x, ordinaryReceiver.y, ordinaryReceiver._isWall);

function NewTargetCheck() {
    this.matches = new.target === NewTargetCheck;
}
console.log(new NewTargetCheck().matches);

function OverrideResult() {
    this.value = "receiver";
    return { value: "override" };
}
console.log(new OverrideResult().value);
