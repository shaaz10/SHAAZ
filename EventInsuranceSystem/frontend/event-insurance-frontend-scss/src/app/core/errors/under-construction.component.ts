import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-under-construction',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="error-container">
      <div class="error-card">
        <div class="error-icon">
          <img src="under-construction.gif" alt="Working..." style="max-width: 250px; margin-bottom: 1rem;">
          <br>
          <i class="fa-solid fa-person-digging"></i>
        </div>
        <h1>Under Construction</h1>
        <p>This premium feature is currently being built to provide you with the best experience.</p>
        <p class="secondary-text">Our engineering team is working hard to bring you the Agentless Policy Request system. Stay tuned for updates!</p>
        <div class="actions">
          <a routerLink="/" class="btn btn-primary">Back to Home</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .error-container {
      min-height: calc(100vh - 86px);
      display: flex;
      align-items: center;
      justify-content: center;
      background-color: #f8fafc;
      padding: 3rem 1rem;
    }
    .error-card {
      text-align: center;
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 10px 30px rgba(0,0,0,0.05);
      max-width: 600px;
      width: 100%;
      border-top: 5px solid #004B87;
    }
    .error-icon {
      margin-bottom: 1.5rem;
      i {
          display: block;
          font-size: 3rem;
          color: #004b87;
          margin-top: 1rem;
      }
    }
    h1 {
      font-size: 2.5rem;
      color: #1a202c;
      margin-bottom: 1rem;
    }
    p {
      color: #4a5568;
      font-size: 1.1rem;
      margin-bottom: 1rem;
    }
    .secondary-text {
        font-size: 0.95rem;
        opacity: 0.8;
    }
    .actions {
      margin-top: 2rem;
    }
    .btn {
      display: inline-block;
      padding: 0.75rem 2rem;
      border-radius: 6px;
      text-decoration: none;
      font-weight: 600;
      transition: all 0.3s;
    }
    .btn-primary {
      background-color: #004B87;
      color: white;
    }
    .btn-primary:hover {
      background-color: #003764;
      transform: translateY(-2px);
    }
  `]
})
export class UnderConstructionComponent { }
