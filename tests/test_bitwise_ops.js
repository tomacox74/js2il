// Test bitwise operations that are used in the prime sieve

function testBitwiseOps() {
    console.log("Testing bitwise operations:");
    
    // Test unsigned right shift (>>>)
    const size = 100;
    const result1 = size >>> 5;  // Should be 3 (100 / 32)
    console.log("100 >>> 5 =", result1, "(expected: 3)");
    
    // Test bitwise AND (&)
    const index = 67;
    const result2 = index & 31;  // Should be 3 (67 % 32)
    console.log("67 & 31 =", result2, "(expected: 3)");
    
    // Test left shift (<<)
    const bitOffset = 3;
    const result3 = 1 << bitOffset;  // Should be 8
    console.log("1 << 3 =", result3, "(expected: 8)");
    
    // Test bitwise OR (|)
    let value = 0;
    value |= (1 << 0);  // Set bit 0
    value |= (1 << 2);  // Set bit 2
    value |= (1 << 5);  // Set bit 5
    console.log("After setting bits 0, 2, 5: value =", value, "(expected: 37)");
    
    // Test bitwise AND for checking bit
    const testBit = value & (1 << 2);  // Check bit 2
    console.log("value & (1 << 2) =", testBit, "(expected: 4, non-zero means true)");
    
    // Test compound bitwise OR
    const arr = new Int32Array(4);
    arr[0] = 5;  // Binary: 101
    arr[0] |= (1 << 1);  // Set bit 1
    console.log("After arr[0] |= (1 << 1):", arr[0], "(expected: 7)");
    
    // Test comparison with bitwise result
    const testVal = 15;
    const checkBit = testVal & (1 << 3);
    if (checkBit) {
        console.log("Bit 3 is set in 15 (correct)");
    } else {
        console.log("Bit 3 is NOT set in 15 (WRONG!)");
    }
}

testBitwiseOps();
