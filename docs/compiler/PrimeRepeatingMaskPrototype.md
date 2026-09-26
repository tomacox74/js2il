# Prime repeating-mask block prototype

This is a benchmark-only alternative to the large-step branch of
`BitArray.setBitsTrue` in `tests/performance/PrimeJavaScript.js`. It is **not**
used by the compiler or the production Prime script; the generated Prime IL
continues to execute the original strided loop.

For an eligible call, let `s` be `range_start`, `d` be `step`, and `R` be
`range_stop`. The current large-step branch runs when `d > 16` and
`s + 32d <= R`. For `j = 0..31`, it computes

```text
i_j = s + j*d
w_j = floor(i_j / 32)
m_j = 1 << (i_j mod 32)
T   = floor(R / 32)
```

It ORs `m_j` into every word `w_j + k*d <= T` for `k >= 0`. In particular,
the current code compares **word offsets inclusively** against `T`, even
though `R` is an exclusive bit limit elsewhere in the sieve. An equivalent
prototype must retain this last-word behavior rather than silently correcting
it. Out-of-bounds typed-array writes remain no-ops.

The 32 initial word offsets occupy at most `d + 1` positions: because
`i_31 - i_0 = 31d`, `w_31 - w_0 <= d`. Build a `d`-word mask by ORing each
`m_j` into slot `(w_j - w_0) mod d`. Apply each initial `m_j` at `w_j` as
the first partial block; then, beginning at `w_0 + d`, OR the mask block
contiguously until `T` or the end of the backing array. Every subsequent
write from the original loop has exactly that residue modulo `d`. When
`w_31 = w_0 + d`, the first and last initial offsets share a residue:
ORing masks in that slot and applying the initial writes separately preserves
both contributions. Clamp the last block to the inclusive final word and the
array length; vector loads/stores operate only within the clamped block, with
scalar prologue/tail as needed.

The scalar block prototype establishes equivalence before using portable
`Vector128<int>` OR. It walks each contiguous block but skips array
read/modify/write for zero mask slots; the vector version loads and stores
whole vectors, including lanes whose mask is zero. Eligible calls require
an exact `Int32Array` with fixed, non-shared, non-detached contiguous backing
and safe integer bounds; all other cases retain the existing algorithm.
Benchmark setup compares the original, scalar-block, and vector-block
buffers before measuring them.

This transformation trades sparse strided writes for contiguous scans and
construction of `d` mask words. The original kernel performs roughly
`32 * (T - w_0) / d` read/modify/write operations. Both block variants
inspect roughly `T - w_0` mask slots, while the vector version also
reads and writes zero-mask array lanes and the scalar variant does not.
Both pay for the first partial block and mask allocation. They may
therefore lose for large `d` despite SIMD. The controlled kernel benchmark
`--prime-mask-blocks` measures elapsed time and allocation with hardware
intrinsics enabled and disabled; `MaskConstruction` isolates setup cost.
These are algorithm-kernel measurements, not claims about end-to-end JROC
execution. The original Prime one-pass benchmark remains the user-visible
baseline until an eligible transformation is justified by a repeatable gain.

## Initial measurements

On x64 Intel Xeon 6975P-C with .NET 10.0.12, two local BenchmarkDotNet
ShortRuns (`N=3` measured iterations per case per run) measured the **C#
kernels** over a 500,000-bit range. Each block timing includes mask
construction; `setup` isolates that cost. First-run means in microseconds,
with hardware intrinsics enabled:

| Step | Original | Scalar blocks | Vector blocks | Setup (ns) | Block allocation |
| ---: | ---: | ---: | ---: | ---: | ---: |
| 17 | 29.98 | 9.45 | 3.93 | 60 | 96 B |
| 31 | 8.80 | 13.30 | 3.36 | 74 | 152 B |
| 61 | 3.86 | 7.09 | 2.72 | 69 | 272 B |
| 127 | 2.16 | 7.05 | 3.64 | 89 | 536 B |
| 251 | 1.28 | 7.12 | 3.03 | 107 | 1,032 B |
| 509 | 0.44 | 4.32 | 2.22 | 123 | 2,064 B |

The original allocates 0 B. With `DOTNET_EnableHWIntrinsic=0`, both block
paths win at step 17 (10.45/10.59 vs. 30.46 µs), but generally lose from
step 31 onward. A second independent ShortRun reproduced the direction at
every sampled step: at steps 17/31/61/127, original versus scalar versus
vector took 29.84/9.59/3.72, 7.80/11.62/3.30, 3.93/6.65/2.50,
and 2.07/6.86/3.41 µs respectively. The observed scalar crossover lies
between steps 17 and 31, and the vector crossover between 61 and 127;
these are **not calibrated production thresholds**. Two short local runs
do not establish a benefit on other machines or an end-to-end speedup:
no JavaScript compilation, module initialization, or full Prime pass is
included in the timed region.

Instrumented target-array reads and writes (each count applies to both reads
and writes) for the same range explain the reversal:

| Step | Original words | Scalar words | Vector words (intrinsics on) |
| ---: | ---: | ---: | ---: |
| 17 | 29,406 | 15,622 | 15,622 |
| 31 | 16,115 | 15,611 | 15,611 |
| 61 | 8,167 | 8,167 | 15,312 |
| 127 | 3,874 | 3,874 | 15,008 |
| 251 | 1,867 | 1,867 | 14,468 |
| 509 | 728 | 728 | 11,556 |

The scalar path skips target writes for zero mask slots, so its traffic
converges to the original. The vector path reads and writes zero-mask lanes:
their cost dominates the diminishing number of useful words at large steps.
The mask is built once **per prototype invocation**, not cached across
invocations. Keep the production strided fallback: before adopting any block
path, characterize cross-machine crossover and establish an end-to-end Prime
benefit without compromising backing-store safety or exact bit equivalence.
