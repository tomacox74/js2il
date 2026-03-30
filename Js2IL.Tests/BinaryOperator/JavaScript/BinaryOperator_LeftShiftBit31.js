"use strict";

// Test left shift that produces bit 31 (sign bit)
// In JavaScript, 1 << 31 should produce -2147483648 (0x80000000 as signed int32)
var mask31 = 1 << 31;
console.log('mask31 is', mask31);

// Test bitwise OR with the sign bit
var value = 0;
value |= (1 << 31);
console.log('value after OR with bit 31:', value);

// Test all bit positions to ensure they work correctly
var allBits = 0;
for (var i = 0; i < 32; i++) {
    allBits |= (1 << i);
}
console.log('all 32 bits set:', allBits);

// Test that bitwise AND works correctly with bit 31
var testValue = -2147483648;  // bit 31 set
var result = testValue & (1 << 31);
console.log('AND result with bit 31:', result);
