"use strict";

function runCommon(s) {
    console.log(s.charCodeAt());
    console.log(s.charCodeAt(1));
    console.log(s.substring(1, 4));
    console.log(s.slice(2, 5));
    console.log(s.indexOf('cd'));
    console.log(s.indexOf('cd', 3));
    console.log(s.startsWith('ab'));
    console.log(s.startsWith('bc', 1));
    console.log(s.includes('de'));
    console.log(s.trim());
}

runCommon('abcdef');
console.log('  x  '.trim());
console.log('A'.toLowerCase());
console.log('a'.toUpperCase());
