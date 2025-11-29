// Test parameter comparison bug

function testParam(step, range_stop) {
    console.log('step=' + step + ', range_stop=' + range_stop);
    console.log('step==3? ' + (step == 3));
    console.log('step==5? ' + (step == 5));
    console.log('step==7? ' + (step == 7));
}

testParam(3, 100);
testParam(5, 100);
testParam(7, 100);
