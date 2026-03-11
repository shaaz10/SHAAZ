import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClaimService } from '../../core/services/claim.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-claims',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './claims.component.html',
  styleUrl: './claims.component.scss'})
export class ClaimsComponent {
  claimAmount = 15000;
  description = "Event cancelled due to bad weather, requesting claim settlement";

  loading = false;
  loadingFraud = false;

  recentClaimId: number | null = null;
  fraudResult: any = null;

  constructor(
    private claimService: ClaimService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  isCustomer(): boolean {
    return this.authService.hasRole('Customer');
  }

  submitClaim() {
    const customerId = this.authService.currentUserId();
    if (!customerId) {
      alert("Please login first to submit a claim.");
      return;
    }

    this.loading = true;
    this.cdr.detectChanges();
    const payload = {
      policyId: 1,
      customerId: customerId,
      claimAmount: this.claimAmount,
      description: this.description
    };

    this.claimService.raiseClaim(payload).subscribe({
      next: (res: any) => {
        this.recentClaimId = res.id;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        alert('Error submitting claim. ' + (err.error?.message || ''));
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  runFraudCheck() {
    if (!this.recentClaimId) return;
    this.loadingFraud = true;
    this.cdr.detectChanges();
    this.claimService.detectFraud(this.recentClaimId).subscribe({
      next: (res: any) => {
        this.fraudResult = res;
        this.loadingFraud = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        alert('Error running fraud check.');
        this.loadingFraud = false;
        this.cdr.detectChanges();
      }
    });
  }
}
