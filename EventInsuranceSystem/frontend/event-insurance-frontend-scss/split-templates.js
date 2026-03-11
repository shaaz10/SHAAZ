const fs = require('fs');
const path = require('path');

function processFile(filePath) {
    let content = fs.readFileSync(filePath, 'utf8');
    let hasChanges = false;

    // Extract template
    const templateRegex = /template:\s*`([\s\S]*?)`\s*(,?)/;
    const match = content.match(templateRegex);

    if (match) {
        const htmlContent = match[1];
        const htmlPath = filePath.replace('.ts', '.html');
        fs.writeFileSync(htmlPath, htmlContent.trim() + '\n');
        content = content.replace(templateRegex, `templateUrl: './${path.basename(htmlPath)}'$2`);
        hasChanges = true;
    }

    // Extract styles
    const stylesRegex = /styles:\s*\[\s*`([\s\S]*?)`\s*\]\s*(,?)/;
    const matchStyles = content.match(stylesRegex);

    if (matchStyles) {
        const cssContent = matchStyles[1];
        const cssPath = filePath.replace('.ts', '.scss');
        fs.writeFileSync(cssPath, cssContent.trim() + '\n');
        content = content.replace(stylesRegex, `styleUrl: './${path.basename(cssPath)}'$2`);
        hasChanges = true;
    }

    if (hasChanges) {
        fs.writeFileSync(filePath, content);
        console.log(`Processed: ${filePath}`);
    }
}

function walkDir(dir) {
    const files = fs.readdirSync(dir);
    for (const file of files) {
        const fullPath = path.join(dir, file);
        if (fs.statSync(fullPath).isDirectory() && !fullPath.includes('node_modules')) {
            walkDir(fullPath);
        } else if (fullPath.endsWith('.component.ts')) {
            processFile(fullPath);
        }
    }
}

const rootDir = 'C:/Users/Lenovo/Desktop/Capstone/EventInsuranceSystem/event-insurance-frontend/src/app';
walkDir(rootDir);
console.log('Template separation complete.');
