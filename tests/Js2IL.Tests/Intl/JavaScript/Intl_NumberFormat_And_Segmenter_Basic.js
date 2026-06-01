const formatter = new Intl.NumberFormat();
console.log(formatter.format(1234567));

const segmenter = new Intl.Segmenter();
for (const {segment} of segmenter.segment("A😀B")) {
    console.log(segment);
}
