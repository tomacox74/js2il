'use strict';

const nums = [1, 2, 3, 4, 5];

const doubled = nums.map(function(n) { return n * 2; });
const evens = doubled.filter(function(n) { return n % 2 === 0; });
const sum = evens.reduce(function(acc, n) { return acc + n; }, 0);

console.log('doubled=' + doubled.join(','));
console.log('evens=' + evens.join(','));
console.log('sum=' + sum);
console.log('CANARY:array-methods:ok');
