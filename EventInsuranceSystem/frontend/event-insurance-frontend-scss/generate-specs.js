const fs = require('fs');
const path = require('path');

function generateSpec(filePath) {
    const compName = path.basename(filePath, '.component.ts');
    const specPath = filePath.replace('.ts', '.spec.ts');

    if (fs.existsSync(specPath)) return;

    const classNameMatch = fs.readFileSync(filePath, 'utf8').match(/export class (\w+)/);
    if (!classNameMatch) return;
    const className = classNameMatch[1];

    const content = `import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { ${className} } from './${compName}.component';

describe('${className}', () => {
  let component: ${className};
  let fixture: ComponentFixture<${className}>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [${className}],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([])
      ]
    }).compileComponents();
    
    fixture = TestBed.createComponent(${className});
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
`;

    fs.writeFileSync(specPath, content);
    console.log('Created: ' + specPath);
}

function walkDir(dir) {
    const files = fs.readdirSync(dir);
    for (const file of files) {
        const fullPath = path.join(dir, file);
        if (fs.statSync(fullPath).isDirectory() && !fullPath.includes('node_modules')) {
            walkDir(fullPath);
        } else if (fullPath.endsWith('.component.ts')) {
            generateSpec(fullPath);
        }
    }
}

const rootDir = 'C:/Users/Lenovo/Desktop/Capstone/EventInsuranceSystem/event-insurance-frontend/src/app';
walkDir(rootDir);
console.log('Spec generation complete.');
