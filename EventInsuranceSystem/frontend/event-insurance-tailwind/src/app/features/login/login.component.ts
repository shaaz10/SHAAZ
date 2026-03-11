import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading = signal(false);
  error = signal('');
  showPassword = signal(false);

  // CAPTCHA State
  captchaNum1 = signal(0);
  captchaNum2 = signal(0);
  captchaOp = signal('+');
  captchaAnswer = signal(0);
  captchaInput = '';
  captchaError = signal(false);
  captchaShake = signal(false);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    if (this.authService.currentUserValue) {
      this.routeToDashboard();
    }
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.generateCaptcha();
  }

  generateCaptcha() {
    const ops = ['+', '-', '×'];
    const op = ops[Math.floor(Math.random() * ops.length)];
    let n1 = Math.floor(Math.random() * 9) + 1;
    let n2 = Math.floor(Math.random() * 9) + 1;
    let answer: number;
    if (op === '+') { answer = n1 + n2; }
    else if (op === '-') { if (n1 < n2) { [n1, n2] = [n2, n1]; } answer = n1 - n2; }
    else { answer = n1 * n2; }
    this.captchaNum1.set(n1);
    this.captchaNum2.set(n2);
    this.captchaOp.set(op);
    this.captchaAnswer.set(answer);
    this.captchaInput = '';
    this.captchaError.set(false);
  }

  togglePassword() {
    this.showPassword.set(!this.showPassword());
  }

  onSubmit() {
    if (this.loginForm.invalid) return;

    // Validate CAPTCHA
    if (parseInt(this.captchaInput, 10) !== this.captchaAnswer()) {
      this.captchaError.set(true);
      this.captchaShake.set(true);
      setTimeout(() => { this.captchaShake.set(false); this.generateCaptcha(); }, 600);
      return;
    }

    this.loading.set(true);
    this.error.set('');

    this.authService.login(this.loginForm.value).subscribe({
      next: () => { this.routeToDashboard(); },
      error: (err: any) => {
        const errorMessage = typeof err.error === 'string' ? err.error
          : err.error?.message ? err.error.message
            : 'Invalid email or password. Please try again.';
        this.error.set(errorMessage);
        this.loading.set(false);
        this.generateCaptcha();
      }
    });
  }

  private routeToDashboard() {
    if (this.authService.hasRole('Admin')) this.router.navigate(['/dashboard/admin']);
    else if (this.authService.hasRole('Agent')) this.router.navigate(['/dashboard/agent']);
    else if (this.authService.hasRole('Customer')) this.router.navigate(['/dashboard/customer']);
    else if (this.authService.hasRole('ClaimsOfficer')) this.router.navigate(['/dashboard/claims-officer']);
    else this.router.navigate(['/']);
  }
}
