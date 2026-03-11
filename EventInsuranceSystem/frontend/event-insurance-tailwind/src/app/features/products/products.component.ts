import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../core/services/product.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products.component.html',
  styleUrl: './products.component.css'})
export class ProductsComponent {
  eventDetails = '';
  loading = false;
  suggestions: any[] = [];

  constructor(
    private productService: ProductService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  isCustomer(): boolean {
    return this.authService.hasRole('Customer');
  }

  getSuggestions() {
    this.loading = true;
    this.suggestions = [];
    this.cdr.detectChanges();
    this.productService.suggestPolicies(this.eventDetails).subscribe({
      next: (res) => {
        // Backend returns an array or single object inside `res`. 
        // Our controller returns a `List<PolicySuggestionResponseDto>`, so it should be an array.
        this.suggestions = res;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        alert('Error communicating with API.');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  apply(suggestion: any) {
    const customerId = this.authService.currentUserId();
    if (!customerId) {
      alert("Please login first to apply.");
      return;
    }

    this.productService.createApplication(suggestion.id, customerId).subscribe({
      next: (res) => {
        alert(`Application Created Successfuly! Application ID: ${res.applicationId}`);
      },
      error: (err) => {
        alert('Error creating application');
      }
    });
  }
}
