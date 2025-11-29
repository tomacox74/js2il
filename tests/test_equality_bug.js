// Simple test to check equality comparison bug

function testEquality() {
    let step = 3;
    console.log('step =', step);
    console.log('step == 3?', step == 3);
    console.log('step == 5?', step == 5);
    
    step = 5;
    console.log('step =', step);
    console.log('step == 3?', step == 3);
    console.log('step == 5?', step == 5);
    
    step = 7;
    console.log('step =', step);
    console.log('step == 3?', step == 3);
    console.log('step == 7?', step == 7);
}

testEquality();
