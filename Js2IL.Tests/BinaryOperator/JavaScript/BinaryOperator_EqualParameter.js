// Test equality comparison with function parameters
// This tests a bug where parameter == literal was always returning true

function testParam(value) {
    var eq3 = value == 3;
    var eq5 = value == 5;
    var eq7 = value == 7;
    
    console.log("value:", value);
    console.log("value == 3:", eq3);
    console.log("value == 5:", eq5);
    console.log("value == 7:", eq7);
}

testParam(3);
testParam(5);
testParam(7);
