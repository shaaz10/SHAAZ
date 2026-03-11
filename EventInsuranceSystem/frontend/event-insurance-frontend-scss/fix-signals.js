const fs = require('fs');
const path = require('path');

const dir = 'C:/Users/Lenovo/Desktop/Capstone/EventInsuranceSystem/event-insurance-frontend/src/app/features';

function walkDir(d) {
    const files = fs.readdirSync(d);
    for (const file of files) {
        const fullPath = path.join(d, file);
        if (fs.statSync(fullPath).isDirectory()) {
            walkDir(fullPath);
        } else if (fullPath.endsWith('.component.ts')) {
            let content = fs.readFileSync(fullPath, 'utf8');
            let changed = false;

            // Replace .currentUserId (not followed by () already) with .currentUserId()
            const r1 = content.replace(/\.currentUserId(?!\()/g, '.currentUserId()');
            if (r1 !== content) { content = r1; changed = true; }

            // Replace .currentUserRole (not followed by () already) with .currentUserRole()
            const r2 = content.replace(/\.currentUserRole(?!\()/g, '.currentUserRole()');
            if (r2 !== content) { content = r2; changed = true; }

            if (changed) {
                fs.writeFileSync(fullPath, content);
                console.log('Fixed signals in: ' + fullPath);
            }
        }
    }
}

walkDir(dir);
console.log('Done fixing signal calls.');
