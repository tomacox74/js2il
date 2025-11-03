// Recursive IIFE pattern similar to walk() usage in cleanUnusedSnapshots.js
(function walk(node, depth) {
    console.log('visit', node.name, depth);
    if (depth > 2) {
        return;
    }
    for (var i = 0; i < node.children.length; i++) {
        var child = node.children[i];
        walk(child, depth + 1);
    }
})({ name: 'root', children: [ { name: 'a', children: [] }, { name: 'b', children: [ { name: 'b1', children: [] } ] } ] }, 0);
