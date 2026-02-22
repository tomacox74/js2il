# Benchmark Script Provenance and Licensing

This document tracks the origin and licensing of benchmark scripts used in this project.

## Jint Benchmark Scripts

**Source Repository:** https://github.com/sebastienros/jint  
**License:** BSD 2-Clause License  
**Copyright:** Copyright (c) 2013, Sebastien Ros  
**Commit/Version:** Referenced from main branch (2026-02-15)  
**Original Location:** Jint.Benchmark/Scripts/

### Scripts Copied

The following scripts were copied from the Jint benchmark suite:

1. **minimal.js**
   - Original path: `Jint.Benchmark/Scripts/minimal.js`
   - Purpose: Minimal execution benchmark (simple arithmetic)
   - License: BSD 2-Clause (Jint project)

2. **evaluation.js**
   - Original path: `Jint.Benchmark/Scripts/evaluation.js`
   - Purpose: Object property evaluation and function recursion (fibonacci)
   - License: BSD 2-Clause (Jint project)

3. **evaluation-modern.js**
   - Original path: `Jint.Benchmark/Scripts/evaluation-modern.js`
   - Purpose: Modern ES6+ syntax version of evaluation benchmark (const, arrow functions)
   - License: BSD 2-Clause (Jint project)

4. **stopwatch.js**
   - Original path: `Jint.Benchmark/Scripts/stopwatch.js`
   - Purpose: Class instantiation and method calls with state management
   - License: BSD 2-Clause (Jint project)

5. **array-stress.js**
   - Original path: `Jint.Benchmark/Scripts/array-stress.js`
   - Purpose: Array manipulation stress test (push, pop, shift, unshift, splice)
   - License: BSD 2-Clause (Jint project)

6. **Additional scripts imported (minimal compatibility modification)**
   - Changes applied for js2il compatibility:
     - Prepended `"use strict";` to support strict-only parsing.
     - Added local no-op harness shims (`startTest`, `endTest`, `prep`, `test`) so scenarios can run standalone.
     - Replaced unsupported constructs in specific files:
       - `dromaeo-core-eval*.js`: removed `eval`/`new Function` dependency and executed equivalent benchmark body directly.
       - `dromaeo-string-base64*.js`: replaced `Components.Exception(...)` with a standard thrown string.
   - `Jint.Benchmark/Scripts/dromaeo-3d-cube-modern.js`
   - `Jint.Benchmark/Scripts/dromaeo-3d-cube.js`
   - `Jint.Benchmark/Scripts/dromaeo-core-eval-modern.js`
   - `Jint.Benchmark/Scripts/dromaeo-core-eval.js`
   - `Jint.Benchmark/Scripts/dromaeo-object-array-modern.js`
   - `Jint.Benchmark/Scripts/dromaeo-object-array.js`
   - `Jint.Benchmark/Scripts/dromaeo-object-regexp-modern.js`
   - `Jint.Benchmark/Scripts/dromaeo-object-regexp.js`
   - `Jint.Benchmark/Scripts/dromaeo-object-string-modern.js`
   - `Jint.Benchmark/Scripts/dromaeo-object-string.js`
   - `Jint.Benchmark/Scripts/dromaeo-string-base64-modern.js`
   - `Jint.Benchmark/Scripts/dromaeo-string-base64.js`
   - `Jint.Benchmark/Scripts/linq-js.js`
   - `Jint.Benchmark/Scripts/stopwatch-modern.js`

## License Compliance

The BSD 2-Clause License permits:
- ✅ Redistribution in source and binary forms
- ✅ Modification
- ✅ Commercial use

Requirements:
- ✅ Copyright notice must be retained
- ✅ License text must be included in distributions
- ✅ Disclaimer must be included

This project complies with these requirements by:
1. Including this provenance document with copyright and license information
2. Preserving original benchmark intent and script bodies as much as possible, with only targeted compatibility edits required by js2il
3. Including the full BSD 2-Clause license text below

## Full License Text

```
BSD 2-Clause License

Copyright (c) 2013, Sebastien Ros
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this 
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this 
   list of conditions and the following disclaimer in the documentation and/or 
   other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
OF THE POSSIBILITY OF SUCH DAMAGE.
```

## Modification Policy

If any of these scripts need modification for compatibility:
1. Document the change and rationale in this file
2. Consider creating a modified variant with a clear filename suffix
3. Maintain backward compatibility where possible

## Future Additions

When adding new benchmark scripts:
1. Document the source, license, and copyright
2. Verify license compatibility
3. Include proper attribution
4. Update this provenance document
