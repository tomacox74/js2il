'use strict';
/**
 * Seeded deterministic JS program generator for differential testing.
 *
 * Each template targets one of the four risk areas from the issue:
 *   1. Control-flow joins (?: && ||)
 *   2. Loop back-edges and variable updates
 *   3. Numeric ops (+ - * / %) with mixed boxed/unboxed numbers
 *   4. Array .length and index-math patterns
 *
 * Usage (as a module):
 *   const { generate } = require('./generate');
 *   const files = generate(seed, count, outputDir);
 *
 * Usage (standalone):
 *   node scripts/differential-test/generate.js --seed 42 --count 5 --output /tmp/gen
 */

const fs   = require('fs');
const path = require('path');

// ---------------------------------------------------------------------------
// Seeded PRNG (LCG – Numerical Recipes parameters)
// ---------------------------------------------------------------------------
function makePRNG(seed) {
    let s = seed >>> 0;
    return {
        /** Returns a float in [0, 1) */
        next() {
            s = Math.imul(s, 1664525) + 1013904223 >>> 0;
            return s / 0x100000000;
        },
        /** Returns an integer in [min, max] */
        int(min, max) {
            return min + Math.floor(this.next() * (max - min + 1));
        },
        /** Returns one element chosen from array */
        pick(arr) {
            return arr[this.int(0, arr.length - 1)];
        },
    };
}

// ---------------------------------------------------------------------------
// Template generators
// Each template function receives (rng) and returns a JS source string.
// The program MUST be deterministic and not rely on Date / Math.random etc.
// ---------------------------------------------------------------------------

/**
 * Template A – arithmetic loop accumulator.
 * Risk: numeric boxing + loop back-edge.
 */
function templateArithmeticLoop(rng) {
    const limit = rng.int(5, 20);
    const ops   = [
        { sym: '+',  identity: 0,  fn: (a, b) => a + b },
        { sym: '-',  identity: 0,  fn: (a, b) => a - b },
        { sym: '*',  identity: 1,  fn: (a, b) => a * b },
    ];
    const op  = rng.pick(ops);
    const rhs = rng.int(1, 9);

    // Compute expected result
    let acc = op.identity;
    for (let i = 0; i < limit; i++) acc = op.fn(acc, rhs);

    return `"use strict";
// [generated] arithmetic loop – operator ${op.sym}, rhs ${rhs}, count ${limit}
let acc = ${op.identity};
for (let i = 0; i < ${limit}; i++) {
    acc = acc ${op.sym} ${rhs};
}
console.log(acc); // ${acc}
`;
}

/**
 * Template B – ternary / conditional in a loop.
 * Risk: control-flow join (phi-like merge).
 */
function templateTernaryLoop(rng) {
    const limit = rng.int(6, 15);
    const threshold = rng.int(1, limit - 1);
    const thenVal   = rng.int(1, 10);
    const elseVal   = rng.int(1, 10);

    let sum = 0;
    for (let i = 0; i < limit; i++) {
        sum += (i < threshold) ? thenVal : elseVal;
    }

    return `"use strict";
// [generated] ternary in loop – threshold ${threshold}, then ${thenVal}, else ${elseVal}
let sum = 0;
for (let i = 0; i < ${limit}; i++) {
    sum += (i < ${threshold}) ? ${thenVal} : ${elseVal};
}
console.log(sum); // ${sum}
`;
}

/**
 * Template C – while loop with conditional update.
 * Risk: back-edge with multiple possible variable values.
 */
function templateWhileConditional(rng) {
    const start = rng.int(10, 50);
    const step  = rng.int(1, 5);
    const stop  = rng.int(1, 5);

    let n = start, count = 0;
    const MAX_ITER = 200;
    while (n > stop && count < MAX_ITER) {
        n -= step;
        count++;
    }
    if (n < 0) n = 0;

    return `"use strict";
// [generated] while loop – start ${start}, step ${step}, stop ${stop}
let n = ${start};
let count = 0;
while (n > ${stop}) {
    n -= ${step};
    count++;
}
console.log(count); // ${count}
console.log(n <= ${stop} ? "done" : "incomplete"); // done
`;
}

/**
 * Template D – array construction + index-based sum.
 * Risk: array .length, index arithmetic.
 */
function templateArraySum(rng) {
    const len  = rng.int(4, 12);
    const vals = Array.from({ length: len }, () => rng.int(1, 20));
    const sum  = vals.reduce((a, b) => a + b, 0);

    return `"use strict";
// [generated] array index sum
const arr = [${vals.join(', ')}];
let total = 0;
for (let i = 0; i < arr.length; i++) {
    total += arr[i];
}
console.log(arr.length); // ${len}
console.log(total); // ${sum}
`;
}

/**
 * Template E – logical || / && chains with number coercion.
 * Risk: control-flow join via short-circuit operators.
 */
function templateLogicalChain(rng) {
    const a = rng.int(0, 2);   // 0 = falsy
    const b = rng.int(1, 10);  // non-zero (truthy)
    const c = rng.int(1, 10);

    const orResult  = a || b;   // should be b since a is 0
    const andResult = b && c;   // should be c
    const nullish   = (a === 0 ? null : a) ?? b;  // null ?? b = b

    return `"use strict";
// [generated] logical chain – a=${a}, b=${b}, c=${c}
const a = ${a}, b = ${b}, c = ${c};
console.log(a || b);  // ${orResult}
console.log(b && c);  // ${andResult}
console.log(${a === 0 ? 'null' : a} ?? ${b});  // ${nullish}
`;
}

// ---------------------------------------------------------------------------
// Public API
// ---------------------------------------------------------------------------

const TEMPLATES = [
    templateArithmeticLoop,
    templateTernaryLoop,
    templateWhileConditional,
    templateArraySum,
    templateLogicalChain,
];

/**
 * Generate `count` programs using a deterministic PRNG seeded with `seed`.
 * Programs are written to `outputDir` and the list of paths is returned.
 */
function generate(seed, count, outputDir) {
    fs.mkdirSync(outputDir, { recursive: true });
    const rng   = makePRNG(seed);
    const files = [];

    for (let i = 0; i < count; i++) {
        const tpl  = TEMPLATES[i % TEMPLATES.length];
        const src  = tpl(rng);
        const file = path.join(outputDir, `generated-${seed}-${String(i).padStart(4, '0')}.js`);
        fs.writeFileSync(file, src, 'utf8');
        files.push(file);
    }

    return files;
}

module.exports = { generate, makePRNG, TEMPLATES };

// ---------------------------------------------------------------------------
// CLI entry point
// ---------------------------------------------------------------------------
if (require.main === module) {
    const argv  = process.argv.slice(2);
    let seed    = 42;
    let count   = 10;
    let outDir  = path.join(__dirname, 'generated');

    for (let i = 0; i < argv.length; i++) {
        if (argv[i] === '--seed'   && argv[i+1]) { seed   = Number(argv[++i]); continue; }
        if (argv[i] === '--count'  && argv[i+1]) { count  = Number(argv[++i]); continue; }
        if (argv[i] === '--output' && argv[i+1]) { outDir = argv[++i];         continue; }
    }

    const files = generate(seed, count, outDir);
    files.forEach(f => console.log(f));
}
