import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-under-construction',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-[calc(100vh-86px)] bg-background flex items-center justify-center p-4">
      <div class="max-w-xl w-full bg-white rounded-2xl shadow-xl overflow-hidden animate-scale-in border border-border">
        <div class="h-2 w-full bg-gradient-to-r from-primary to-primary-light"></div>
        <div class="p-8 md:p-12 text-center">
          <div class="text-primary-100 bg-primary/10 rounded-full w-24 h-24 flex items-center justify-center mx-auto mb-6 shadow-sm border-4 border-white animate-bounce-in">
            <i class="fa-solid fa-person-digging text-5xl text-primary"></i>
          </div>
          <h1 class="text-3xl md:text-4xl font-extrabold text-primary-dark mb-4" style="font-family:var(--font-heading)">Under Construction</h1>
          <p class="text-text-secondary text-lg mb-4 leading-relaxed">
            This premium feature is currently being built to provide you with the best experience.
          </p>
          <div class="bg-primary/5 rounded-xl p-4 mb-8 border border-primary/10">
            <p class="text-primary font-medium text-sm">
              <i class="fa-solid fa-screwdriver-wrench mr-2"></i>
              Our engineering team is working hard to bring you the Agentless Policy Request system. Stay tuned for updates!
            </p>
          </div>
          <div class="flex justify-center">
            <a routerLink="/" class="inline-flex items-center justify-center px-8 py-3.5 bg-primary text-white font-bold text-sm rounded-xl transition-all duration-300 hover:bg-primary-light hover:shadow-lg hover:-translate-y-0.5 w-full sm:w-auto">
              <i class="fa-solid fa-house mr-2"></i> Back to Home
            </a>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    @reference "../../../styles.css";
  `]
})
export class UnderConstructionComponent { }
