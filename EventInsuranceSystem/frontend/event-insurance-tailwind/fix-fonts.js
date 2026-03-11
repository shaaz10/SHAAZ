const fs = require('fs');
const path = require('path');

function walk(dir) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(file => {
        file = path.resolve(dir, file);
        const stat = fs.statSync(file);
        if (stat && stat.isDirectory()) {
            results = results.concat(walk(file));
        } else if (file.endsWith('.html') || file.endsWith('.ts')) {
            results.push(file);
        }
    });
    return results;
}

const files = walk('C:/Users/Lenovo/Desktop/Capstone/EventInsuranceSystem/event-insurance-tailwind/src');

files.forEach(file => {
    let content = fs.readFileSync(file, 'utf8');
    // We want to replace exactly `style="font-family: var(--font-family-heading);"`
    // Or `style="font-family:var(--font-family-heading)"`
    // Wait, replacing it with `class="font-heading"` is dangerous if there are already other classes on the exact same line,
    // which leads to multiple class="..." attributes! Wait, it's safer to just replace `--font-family-heading` with `--font-heading`.
    content = content.replace(/--font-family-heading/g, '--font-heading');
    content = content.replace(/--font-family-sans/g, '--font-sans');
    fs.writeFileSync(file, content);
});
console.log('Fixed fonts in ' + files.length + ' files');
