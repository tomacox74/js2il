"use strict";

// ECMA-262 §19.2.5: parseInt ( string , radix )
// Comprehensive spec-driven test coverage for js2il runtime
// https://tc39.es/ecma262/#sec-parseint-string-radix

// ============================================================================
// Whitespace and Sign Handling
// ============================================================================
console.log("=== Whitespace and Sign Handling ===");

// Leading whitespace (space, tab, newline, etc.)
console.log(parseInt("   42"));      // 42
console.log(parseInt("\t42"));       // 42
console.log(parseInt("\n42"));       // 42
console.log(parseInt("\r\n42"));     // 42

// Leading sign
console.log(parseInt("+42"));        // 42
console.log(parseInt("-42"));        // -42
console.log(parseInt("  +42"));      // 42
console.log(parseInt("  -42"));      // -42

// Multiple signs (only first sign is valid)
console.log(parseInt("++42"));       // NaN (second + is not a digit)
console.log(parseInt("--42"));       // NaN (second - is not a digit)

// ============================================================================
// Radix Coercion Edge Cases
// ============================================================================
console.log("=== Radix Coercion Edge Cases ===");

// undefined radix (behaves like 0, which triggers auto-detection)
console.log(parseInt("10", undefined));  // 10 (defaults to base 10)
console.log(parseInt("0x10", undefined)); // 16 (hex prefix detected)

// null radix (ToInt32(null) = 0, triggers auto-detection)
console.log(parseInt("10", null));       // 10
console.log(parseInt("0x10", null));     // 16

// Boolean radix (ToInt32(true) = 1, ToInt32(false) = 0)
console.log(parseInt("10", true));       // NaN (radix 1 is invalid)
console.log(parseInt("10", false));      // 10 (radix 0 → auto-detect)
console.log(parseInt("0x10", false));    // 16 (radix 0 → hex prefix)

// Double radix (ToInt32 conversion)
console.log(parseInt("10", 10.5));       // 10 (10.5 → 10)
console.log(parseInt("10", 16.9));       // 16 (16.9 → 16)

// Out-of-range radix
console.log(parseInt("10", 1));          // NaN (radix < 2)
console.log(parseInt("10", 37));         // NaN (radix > 36)
console.log(parseInt("10", -1));         // NaN (radix < 0)

// ============================================================================
// Hex Prefix Handling (0x/0X)
// ============================================================================
console.log("=== Hex Prefix Handling ===");

// Radix absent or 0: hex prefix triggers radix 16
console.log(parseInt("0x10"));           // 16
console.log(parseInt("0X10"));           // 16 (case-insensitive)
console.log(parseInt("0x10", 0));        // 16

// Radix 16: hex prefix is allowed and stripped
console.log(parseInt("0x10", 16));       // 16
console.log(parseInt("0X10", 16));       // 16

// Radix non-16 (and non-0): hex prefix is NOT stripped, treated as digits
console.log(parseInt("0x10", 10));       // 0 (stops at 'x', which is not a decimal digit)
console.log(parseInt("0x10", 8));        // 0 (stops at 'x')

// Hex prefix with sign
console.log(parseInt("-0x10"));          // -16
console.log(parseInt("+0x10"));          // 16

// ============================================================================
// Digit Scanning and Stop-at-First-Invalid
// ============================================================================
console.log("=== Digit Scanning ===");

// Stops at first invalid character for the given radix
console.log(parseInt("15px", 10));       // 15 (stops at 'p')
console.log(parseInt("12.34", 10));      // 12 (stops at '.')
console.log(parseInt("101xyz", 2));      // 5 (stops at 'x')

// All characters invalid for radix
console.log(parseInt("xyz", 10));        // NaN (no valid prefix)
console.log(parseInt("ghijk", 16));      // NaN (no valid hex digits)

// ============================================================================
// Empty and Invalid Inputs
// ============================================================================
console.log("=== Empty and Invalid Inputs ===");

// Empty string
console.log(parseInt(""));               // NaN

// Only whitespace
console.log(parseInt("   "));            // NaN

// Only sign (no digits after)
console.log(parseInt("+"));              // NaN
console.log(parseInt("-"));              // NaN

// Sign + whitespace + no digits
console.log(parseInt("+   "));           // NaN
console.log(parseInt("-   "));           // NaN

// ============================================================================
// Case-Insensitive Alphabetic Digits (Radix > 10)
// ============================================================================
console.log("=== Case-Insensitive Alphabetic Digits ===");

// Radix 16 (A-F)
console.log(parseInt("abc", 16));        // 2748 (0xABC)
console.log(parseInt("ABC", 16));        // 2748 (0xABC)
console.log(parseInt("aBc", 16));        // 2748 (0xABC)

// Radix 36 (A-Z)
console.log(parseInt("z", 36));          // 35
console.log(parseInt("Z", 36));          // 35

// Mixed case
console.log(parseInt("Hello", 36));      // large number
console.log(parseInt("HELLO", 36));      // same large number

// ============================================================================
// ToString Coercion of First Argument
// ============================================================================
console.log("=== ToString Coercion ===");

// Number to string
console.log(parseInt(123));              // 123
console.log(parseInt(0x10));             // 16 (not "0x10", just "16")

// Boolean to string
console.log(parseInt(true));             // NaN ("true" has no digits)
console.log(parseInt(false));            // NaN ("false" has no digits)

// Object with toString
console.log(parseInt({ toString: function() { return "42"; }})); // 42

// Array (uses join/toString)
console.log(parseInt(["10"]));           // 10
console.log(parseInt([10, 20]));         // 10 (becomes "10,20", stops at comma)

// ============================================================================
// Edge Cases from Spec Steps
// ============================================================================
console.log("=== Spec Edge Cases ===");

// Leading zeros (not octal in strict mode for parseInt)
console.log(parseInt("010"));            // 10 (not 8, radix defaults to 10)
console.log(parseInt("010", 8));         // 8
console.log(parseInt("010", 0));         // 10 (radix 0 → auto-detect, no 0x prefix)

// Just prefix with no digits after
console.log(parseInt("0x"));             // NaN (no digits after prefix)
console.log(parseInt("0x", 16));         // NaN

// Trailing whitespace is ignored during scanning but not stripped before parsing
console.log(parseInt("42   "));          // 42 (stops at space during digit scanning)

// Large numbers exceeding safe integer range (JS parseInt returns doubles)
console.log(parseInt("9007199254740992")); // 9007199254740992 (2^53, still precise)
console.log(parseInt("90071992547409921234567890")); // very large, may lose precision
