function processArray(arr) {
    var result = [];
    for (var i = 0; i < arr.length; i++) {
        var item = arr[i] * 2;
        result.push(item);
    }
    return result;
}

var input = [1, 2, 3, 4];
var output = processArray(input);
console.log(output.join(','));
