import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { AgentPortalComponent } from './agent-portal.component';

describe('AgentPortalComponent', () => {
  let component: AgentPortalComponent;
  let fixture: ComponentFixture<AgentPortalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AgentPortalComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([])
      ]
    }).compileComponents();
    
    fixture = TestBed.createComponent(AgentPortalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
