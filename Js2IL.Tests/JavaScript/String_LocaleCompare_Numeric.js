const items = ["2", "10", "1"]; 
items.sort((a,b)=> String(a).localeCompare(String(b), undefined, { numeric: true }));
for (let i=0;i<items.length;i++){ console.log(items[i]); }
