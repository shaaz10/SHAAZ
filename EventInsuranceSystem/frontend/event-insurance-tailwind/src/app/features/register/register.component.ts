import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
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

  togglePassword() { this.showPassword.set(!this.showPassword()); }

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    if (this.authService.currentUserValue) { this.routeToDashboard(); }
    this.registerForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngOnInit() { this.generateCaptcha(); }

  generateCaptcha() {
    const ops = ['+', '-', '×'];
    const op = ops[Math.floor(Math.random() * ops.length)];
    let n1 = Math.floor(Math.random() * 9) + 1;
    let n2 = Math.floor(Math.random() * 9) + 1;
    let answer: number;
    if (op === '+') { answer = n1 + n2; }
    else if (op === '-') { if (n1 < n2) { [n1, n2] = [n2, n1]; } answer = n1 - n2; }
    else { answer = n1 * n2; }
    this.captchaNum1.set(n1); this.captchaNum2.set(n2);
    this.captchaOp.set(op); this.captchaAnswer.set(answer);
    this.captchaInput = ''; this.captchaError.set(false);
  }

  onSubmit() {
    if (this.registerForm.invalid) return;

    // Validate CAPTCHA
    if (parseInt(this.captchaInput, 10) !== this.captchaAnswer()) {
      this.captchaError.set(true);
      this.captchaShake.set(true);
      setTimeout(() => { this.captchaShake.set(false); this.generateCaptcha(); }, 600);
      return;
    }

    this.loading.set(true);
    this.error.set('');
    const val = this.registerForm.value;
    this.authService.register(val).subscribe({
      next: () => {
        this.authService.login({ email: val.email, password: val.password }).subscribe({
          next: () => { this.routeToDashboard(); },
          error: () => { this.router.navigate(['/login']); }
        });
      },
      error: (err: any) => {
        this.error.set(err.error?.message || 'Registration failed.');
        this.loading.set(false);
        this.generateCaptcha();
      }
    });
  }

  private routeToDashboard() {
    const role = this.authService.currentUserRole();
    if (role === 'Customer') this.router.navigate(['/dashboard/customer']);
    else if (role === 'Agent') this.router.navigate(['/dashboard/agent']);
    else if (role === 'ClaimsOfficer') this.router.navigate(['/dashboard/claims-officer']);
    else if (role === 'Admin') this.router.navigate(['/dashboard/admin']);
    else this.router.navigate(['/']);
  }
}
