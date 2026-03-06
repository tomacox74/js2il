'use strict';

// Deterministic Sieve of Eratosthenes (limit = 50, no timing loop).
function sieve(limit) {
    var composite = [];
    var i;
    for (i = 0; i <= limit; i++) {
        composite[i] = false;
    }
    composite[0] = true;
    composite[1] = true;

    for (i = 2; i * i <= limit; i++) {
        if (!composite[i]) {
            var j = i * i;
            while (j <= limit) {
                composite[j] = true;
                j = j + i;
            }
        }
    }

    var primes = [];
    for (i = 2; i <= limit; i++) {
        if (!composite[i]) {
            primes.push(i);
        }
    }
    return primes;
}

var primes = sieve(50);
console.log('primes=' + primes.join(','));
console.log('count=' + primes.length);
console.log('CANARY:prime-mini:ok');
