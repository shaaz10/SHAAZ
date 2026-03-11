import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  loading = signal(false);
  error = signal('');
  showPassword = signal(false);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    // Check if the user is already authenticated upon component initialization
    // If they are, redirect them immediately to their respective dashboard
    if (this.authService.currentUserValue) {
      this.routeToDashboard();
    }

    // Initialize the reactive login form with validation rules for email and password
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  /**
   * Toggles the visibility of the password input field.
   * Switches the input type between 'text' and 'password'.
   */
  togglePassword() {
    this.showPassword.set(!this.showPassword());
  }

  /**
   * Handles the form submission event.
   * Validates form state, triggers the authentication service, and manages loading/error states.
   */
  onSubmit() {
    // Prevent submission if form validation fails
    if (this.loginForm.invalid) return;

    this.loading.set(true);
    this.error.set('');

    // Attempt to log in using the provided credentials
    this.authService.login(this.loginForm.value).subscribe({
      next: (user) => {
        // Upon successful login, navigate the user based on their specific role
        this.routeToDashboard();
      },
      error: (err: any) => {
        // Log error and provide user-friendly feedback if authentication fails
        console.error('Login error:', err);

        // Extract the clean message from the C# backend, or provide a safe generic fallback.
        // This prevents showing raw "Http failure response for... 401 OK" strings on the UI.
        const errorMessage = typeof err.error === 'string' ? err.error
          : err.error?.message ? err.error.message
            : 'Invalid email or password. Please try again.';

        this.error.set(errorMessage);
        this.loading.set(false);
      }
    });
  }

  /**
   * Redirects the user to the appropriate dashboard based on their assigned role in the system.
   */
  private routeToDashboard() {
    if (this.authService.hasRole('Admin')) {
      this.router.navigate(['/dashboard/admin']);
    } else if (this.authService.hasRole('Agent')) {
      this.router.navigate(['/dashboard/agent']);
    } else if (this.authService.hasRole('Customer')) {
      this.router.navigate(['/dashboard/customer']);
    } else if (this.authService.hasRole('ClaimsOfficer')) {
      this.router.navigate(['/dashboard/claims-officer']);
    } else {
      // Default fallback for users with unrecognized or no roles
      this.router.navigate(['/']);
    }
  }
}
