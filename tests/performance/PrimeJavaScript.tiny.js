"use strict";
const NOW_UNITS_PER_SECOND = 1000;
let config = { sieveSize: 10000, timeLimitSeconds: 0.1, verbose: false, runtime: '' };
const { performance } = require('perf_hooks');
const runtimeParts = process.argv[0].split(/[\\\/]/);
config.runtime = runtimeParts[runtimeParts.length - 1];
class BitArray{ constructor(size){ this.wordArray = new Int32Array(1 + (size >>> 5)); } setBitTrue(i){ const w=i>>>5; const b=i&31; this.wordArray[w]|=(1<<b);} setBitsTrue(s,st,e){ let i=s; let w=i>>>5; let v=this.wordArray[w]; while(i<e){ const b=i&31; v|=(1<<b); i+=st; const nw=i>>>5; if(nw!=w){ this.wordArray[w]=v; w=nw; v=this.wordArray[w]; } } this.wordArray[w]=v; } testBitTrue(i){ const w=i>>>5; const b=i&31; return this.wordArray[w]&(1<<b);} searchBitFalse(i){ while(this.testBitTrue(i)){i++;} return i;}}
class PrimeSieve{ constructor(n){ this.sieveSize=n; this.sieveSizeInBits=n>>>1; this.bitArray=new BitArray(1+this.sieveSizeInBits);} runSieve(){ const q=Math.ceil(Math.sqrt(this.sieveSizeInBits)); let f=1; while(f<q){ const st=f*2+1; const stt=f*f*2+f+f; this.bitArray.setBitsTrue(stt,st,this.sieveSizeInBits); f=this.bitArray.searchBitFalse(f+1);} return this;} countPrimes(){ let t=1; for(let i=1;i<this.sieveSizeInBits;i++){ if(!this.bitArray.testBitTrue(i)){t++;}} return t;} }
(function main(){ const timeStart=performance.now(); let passes=0; const timeFinish=timeStart + config.timeLimitSeconds * NOW_UNITS_PER_SECOND; do{ new PrimeSieve(config.sieveSize).runSieve(); passes++; } while (performance.now() < timeFinish); console.log('tiny passes:', passes); })();
