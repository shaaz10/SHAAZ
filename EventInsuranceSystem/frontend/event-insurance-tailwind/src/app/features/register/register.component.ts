import { Component, signal } from '@angular/core';
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
export class RegisterComponent {
    registerForm: FormGroup;
    loading = signal(false);
    error = signal('');
    showPassword = signal(false);

    togglePassword() {
        this.showPassword.set(!this.showPassword());
    }

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router
    ) {
        if (this.authService.currentUserValue) {
            this.routeToDashboard();
        }
        this.registerForm = this.fb.group({
            fullName: ['', Validators.required],
            email: ['', [Validators.required, Validators.email]],
            phoneNumber: ['', Validators.required],
            password: ['', Validators.required]
        });
    }

    onSubmit() {
        if (this.registerForm.invalid) return;

        this.loading.set(true);
        this.error.set('');

        const val = this.registerForm.value;

        this.authService.register(val).subscribe({
            next: (res: any) => {
                // Automatically login after successful registration
                this.authService.login({ email: val.email, password: val.password }).subscribe({
                    next: () => {
                        this.routeToDashboard();
                    },
                    error: () => {
                        this.router.navigate(['/login']);
                    }
                });
            },
            error: (err: any) => {
                this.error.set(err.error?.message || 'Registration failed.');
                this.loading.set(false);
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
